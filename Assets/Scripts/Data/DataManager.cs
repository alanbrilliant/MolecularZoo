using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine;
using FullSerializer;
using Newtonsoft.Json.Linq;


public class DataManager : MonoBehaviour {

	MoleculeData waterData = new MoleculeData ();
	MoleculeData atpData = new MoleculeData();
	MoleculeData aspirinData = new MoleculeData ();

	private  string gameDataFileName = "atpdata.json";

	// Use this for initialization
	void Start () {
		aspirinData = loadMolecule ("aspirindata.json", "aspirin");
		waterData = loadMolecule ("waterdata.json", "water");
		atpData = loadMolecule ("atpdata.json", "atp");
		/*atoms waterAtoms = new atoms(new List<int>{8,1,1});
		bonds waterBonds = new bonds (new List<int> {1, 1}, new List<int> {2, 3}, new List<int> {1, 1});
		coords waterCoords = new coords (new conformers(new List<float> {0,0.2774f,0.6068f}, new List<float> {0,0.8929f,-0.2383f}, new List<float> {0,0.2544f,-0.7169f}));
		MoleculeData water = new MoleculeData( waterAtoms,waterBonds,waterCoords, "water");*/
		MoleculeCreator script = gameObject.GetComponent<MoleculeCreator> ();
		//Debug.Log(waterData.atom.element[0]);
		//Debug.Log(waterData.conf.x[1]);
		for (int i = 0; i < 5; i++) {
			script.instantiateMolecule (aspirinData, new Vector3(Random.Range(1f,4f),Random.Range(-3f,3f), Random.Range(-5f,3f)));
		}
		for (int i = 0; i < 50; i++) {
			script.instantiateMolecule (waterData, new Vector3(Random.Range(-3f,3f),Random.Range(1f,4f), Random.Range(-5f,3f)));
		}

		script.instantiateMolecule (atpData, new Vector3(Random.Range(1f,4f),Random.Range(-3f,3f), Random.Range(-5f,3f)));






	}
	
	// Update is called once per frame
	void Update () {
		
	}



	public MoleculeData loadMolecule(string fileName, string moleculeName){
		MoleculeData newMolecule = new MoleculeData ();

		JObject dataObj = LoadGameData (fileName);

		atoms atm = dataObj ["PC_Compounds"] [0]["atoms"].ToObject<atoms>() ;
		bonds bnd = dataObj ["PC_Compounds"] [0]["bonds"].ToObject<bonds>();
		conformers cnf =  dataObj ["PC_Compounds"] [0]["coords"][0]["conformers"][0].ToObject<conformers>();
		//Debug.Log (atm.element [0]);


		newMolecule.atom = atm;
		newMolecule.bond = bnd;
		newMolecule.conf = cnf;
		newMolecule.name = moleculeName;
		return newMolecule;
	
	}

	public JObject LoadGameData(string fileName){
		string filePath = Path.Combine (Application.streamingAssetsPath, fileName);

		if (File.Exists(filePath)){
			string dataAsJson = File.ReadAllText (filePath);
			JObject dataObj = JObject.Parse (dataAsJson);
			return dataObj;
			//IList<JToken> partsList = new IList<JToken> ();
			//partsList.Add (dataObj ["PC_Compounds"] [0] ["atoms"]);
			//partsList.Add (dataObj ["PC_Compounds"] [0] ["bonds"]);

			//JToken tok = dataObj ["PC_Compounds"] [0] ["atoms"];






			//JsonConvert.PopulateObject(dataAsJson, waterData);

		
		} else {
			Debug.LogError ("Data file does not exist!");
			return null;
		
		
		}


	}
}



