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
using System.Linq;

public class VoiceRecog : MonoBehaviour {

    // Use this for initialization
    [SerializeField]
    private List<string> ListKeywords = new List<string>();

    private KeywordRecognizer L_Recognizer;
    public DictationRecognizer D_Recognizer;

    private GameObject gameController;
    MoleculeData molData = new MoleculeData();

    private int defaultFontSize = 100;

    public float spawnDist;
    public string searchedMol;
    public string searchText;
    private DataManager script1;


    private List<string> toolKeyWords = new List<string>();
    private Dictionary<string, GameObject> atomPrefabKeywords;
    private List<string> moleculeKeywords;
    


    void Start () {
        MoleculeCreator spawnScript = gameObject.GetComponent<MoleculeCreator>();

        toolKeyWords.Add("Hand");
        Gun[] allTools = Resources.FindObjectsOfTypeAll<Gun>();
        foreach (Gun tool in allTools)
        {
            if (!toolKeyWords.Contains(tool.gameObject.name))
                toolKeyWords.Add(tool.gameObject.name);
        }        //Tool Switchin

        foreach (string a in toolKeyWords)
            Debug.Log(a);


        //Atom Spawning

        atomPrefabKeywords = new Dictionary<string, GameObject> {
            { "Hydrogen" , spawnScript.hydrogenPrefab },
            {"Lithium", spawnScript.lithiumPrefab },
            {"Carbon", spawnScript.carbonPrefab },
            {"Nitrogen", spawnScript.nitrogenPrefab },
            {"Oxygen", spawnScript.oxygenPrefab },
            {"Fluorine", spawnScript.fluorinePrefab },
            {"Chlorine", spawnScript.chlorinePrefab },
            {"Sodium", spawnScript.sodiumPrefab },
            {"Phosphorus", spawnScript.phosphorusPrefab },
            {"Sulfur", spawnScript.sulfurPrefab },
            {"Iron", spawnScript.ironPrefab },
            {"Aluminum", spawnScript.aluminiumPrefab }
        };

        //Preloaded Molecule Spawning
        moleculeKeywords = new List<string> { "ATP", "Water", "CarbonDioxide", "Caffeine", "Aspirin", "SulfuricAcid","SaturatedFat"};

        ListKeywords = ListKeywords.Concat(toolKeyWords).ToList();
        ListKeywords = ListKeywords.Concat(atomPrefabKeywords.Keys).ToList();
        ListKeywords = ListKeywords.Concat(moleculeKeywords).ToList();
        ListKeywords.Add("Reset");        //Game Reset
        ListKeywords.Add("Create");         //Pubchem Molecule Spawning
        ListKeywords.Add("BondCount");        //Count of number of Bonds/Atoms (Not precise yet, but provides rough estimate)
        ListKeywords.Add("AtomCount");
        ListKeywords.Add("help");        //Starts a Demo of how to use voice control
        ListKeywords.Add("Blackhole");         //Creates a "black hole." Pulls all atoms to a single point






        L_Recognizer = new KeywordRecognizer(ListKeywords.ToArray());
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


        ProcessRecognizedPhrase(args.text);
    }

