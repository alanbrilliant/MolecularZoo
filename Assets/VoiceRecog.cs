using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.SceneManagement;
using System.Net;
using System;
using System.IO;
using System.Threading;
using UnityEngine.Networking;

public class VoiceRecog : MonoBehaviour {

    // Use this for initialization
    [SerializeField]
    private string[] ListKeywords;

    private KeywordRecognizer L_Recognizer;
    private DictationRecognizer D_Recognizer;

    private GameObject gameController;
    MoleculeData molData = new MoleculeData();

    public float spawnDist;
    public string searchedMol;
    public string searchText;
    private DataManager script1;



    void Start () {

        ListKeywords = new string[33];
        ListKeywords[0] = "Reset";
        //Tool Switching
        ListKeywords[1] = "RightTractor";
        ListKeywords[2] = "RightGun";
        ListKeywords[3] = "RightHeavyGun";
        ListKeywords[4] = "RightCards";
        ListKeywords[5] = "RightHand";
        ListKeywords[6] = "LeftTractor";
        ListKeywords[7] = "LeftGun";
        ListKeywords[8] = "LeftHeavyGun";
        ListKeywords[9] = "LeftCards";
        ListKeywords[10] = "LeftHand";

        //Atom Spawning
        ListKeywords[11] = "Hydrogen";
        ListKeywords[12] = "Oxygen";
        ListKeywords[13] = "Carbon";
        ListKeywords[14] = "Nitrogen";
        ListKeywords[15] = "Phosphorus";
        ListKeywords[16] = "Sulfur";
        ListKeywords[17] = "Chlorine";
        ListKeywords[18] = "Fluorine";
        ListKeywords[19] = "Iron";
        ListKeywords[20] = "Sodium";
        ListKeywords[21] = "Lithium";
        ListKeywords[22] = "Aluminium";

        //Molecule Spawning
        ListKeywords[23] = "Water";
        ListKeywords[24] = "CarbonDioxide";
        ListKeywords[25] = "ATP"; 
        ListKeywords[26] = "Aspirin";
        ListKeywords[27] = "Caffeine";
        ListKeywords[28] = "SaturatedFat";
        ListKeywords[29] = "SulfuricAcid";
        ListKeywords[30] = "Create";
        ListKeywords[31] = "BondCount";
        ListKeywords[32] = "AtomCount";




        L_Recognizer = new KeywordRecognizer(ListKeywords);
        L_Recognizer.OnPhraseRecognized += OnPhraseRecognized;
        L_Recognizer.Start();
        //StartCoroutine(GetText());

        Debug.Log("Speech recognition initialized!");


    }

