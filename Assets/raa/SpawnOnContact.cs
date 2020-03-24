using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnOnContact : MonoBehaviour {
	public GameObject target;
	public DataManager dataManager;
	public MoleculeCreator script;
	public BoxCollider spawnArea;
	public string[] toSpawn;
	public int[] number;
	public float forceAtSpawnMolecule;
	public float forceAtSpawnAtoms;

	private MoleculeData[] datas;

	// Use this for initialization
	void Start () {
		datas = new MoleculeData[toSpawn.Length];
		for (int i = 0; i < toSpawn.Length; i++)
		{
			datas[i] = dataManager.loadMolecule(toSpawn[i], toSpawn[i].Substring(toSpawn[i].IndexOf(".")));
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnCollisionEnter(Collision col)
	{
		if(target == col.gameObject)
		{
			DestroyAllMolecules();
			Spawn();
		}
	}

	private void DestroyAllMolecules()
	{
		foreach(GameObject g in GameObject.FindGameObjectsWithTag("Molecule"))
		{
			Destroy(g);
		}
		foreach (GameObject g in GameObject.FindGameObjectsWithTag("Atom"))
		{
			Destroy(g);
		}
		foreach (GameObject g in GameObject.FindGameObjectsWithTag("Bond"))
		{
			Destroy(g);
		}
	}

	private void Spawn()
	{
		int upTill = number.Length;
		if(toSpawn.Length < upTill) upTill = toSpawn.Length;
		for(int i = 0;i < toSpawn.Length; i++)
		{
			for (int j = 0; j < number[i]; j++)
			{
				float x = UnityEngine.Random.Range(-spawnArea.size.x / 2, spawnArea.size.x / 2);
				float y = UnityEngine.Random.Range(-spawnArea.size.y / 2, spawnArea.size.y / 2);
				float z = UnityEngine.Random.Range(-spawnArea.size.z / 2, spawnArea.size.z / 2);
				Vector3 pos = spawnArea.transform.position + spawnArea.center;
				pos += new Vector3(x, y, z);
				script.instantiateMolecule(datas[i], pos);
			}
		}

		foreach(GameObject g in GameObject.FindGameObjectsWithTag("Molecule"))
		{
			//for this molecule, add the same force to each atom so that the whole molecule moves in the same direction
			Vector3 theForceForThisMolecule = UnityEngine.Random.insideUnitSphere.normalized * forceAtSpawnMolecule;
			foreach (Rigidbody rig in g.GetComponentsInChildren<Rigidbody>())
			{
				rig.AddForce(theForceForThisMolecule + UnityEngine.Random.insideUnitSphere.normalized * forceAtSpawnAtoms);
			}
		}
	}
}