    public void ProcessRecognizedPhrase(string phrase) {
        //Debug.Log(args.text);

        GameObject.FindWithTag("DictationResult").GetComponent<TextMesh>().fontSize = defaultFontSize;

        //Resets game
        if (phrase == "Reset")
        {
            L_Recognizer.Stop();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        }


        //Changes tool
        if (toolKeyWords.Contains(phrase)) {
            GameObject rightWand = GameObject.FindWithTag("RightWand");
            GameObject leftWand = GameObject.FindWithTag("LeftWand");
            GameObject higherWand = null;
            higherWand = rightWand == null ? leftWand : leftWand == null ? rightWand
                : rightWand.transform.position.y > leftWand.transform.position.y ? rightWand : leftWand;
            if (higherWand == null)
                Debug.LogError("Could not determine which wand has the higher y value");
            higherWand.GetComponent<Wand>().setToolByName(phrase);
         }




        //Spawns atoms
        if (atomPrefabKeywords.ContainsKey(phrase))
        {
            Instantiate(atomPrefabKeywords[phrase], transform.position, transform.rotation);
        }


        //Spawns preloaded molecules
        if (moleculeKeywords.Contains(phrase))
        {
            setMoleculeToSpawn(phrase);
            MoleculeCreator script = gameObject.GetComponent<MoleculeCreator>();
            script.instantiateMolecule(molData, transform.position);


            //Spawn Molecule
            Debug.Log(phrase + " should be spawned!");
        }

        //Spawns Molecules via Pubchem
        if (phrase == "Create")
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
        if (phrase == "BondCount")
        {
            //Gets array of objects tagged with "bond" and "doublebond", returns the sum of their length
            Array getCountSingleBond = GameObject.FindGameObjectsWithTag("Bond");
            Array getCountDoubleBond = GameObject.FindGameObjectsWithTag("DoubleBond");
            int count = getCountSingleBond.Length + getCountDoubleBond.Length * 2;
            GameObject.FindWithTag("DictationResult").GetComponent<TextMesh>().text = "Bonds: " + count;


            //Updates billboard position
            GameObject.FindWithTag("DictationResult").transform.position = GameObject.FindWithTag("DictationPosition").transform.position;
            float a = Mathf.Atan2(GameObject.FindWithTag("DictationPosition").transform.position.x, GameObject.FindWithTag("DictationPosition").transform.position.z) * Mathf.Rad2Deg;
            GameObject.FindWithTag("DictationResult").transform.rotation = Quaternion.AngleAxis(a, Vector3.up);
        }
        if (phrase == "AtomCount")
        {
            //Gets array of objects tagged with "atom", and returns its length
            Array getCount = GameObject.FindGameObjectsWithTag("Atom");
            int count = getCount.Length;
            GameObject.FindWithTag("DictationResult").GetComponent<TextMesh>().text = "Atoms: " + count;


            //Updates billboard position 
            GameObject.FindWithTag("DictationResult").transform.position = GameObject.FindWithTag("DictationPosition").transform.position;
            float a = Mathf.Atan2(GameObject.FindWithTag("DictationPosition").transform.position.x, GameObject.FindWithTag("DictationPosition").transform.position.z) * Mathf.Rad2Deg;
            GameObject.FindWithTag("DictationResult").transform.rotation = Quaternion.AngleAxis(a, Vector3.up);

        }
        if (phrase == "help")
        {
            GameObject.FindWithTag("DictationResult").GetComponent<TextMesh>().fontSize = 50;
            GameObject.FindWithTag("DictationResult").GetComponent<TextMesh>().text =
                "Voice Commands:\n\n ATP, CAFFEINE, SATURATED FAT         Create molecule\n   WATER, CARBON DIOXIDE,\n   SULFURUC ACID, ASPIRIN,       \n CREATE + [Name of Molecule]          Fetch pubchem molecules\n[Element Name]                                   Create atom\n HAND, TRACTOR, PISTOL,            Change what you hold\n       BLASTER, CARDS     \n BLACKHOLE\n RESET                                               Reset game\n";


            GameObject.FindWithTag("DictationResult").transform.position = GameObject.FindWithTag("DictationPosition").transform.position;
            float a = Mathf.Atan2(GameObject.FindWithTag("DictationPosition").transform.position.x, GameObject.FindWithTag("DictationPosition").transform.position.z) * Mathf.Rad2Deg;
             GameObject.FindWithTag("DictationResult").transform.rotation = Quaternion.AngleAxis(a, Vector3.up);
            //GameObject.FindWithTag("DictationResult").transform.rotation = GameObject.FindWithTag("DictationPosition").transform.rotation;
        }
        if (phrase == "Blackhole")
        {
            GameObject.Find("Blackhole").GetComponent<BlackHole>().createBlackHole();
        }
    }

    public void DictationRecognizer_DictationComplete(DictationCompletionCause cause)
    {

        //Shuts off dictator and restarts keyword recognizer
        Debug.Log("Dictation Timeout");
        D_Recognizer.Dispose();
        PhraseRecognitionSystem.Restart();


    }
    public void DictationRecognizer_DictationResult(string text, ConfidenceLevel confidence)
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