    public void setMoleculeToSpawn(string molName)
    {
        
        gameController = GameObject.FindWithTag("GameController");
        script1 = gameController.GetComponent<DataManager>();
        molData = script1.loadMolecule(molName.ToLower() + "data.json", molName);
        

    }
    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {

        Debug.Log(args.text);
        
        //Resets game
        if (args.text== ListKeywords[0])
        {
            L_Recognizer.Stop();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            
        }

        for (int i = 1; i < 6; i++)
        {

            Debug.Log("This should update the controller state!"+args.text);
            if (args.text == ListKeywords[i])
            {

                GameObject.FindWithTag("RightWand").GetComponent<Wand>().controllerState = i-1;
                GameObject.FindWithTag("RightWand").GetComponent<Wand>().updateControllerState();

            }

            //GameObject.FindWithTag("RightWand").GetComponent<Wand>().updateControllerState();

        }


        for (int i = 6; i < 11; i++)
        {
            Debug.Log("This should update the controller state!");
            if (args.text == ListKeywords[i])
            {

                GameObject.FindWithTag("LeftWand").GetComponent<Wand>().controllerState = i-6;
                GameObject.FindWithTag("LeftWand").GetComponent<Wand>().updateControllerState();


            }
        }


        
            Debug.Log("This should spawn an atom!");
            //Spawn Atom
            MoleculeCreator spawnScript = gameObject.GetComponent<MoleculeCreator>();
            if (args.text == "Hydrogen")
            {
                //GetComponent<PubChemPuller>().startRoutine("t");

            GameObject newAtom = Instantiate(spawnScript.hydrogenPrefab, transform.position, transform.rotation);
            }
            if (args.text == "Oxygen")
            {
                GameObject newAtom = Instantiate(spawnScript.oxygenPrefab, transform.position, transform.rotation);
            }
            if (args.text == "Carbon")
            {
                GameObject newAtom = Instantiate(spawnScript.carbonPrefab, transform.position, transform.rotation);
            }
            if (args.text == "Nitrogen")
            {
                GameObject newAtom = Instantiate(spawnScript.nitrogenPrefab, transform.position, transform.rotation);
            }
            if (args.text == "Phosphorus")
            {
                GameObject newAtom = Instantiate(spawnScript.phosphorusPrefab, transform.position, transform.rotation);
            }
            if (args.text == "Iron")
            {
                GameObject newAtom = Instantiate(spawnScript.ironPrefab, transform.position, transform.rotation);
            }
            if (args.text == "Sodium")
            {
                GameObject newAtom = Instantiate(spawnScript.sodiumPrefab, transform.position, transform.rotation);
            }
            if (args.text == "Lithium")
            {
                GameObject newAtom = Instantiate(spawnScript.lithiumPrefab, transform.position, transform.rotation);
            }
            if (args.text == "Chlorine")
            {
                GameObject newAtom = Instantiate(spawnScript.chlorinePrefab, transform.position, transform.rotation);
            }
            if (args.text == "Fluorine")
            {
                GameObject newAtom = Instantiate(spawnScript.fluorinePrefab, transform.position, transform.rotation);
            }
            if (args.text == "Aluminium")
            {
                GameObject newAtom = Instantiate(spawnScript.aluminiumPrefab, transform.position, transform.rotation);
            }







        for (int i=23; i<30;  i++)
        {
            if (args.text == ListKeywords[i])
            {



                setMoleculeToSpawn(args.text);
                MoleculeCreator script = gameObject.GetComponent<MoleculeCreator>();
                script.instantiateMolecule(molData, transform.position);


                //Spawn Molecule
                Debug.Log(args.text+" should be spawned!");
            }
        }
        if (args.text == ListKeywords[30])
        {
            Debug.Log("Stopping the keyword recognizer!");
            PhraseRecognitionSystem.Shutdown();
            D_Recognizer = new DictationRecognizer();


            GameObject.FindWithTag("DictationResult").GetComponent<TextMesh>().text = "Listening";



            GameObject.FindWithTag("DictationResult").transform.position = GameObject.FindWithTag("DictationPosition").transform.position;

            
            float a = Mathf.Atan2(GameObject.FindWithTag("DictationPosition").transform.position.x, GameObject.FindWithTag("DictationPosition").transform.position.z) * Mathf.Rad2Deg;
            Debug.Log("Transform position x"+ GameObject.FindWithTag("DictationPosition").transform.position.x + " Transform position z"+ GameObject.FindWithTag("DictationPosition").transform.position.z);
            Debug.Log("Angle: "+a);
            GameObject.FindWithTag("DictationResult").transform.rotation = Quaternion.AngleAxis(a, Vector3.up);
            


            Debug.Log("1");

            D_Recognizer.DictationResult += DictationRecognizer_DictationResult;
            D_Recognizer.DictationComplete += DictationRecognizer_DictationComplete;
            Debug.Log("2");

            D_Recognizer.Start();
            Debug.Log("3");

        }
        if (args.text == ListKeywords[31])
        {
            Array getCount = GameObject.FindGameObjectsWithTag("Bond");
            int count = getCount.Length;
            GameObject.FindWithTag("DictationResult").GetComponent<TextMesh>().text = "Bonds: "+count;



            GameObject.FindWithTag("DictationResult").transform.position = GameObject.FindWithTag("DictationPosition").transform.position;
            float a = Mathf.Atan2(GameObject.FindWithTag("DictationPosition").transform.position.x, GameObject.FindWithTag("DictationPosition").transform.position.z) * Mathf.Rad2Deg;
            GameObject.FindWithTag("DictationResult").transform.rotation = Quaternion.AngleAxis(a, Vector3.up);
        }
        if (args.text == ListKeywords[32])
        {


            Array getCount = GameObject.FindGameObjectsWithTag("Atom");
            int count = getCount.Length;

            GameObject.FindWithTag("DictationResult").GetComponent<TextMesh>().text = "Atoms: "+count;



            GameObject.FindWithTag("DictationResult").transform.position = GameObject.FindWithTag("DictationPosition").transform.position;
            float a = Mathf.Atan2(GameObject.FindWithTag("DictationPosition").transform.position.x, GameObject.FindWithTag("DictationPosition").transform.position.z) * Mathf.Rad2Deg;
            GameObject.FindWithTag("DictationResult").transform.rotation = Quaternion.AngleAxis(a, Vector3.up);

        }





    }
    private void DictationRecognizer_DictationComplete(DictationCompletionCause cause)
    {
        Debug.Log("Dictation Timeout");

        D_Recognizer.Dispose();
        PhraseRecognitionSystem.Restart();
        //L_Recognizer.Start();
    }
    private void DictationRecognizer_DictationResult(string text, ConfidenceLevel confidence)
    {
        Debug.Log("Create: "+text);
        
        D_Recognizer.Stop();

        GameObject.FindWithTag("DictationResult").GetComponent<TextMesh>().text = "Searching: "+text;
        
        Vector3 temp = new Vector3(10.0f,0, 0);
        /*
        GameObject.FindWithTag("DictationResult").transform.position = GameObject.FindWithTag("DictationPosition").transform.position;
        float a =Mathf.Atan2(transform.position.x, transform.position.z) *Mathf.Rad2Deg;
        GameObject.FindWithTag("DictationResult").transform.rotation = Quaternion.AngleAxis(a, Vector3.up);
        */




        searchedMol = text.Replace(" ", "_");
        searchText = "https://pubchem.ncbi.nlm.nih.gov/rest/pug/compound/name/" + searchedMol + "/cids/JSON?name_type=word";
        Debug.Log("Starting Coroutine");
        GetComponent<PubChemPuller>().startRoutine(searchedMol);

        /*
        MoleculeCreator script = gameObject.GetComponent<MoleculeCreator>();
        script.instantiateMolecule(GetComponent<PubChemPuller>().molData, transform.position);
        */
        //GetComponent<PubChemPuller>().startRoutine(searchText);


        //StartCoroutine(GetText());
        //https://pubchem.ncbi.nlm.nih.gov/rest/pug/compound/name/dihydrogen_monoxide/cids/XML?name_type=word


        //D_Recognizer.Dispose();
        //PhraseRecognitionSystem.Restart();

        //GetComponent<PubChemPuller>().startRoutine("t");

    }




    /*
     IEnumerator GetText()
 {
         Debug.Log("SearchedMol: ");
     UnityWebRequest www = UnityWebRequest.Get(searchText);
         Debug.Log("Made web request");

         yield return www.SendWebRequest();

         Debug.Log("Checking for network error");

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
     }
 }*/

 /*
    IEnumerator GetText()
    {
        using (UnityWebRequest req = UnityWebRequest.Get("https://pubchem.ncbi.nlm.nih.gov/rest/pug/compound/name/dihydrogen_monoxide/cids/XML?name_type=word"))
        {
            yield return req.SendWebRequest();
            while (!req.isDone)
                yield return null;
            Debug.Log(req.downloadHandler.text);
            byte[] result = req.downloadHandler.data;
            string molCidJSON = System.Text.Encoding.Default.GetString(result);
            
        }
    }*/



}
