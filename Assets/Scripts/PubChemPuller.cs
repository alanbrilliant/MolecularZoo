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

    public void startRoutine(String goalMol)
    {
        molName = goalMol;
        CIDNum = 0;
        if (!loading)
        {
            StartCoroutine(GetCID(molName));

        }


    }

    IEnumerator GetCID(String molName)
    {
        isCID = false;
        if  (Regex.IsMatch(molName, @"^\d+$")){
            CID = Int32.Parse(molName);
            StartCoroutine(GetMolName(CID));

            StartCoroutine(GetConformerID(CID));
            isCID = true;

            Debug.Log("CID Entered!");
        }

        string dodec = "2,3a,9a-(Methylidynetrismethylene)-5,3,6a,1,8-(1,2,3,4,5-pentanpentayl)dodecahydro-1H-phenalene";
        string dodec2 = "cubane";

        loading = true;
        Debug.Log("SearchedMol: "+molName);
        UnityWebRequest www = UnityWebRequest.Get("https://pubchem.ncbi.nlm.nih.gov/rest/pug/compound/name/" + molName + "/cids/JSON?name_type=word");
        Debug.Log("Made web request");

        yield return www.SendWebRequest();
        loading = false;
        Debug.Log("Checking for network error");

        if (www.isNetworkError || www.isHttpError)
        {

            if (!isCID) { 

            GameObject.FindWithTag("DictationResult").GetComponent<TextMesh>().text = "No Molecules found!";
            }


            Debug.Log(www.error);
            Debug.Log("No valid molecules!");

        }
        else
        {

            // Show results as text
            Debug.Log(www.downloadHandler.text);

            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;


            JObject dataObj = JObject.Parse(www.downloadHandler.text);
            Debug.Log("CID" + parseForCID(dataObj));
            CID = parseForCID(dataObj);

            StartCoroutine(GetMolName(CID));

            StartCoroutine(GetConformerID(CID));

        }
    }
    public int parseForCID(JObject dataObj)
    {
        return (int)dataObj["IdentifierList"]["CID"][CIDNum];

    }
    IEnumerator GetConformerID(int CID)
    {




        UnityWebRequest www = UnityWebRequest.Get("https://pubchem.ncbi.nlm.nih.gov/rest/pug/compound/cid/" + CID + "/conformers/JSON");


        yield return www.SendWebRequest();


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
            //GameObject.FindWithTag("DictationResult").GetComponent<TextMesh>().text = "No conformers found!";


        }
        else
        {

            // Show results as text
            Debug.Log(www.downloadHandler.text);

            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;


            JObject dataObj = JObject.Parse(www.downloadHandler.text);
            Debug.Log("ConformerID" + dataObj["InformationList"]["Information"][0]["ConformerID"][0]);
            ConformerID = (string)dataObj["InformationList"]["Information"][0]["ConformerID"][0];


            StartCoroutine(GetConformer(ConformerID));
        }
    }

    IEnumerator GetConformer(string ConformerID)
    {




        UnityWebRequest www = UnityWebRequest.Get("https://pubchem.ncbi.nlm.nih.gov/rest/pug/conformers/" + ConformerID + "/JSON");


        yield return www.SendWebRequest();


        if (www.isNetworkError || www.isHttpError)
        {

            Debug.Log(www.error);
        }
        else
        {

            // Show results as text
            Debug.Log(www.downloadHandler.text);

            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;


            JObject dataObj = JObject.Parse(www.downloadHandler.text);
            //Debug.Log("ConformerID" + dataObj["InformationList"]["Information"][0]["ConformerID"][0]);
            //ConformerID = (string)dataObj["InformationList"]["Information"][0]["ConformerID"][0];
            molData = loadMolecule(dataObj);
            MoleculeCreator script = gameObject.GetComponent<MoleculeCreator>();
            script.instantiateMolecule(molData, transform.position);
            GameObject.FindWithTag("DictationResult").GetComponent<TextMesh>().text = "Loaded: " + officialMolName;
            StartCoroutine(ClearText());


        }
    }

    IEnumerator GetMolName(int CID)
    {




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

            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;


            JObject dataObj = JObject.Parse(www.downloadHandler.text);
            officialMolName= (string)dataObj["InformationList"]["Information"][0]["Synonym"][0];
            Debug.Log("This is the official molecule name: "+officialMolName);


        }
    }


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

        /*
        Debug.Log("Loading Molecule2");
        Debug.Log(newMolecule.atom);
        Debug.Log(newMolecule.bond);
        Debug.Log(newMolecule.conf);
        Debug.Log(newMolecule.name);
        */
        return newMolecule;
    }
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
