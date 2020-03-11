using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnOnContact : MonoBehaviour {
	public GameObject target;
	public DataManager dataManager;
	public MoleculeCreator script;
	public string[] toSpawn;
	public int[] number;

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
			Spawn();
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
				script.instantiateMolecule(datas[i], transform.position);
			}
		}
	}
}
