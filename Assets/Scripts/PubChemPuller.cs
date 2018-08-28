using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

class PubChemPuller : MonoBehaviour
{


    public MoleculeData molData = new MoleculeData();

    private bool loading = false;
    public JObject dataOBJ;
    public int CID;
    public string ConformerID;
    public string molName;
    public string officialMolName;
    private int CIDNum;
    private bool isCID;

    //https://pubchem.ncbi.nlm.nih.gov/rest/pug/compound/cid/962/conformers/JSON


    //Called upon start, initiates CIDNum (The CID that will be taken from the CID list) to 0
    public void startRoutine(String goalMol)
    {
        molName = goalMol;
        CIDNum = 0;
        
        StartCoroutine(GetCID(molName));

        
    }



    IEnumerator GetCID(String molName)
    {
        //Checks whether molName is numeric, if so it starts coruntines using the CID, and skips the CID search web request
        isCID = false;
        if  (Regex.IsMatch(molName, @"^\d+$")){
            CID = Int32.Parse(molName);
            StartCoroutine(GetMolName(CID));

            StartCoroutine(GetConformerID(CID));
            isCID = true;

            Debug.Log("CID Entered!");
        }

        

        
        if (!isCID)
        {
            //Creates a UnityWebRequest in order to search for CIDs matching molname, yields until request is returned
            Debug.Log("SearchedMol: " + molName);
            UnityWebRequest www = UnityWebRequest.Get("https://pubchem.ncbi.nlm.nih.gov/rest/pug/compound/name/" + molName + "/cids/JSON?name_type=word");

            Debug.Log("Made web request");

            yield return www.SendWebRequest();
            Debug.Log("Checking for network error");


            //If the request returned a network error (No CIDS were found) display the billboard stating that none were found
            if (www.isNetworkError || www.isHttpError)
            {

                GameObject.FindWithTag("DictationResult").GetComponent<TextMesh>().text = "No Molecules found!";
                Debug.Log(www.error);
                Debug.Log("No valid molecules!");

            }
            else
            {

                // Show results as text
                Debug.Log(www.downloadHandler.text);

                


                JObject dataObj = JObject.Parse(www.downloadHandler.text);
                Debug.Log("CID" + parseForCID(dataObj));

                CID = parseForCID(dataObj);

                StartCoroutine(GetMolName(CID));

                StartCoroutine(GetConformerID(CID));

            }
        }
    }

    //Returns the (CIDNum)th CID from dataOBJ
    public int parseForCID(JObject dataObj)
    {
        return (int)dataObj["IdentifierList"]["CID"][CIDNum];

    }

    //Uses a CID to get the first conformer ID if one exists
    IEnumerator GetConformerID(int CID)
    {



        //Creates webRequest to find ConformerIDs
        UnityWebRequest www = UnityWebRequest.Get("https://pubchem.ncbi.nlm.nih.gov/rest/pug/compound/cid/" + CID + "/conformers/JSON");


        yield return www.SendWebRequest();

        //If no conformers are found, increment CIDNum, and attempt to find conformers on the (CIDNum)th CID\
        //Note:Still need to properly handle situations where none of the CIDs have conformers, currently increasing CIDNum until it is larger than the number of CIDs, and returning an "Index out of range" error
        if (www.isNetworkError || www.isHttpError)
        {

            Debug.Log(www.error);
            Debug.Log("No valid conformers!");
            CIDNum++;
            if (!isCID)
            {
                StartCoroutine(GetCID(molName));
            }
            else
            {
                 GameObject.FindWithTag("DictationResult").GetComponent<TextMesh>().text = "No conformer for CID";
            }


        }
        //Takes the first conformerID and passes it into GetConformer
        else
        {

            // Show results as text
            Debug.Log(www.downloadHandler.text);




            JObject dataObj = JObject.Parse(www.downloadHandler.text);
            Debug.Log("ConformerID" + dataObj["InformationList"]["Information"][0]["ConformerID"][0]);
            ConformerID = (string)dataObj["InformationList"]["Information"][0]["ConformerID"][0];


            StartCoroutine(GetConformer(ConformerID));
        }
    }
    
    //Gets the JSON conformer file
    IEnumerator GetConformer(string ConformerID)
    {

        //Creates web request to load the JSON conformer via the conformerID
        UnityWebRequest www = UnityWebRequest.Get("https://pubchem.ncbi.nlm.nih.gov/rest/pug/conformers/" + ConformerID + "/JSON");

        yield return www.SendWebRequest();

        //This should only happen with a real network error
        if (www.isNetworkError || www.isHttpError)
        {

            Debug.Log(www.error);
        }


        else
        {

            // Show results as text
            Debug.Log(www.downloadHandler.text);

         

            JObject dataObj = JObject.Parse(www.downloadHandler.text);
            
            //Loads molecule from JObject and instantiates it at the position of the VoiceRecognizer
            molData = loadMolecule(dataObj);
            MoleculeCreator script = gameObject.GetComponent<MoleculeCreator>();
            script.instantiateMolecule(molData, transform.position);
            GameObject.FindWithTag("DictationResult").GetComponent<TextMesh>().text = "Loaded: " + officialMolName;

            //Starts a couroutine to clear the billboard after a number of seconds
            StartCoroutine(ClearText());


        }
    }

    //Returns the first Synonym (Molecule Name) of a CID
    IEnumerator GetMolName(int CID)
    {



        //Creates web request for Synonyms
        UnityWebRequest www = UnityWebRequest.Get("https://pubchem.ncbi.nlm.nih.gov/rest/pug/compound/cid/"+CID+"/synonyms/JSON");


        yield return www.SendWebRequest();


        if (www.isNetworkError || www.isHttpError)
        {

            Debug.Log(www.error);
        }
        else
        {

            // Show results as text
            Debug.Log(www.downloadHandler.text);

            //Parses JObject, takes first Synonym
            JObject dataObj = JObject.Parse(www.downloadHandler.text);
            officialMolName= (string)dataObj["InformationList"]["Information"][0]["Synonym"][0];
            Debug.Log("This is the official molecule name: "+officialMolName);


        }
    }

    //Returns a molecule after parsing through a JObject 
    public MoleculeData loadMolecule(JObject dataObj)
    {
        MoleculeData newMolecule = new MoleculeData();

        

        atoms atm = dataObj["PC_Compounds"][0]["atoms"].ToObject<atoms>();
        bonds bnd = dataObj["PC_Compounds"][0]["bonds"].ToObject<bonds>();
        conformers cnf = dataObj["PC_Compounds"][0]["coords"][0]["conformers"][0].ToObject<conformers>();
        string moleculeName = officialMolName;
        //Debug.Log (atm.element [0]);
        Debug.Log("Loading Molecule");

        newMolecule.atom = atm;
        newMolecule.bond = bnd;
        newMolecule.conf = cnf;
        newMolecule.name = moleculeName;

        
        return newMolecule;
    }

    //Coroutine to remove text from the screen after a number of seconds
    IEnumerator ClearText()
    {
        Debug.Log("Starting text clearer");
        float timeWaited = 0;
        while (timeWaited < 3)
        {
            timeWaited += Time.deltaTime;

            yield return null;
        }
        GameObject.FindWithTag("DictationResult").GetComponent<TextMesh>().text = "";





    }

}
