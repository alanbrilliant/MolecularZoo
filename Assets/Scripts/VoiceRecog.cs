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

    public float spawnDist;
    public string searchedMol;
    public string searchText;
    private DataManager script1;


    private List<string> toolKeyWords = new List<string>();
    private Dictionary<string, GameObject> atomPrefabKeywords;
    private List<string> moleculeKeywords;
    


    void Awake() {
        MoleculeCreator spawnScript = gameObject.GetComponent<MoleculeCreator>();

        toolKeyWords.Add("Hand");
        Gun[] allTools = Resources.FindObjectsOfTypeAll<Gun>();
        foreach (Gun tool in allTools)
        {
            if (/*tool.gameObject.activeSelf &&*/tool.voiceName != "" && !toolKeyWords.Contains(tool.voiceName))
	    {
                Debug.Log("hello " + tool.voiceName);
                toolKeyWords.Add(tool.voiceName);
                
	    }
        }        //Tool Switchin


		Debug.Log("Speech tool list " + String.Join(", ", toolKeyWords.ToArray()));


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
            higherWand.GetComponent<Wand>().setToolByVoiceName(phrase);
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
        }

        //Spawns Molecules via Pubchem
        if (phrase == "Create")
        {
            //Stops keyword recognizer (needed in order to start dictator)
            PhraseRecognitionSystem.Shutdown();
            D_Recognizer = new DictationRecognizer();

            //Updates position of "billboard" and updates its status to "Listening"
	    ShowOnBillboard("Listening");

            D_Recognizer.DictationResult += DictationRecognizer_DictationResult;
            D_Recognizer.DictationComplete += DictationRecognizer_DictationComplete;
	    D_Recognizer.DictationError += DictationRecognizer_DictationError;

	    //Starts dictation recognizer
            D_Recognizer.Start();


        }
        if (phrase == "BondCount")
        {
            //Gets array of objects tagged with "bond" and "doublebond", returns the sum of their length
            Array getCountSingleBond = GameObject.FindGameObjectsWithTag("Bond");
            Array getCountDoubleBond = GameObject.FindGameObjectsWithTag("DoubleBond");
            int count = getCountSingleBond.Length + getCountDoubleBond.Length * 2;
	    ShowOnBillboard("Bonds: " + count);
        }
        if (phrase == "AtomCount")
        {
            //Gets array of objects tagged with "atom", and returns its length
            Array getCount = GameObject.FindGameObjectsWithTag("Atom");
            int count = getCount.Length;
            ShowOnBillboard("Atoms: " + count);
        }
        if (phrase == "help")
        {
	    string help = "Voice Commands:\n\n ATP, CAFFEINE, SATURATED FAT         Create molecule\n   WATER, CARBON DIOXIDE,\n   SULFURIC ACID, ASPIRIN,       \n CREATE + [Name of Molecule]          Fetch pubchem molecules\n[Element Name]                                   Create atom\n HAND, TRACTOR, PISTOL,            Change what you hold\n       BLASTER, CARDS     \n BLACKHOLE\n RESET                                               Reset game\n";
	    ShowOnBillboard(help, 50);
        }
        if (phrase == "Blackhole")
        {
            GameObject.Find("Blackhole").GetComponent<BlackHole>().createBlackHole();
        }
    }

    private void ShowOnBillboard(string text, int font_size = 100)
    {
	GameObject billboard = GameObject.FindWithTag("DictationResult");
	TextMesh textm = billboard.GetComponent<TextMesh>();
	textm.fontSize = font_size;
	textm.text = text;

	// Position billboard in current view direction rotated to face viewer.
	Vector3 gaze_point = GameObject.FindWithTag("DictationPosition").transform.position;
        billboard.transform.position = gaze_point;
        float a = Mathf.Atan2(gaze_point.x, gaze_point.z) * Mathf.Rad2Deg;
        billboard.transform.rotation = Quaternion.AngleAxis(a, Vector3.up);
    }

    public void DictationRecognizer_DictationError(string error, int hresult)
    {
	ShowOnBillboard(WrapText(error, 40));
	D_Recognizer.Dispose();
	// For some reason phrase recognition is broken
	// after dictation error unless we restart it.
	PhraseRecognitionSystem.Restart();
    }

    public string WrapText(string text, int wrap_length)
    {
	if (text.Length <= wrap_length)
	    return text;
	else
	{
	    int i = text.IndexOf(" ", wrap_length, text.Length-wrap_length);
	    if (i == -1)
	        return text;
	    else
		return String.Concat(text.Substring(0,i), "\n", WrapText(text.Substring(i),wrap_length));
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
