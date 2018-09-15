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

        ListKeywords = new string[35];

        //Game Reset
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

        //Preloaded Molecule Spawning
        ListKeywords[23] = "Water";
        ListKeywords[24] = "CarbonDioxide";
        ListKeywords[25] = "ATP"; 
        ListKeywords[26] = "Aspirin";
        ListKeywords[27] = "Caffeine";
        ListKeywords[28] = "SaturatedFat";
        ListKeywords[29] = "SulfuricAcid";
        //Pubchem Molecule Spawning
        ListKeywords[30] = "Create";

        //Count of number of Bonds/Atoms (Not precise yet, but provides rough estimate)
        ListKeywords[31] = "BondCount";
        ListKeywords[32] = "AtomCount";

        //Starts a Demo of how to use voice control
        ListKeywords[33] = "Help";

        //Creates a "black hole." Pulls all atoms to a single point
        ListKeywords[34] = "BlackHole";



        L_Recognizer = new KeywordRecognizer(ListKeywords);
        L_Recognizer.OnPhraseRecognized += OnPhraseRecognized;
        L_Recognizer.Start();
        //StartCoroutine(GetText());

        Debug.Log("Speech recognition initialized!");


    }

    //Takes a string with the molecule name, and loads that molecule to molData
    public void setMoleculeToSpawn(string molName)
    {
        
        gameController = GameObject.FindWithTag("GameController");
        script1 = gameController.GetComponent<DataManager>();
        molData = script1.loadMolecule(molName.ToLower() + "data.json", molName);
        

    }

    //Triggered when a phrase (word contained in ListKeywords) is recognized
    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {

        //Debug.Log(args.text);
        
        //Resets game
        if (args.text== ListKeywords[0])
        {
            L_Recognizer.Stop();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            
        }


        //Updates right controller state
        for (int i = 1; i < 6; i++)
        {

            if (args.text == ListKeywords[i])
            {

                GameObject.FindWithTag("RightWand").GetComponent<Wand>().controllerState = i-1;
                GameObject.FindWithTag("RightWand").GetComponent<Wand>().updateControllerState();

            }


        }

        //Updates left contoller state
        for (int i = 6; i < 11; i++)
        {
            Debug.Log("This should update the controller state!");
            if (args.text == ListKeywords[i])
            {

                GameObject.FindWithTag("LeftWand").GetComponent<Wand>().controllerState = i-6;
                GameObject.FindWithTag("LeftWand").GetComponent<Wand>().updateControllerState();


            }
        }


        
            //Spawns Atoms
            MoleculeCreator spawnScript = gameObject.GetComponent<MoleculeCreator>();
            if (args.text == "Hydrogen")
            {

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






        //Spawns preloaded molecules
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


        //Spawns Molecules via Pubchem
        if (args.text == ListKeywords[30])
        {
            //Stops keyword recognizer (needed in order to start dictator)
            PhraseRecognitionSystem.Shutdown();
            D_Recognizer = new DictationRecognizer();

            //Updates position of "billboard" and updates its status to "Listening" 
            GameObject.FindWithTag("DictationResult").GetComponent<TextMesh>().text = "Listening";
            GameObject.FindWithTag("DictationResult").transform.position = GameObject.FindWithTag("DictationPosition").transform.position;
            float a = Mathf.Atan2(GameObject.FindWithTag("DictationPosition").transform.position.x, GameObject.FindWithTag("DictationPosition").transform.position.z) * Mathf.Rad2Deg;
            GameObject.FindWithTag("DictationResult").transform.rotation = Quaternion.AngleAxis(a, Vector3.up);
            


            
            D_Recognizer.DictationResult += DictationRecognizer_DictationResult;
            D_Recognizer.DictationComplete += DictationRecognizer_DictationComplete;
           
            //Starts dictation recognizer
            D_Recognizer.Start();
            

        }
        if (args.text == ListKeywords[31])
        {
            //Gets array of objects tagged with "bond" and "doublebond", returns the sum of their length
            Array getCountSingleBond = GameObject.FindGameObjectsWithTag("Bond");
            Array getCountDoubleBond = GameObject.FindGameObjectsWithTag("DoubleBond");
            int count = getCountSingleBond.Length+getCountDoubleBond.Length*2;
            GameObject.FindWithTag("DictationResult").GetComponent<TextMesh>().text = "Bonds: "+count;


            //Updates billboard position
            GameObject.FindWithTag("DictationResult").transform.position = GameObject.FindWithTag("DictationPosition").transform.position;
            float a = Mathf.Atan2(GameObject.FindWithTag("DictationPosition").transform.position.x, GameObject.FindWithTag("DictationPosition").transform.position.z) * Mathf.Rad2Deg;
            GameObject.FindWithTag("DictationResult").transform.rotation = Quaternion.AngleAxis(a, Vector3.up);
        }
        if (args.text == ListKeywords[32])
        {
            //Gets array of objects tagged with "atom", and returns its length
            Array getCount = GameObject.FindGameObjectsWithTag("Atom");
            int count = getCount.Length;
            GameObject.FindWithTag("DictationResult").GetComponent<TextMesh>().text = "Atoms: "+count;


            //Updates billboard position 
            GameObject.FindWithTag("DictationResult").transform.position = GameObject.FindWithTag("DictationPosition").transform.position;
            float a = Mathf.Atan2(GameObject.FindWithTag("DictationPosition").transform.position.x, GameObject.FindWithTag("DictationPosition").transform.position.z) * Mathf.Rad2Deg;
            GameObject.FindWithTag("DictationResult").transform.rotation = Quaternion.AngleAxis(a, Vector3.up);

        }
        if (args.text == ListKeywords[33])
        {
            GameObject.FindWithTag("DictationResult").GetComponent<TextMesh>().text = "Commands: AtomCount, BondCount, Reset\n [Name of Atom], [Name of Molecule]\n Create +[Name of Molecule]\n Right +[Name of tool], Left+ [Name of tool]";


            GameObject.FindWithTag("DictationResult").transform.position = GameObject.FindWithTag("DictationPosition").transform.position;
            float a = Mathf.Atan2(GameObject.FindWithTag("DictationPosition").transform.position.x, GameObject.FindWithTag("DictationPosition").transform.position.z) * Mathf.Rad2Deg;
            GameObject.FindWithTag("DictationResult").transform.rotation = Quaternion.AngleAxis(a, Vector3.up);
        }
        if (args.text == ListKeywords[34])
        {
            GameObject[] atoms = GameObject.FindGameObjectsWithTag ("Atom");
            foreach(GameObject atom in atoms)
            {
                atom.GetComponent<Rigidbody>().velocity = Vector3.zero - atom.transform.position.normalized * 10f;
            }
        }




    }
    private void DictationRecognizer_DictationComplete(DictationCompletionCause cause)
    {

        //Shuts off dictator and restarts keyword recognizer
        Debug.Log("Dictation Timeout");
        D_Recognizer.Dispose();
        PhraseRecognitionSystem.Restart();


    }
    private void DictationRecognizer_DictationResult(string text, ConfidenceLevel confidence)
    {


        Debug.Log("Create: "+text);

        //Stops dictation recognizer by calling DictationRecognizer_DictationComplete()
        D_Recognizer.Stop();

        //Updates "Billboard" text to searching
        GameObject.FindWithTag("DictationResult").GetComponent<TextMesh>().text = "Searching: "+text;
        



        searchedMol = text.Replace(" ", "_");
        searchText = "https://pubchem.ncbi.nlm.nih.gov/rest/pug/compound/name/" + searchedMol + "/cids/JSON?name_type=word";
        Debug.Log("Starting Coroutine");
        GetComponent<PubChemPuller>().startRoutine(searchedMol);
        
    }

}
