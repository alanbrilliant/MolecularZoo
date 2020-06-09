﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine;
using FullSerializer;
using Newtonsoft.Json.Linq;


public class DataManager : MonoBehaviour {
    public float spawnDist;
	MoleculeData waterData = new MoleculeData ();
	MoleculeData atpData = new MoleculeData();
	MoleculeData aspirinData = new MoleculeData ();
	MoleculeData caffeineData = new MoleculeData();
	MoleculeData carbondioxideData = new MoleculeData();
	MoleculeData saturatedfatData = new MoleculeData();
	MoleculeData sulfuricacidData = new MoleculeData();
    MoleculeData nitroData = new MoleculeData();

	private  string gameDataFileName = "atpdata.json";

	// Use this for initialization
	void Start() {
		aspirinData = loadMolecule ("aspirindata.json", "Aspirin");
		waterData = loadMolecule ("waterdata.json", "Water");
		atpData = loadMolecule ("atpdata.json", "ATP");
		caffeineData = loadMolecule ("caffeinedata.json", "Caffeine");
		carbondioxideData = loadMolecule ("carbondioxidedata.json", "CarbonDioxide");
	    saturatedfatData = loadMolecule ("saturatedfatdata.json", "SaturatedFat");
		sulfuricacidData = loadMolecule ("sulfuricaciddata.json", "SulfuricAcid");
        nitroData = loadMolecule("nitrodata.json", "Nitroglycerin");
		/*atoms waterAtoms = new atoms(new List<int>{8,1,1});
		bonds waterBonds = new bonds (new List<int> {1, 1}, new List<int> {2, 3}, new List<int> {1, 1});
		coords waterCoords = new coords (new conformers(new List<float> {0,0.2774f,0.6068f}, new List<float> {0,0.8929f,-0.2383f}, new List<float> {0,0.2544f,-0.7169f}));
		MoleculeData water = new MoleculeData( waterAtoms,waterBonds,waterCoords, "water");*/
		MoleculeCreator script = gameObject.GetComponent<MoleculeCreator> ();
        //Debug.Log(waterData.atom.element[0]);
        //Debug.Log(waterData.conf.x[1]);
        
		for (int i = 0; i < 3; i++) {
			script.instantiateMolecule (aspirinData, new Vector3(Random.Range(-3f,4f),Random.Range(2f,4f), Random.Range(-4f,4f))*spawnDist);
		}
		for (int i = 0; i < 50; i++) {
			script.instantiateMolecule (waterData, new Vector3(Random.Range(-3f,4f),Random.Range(2f,4f), Random.Range(-4f,4f)) * spawnDist);
		}

		for (int i = 0; i < 50; i++) {
			script.instantiateMolecule (carbondioxideData, new Vector3(Random.Range(-3f,4f),Random.Range(2f,4f), Random.Range(-4,4f)) * spawnDist);
		}

		script.instantiateMolecule (caffeineData, new Vector3(Random.Range(-3f,4f),Random.Range(3f,4f), Random.Range(-4f,4f)) * spawnDist);
		script.instantiateMolecule (caffeineData, new Vector3(Random.Range(-3f,4f),Random.Range(3f,4f), Random.Range(-4f,4f)) * spawnDist);
        script.instantiateMolecule(caffeineData, new Vector3(Random.Range(-3f, 4f), Random.Range(3f, 4f), Random.Range(-4f, 4f)) * spawnDist);



        script.instantiateMolecule (saturatedfatData, new Vector3(Random.Range(-3f,4f),Random.Range(3f,4f), Random.Range(-4f,4f)) * spawnDist);


        script.instantiateMolecule(sulfuricacidData, new Vector3(Random.Range(-3f, 4f), Random.Range(3f, 4f), Random.Range(-4f, 4f)) * spawnDist);

        script.instantiateMolecule(nitroData, new Vector3(Random.Range(-3f, 4f), Random.Range(3f, 4f), Random.Range(-4f, 4f)) * spawnDist);


        
        script.instantiateMolecule (atpData, new Vector3(Random.Range(-3f,4f),Random.Range(3f,4f), Random.Range(-4f,4f)) * spawnDist);
        /*
       script.instantiateMiniatureRigidMolecule(saturatedfatData, new Vector3(Random.Range(-3f, 4f), Random.Range(3f, 4f), Random.Range(-4f, 4f)));

      script.instantiateMiniatureRigidMolecule(atpData, new Vector3(Random.Range(-3f, 4f), Random.Range(3f, 4f), Random.Range(-4f, 4f)) );


        script.instantiateMiniatureRigidMolecule(caffeineData, new Vector3(Random.Range(-3f, 4f), Random.Range(3f, 4f), Random.Range(-4f, 4f)));

        script.instantiateMiniatureRigidMolecule(aspirinData, new Vector3(Random.Range(-3f, 4f), Random.Range(2f, 4f), Random.Range(-4f, 4f)));*/





    }

    // Update is called once per frame
    void Update () {
		
	}



	public MoleculeData loadMolecule(string fileName, string moleculeName){
		MoleculeData newMolecule = new MoleculeData ();

		JObject dataObj = LoadGameData (fileName);

		//Give more information when a dataObj cannot be loaded
		if (dataObj == null) Debug.LogError("Tried to load an invalid molecule file: " + fileName + ", name: " + moleculeName);

		atoms atm = dataObj ["PC_Compounds"] [0]["atoms"].ToObject<atoms>() ;
		bonds bnd = dataObj ["PC_Compounds"] [0]["bonds"].ToObject<bonds>();
		conformers cnf =  dataObj ["PC_Compounds"] [0]["coords"][0]["conformers"][0].ToObject<conformers>();
        //Debug.Log (atm.element [0]);
//        Debug.Log("Loading Molecule");

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



