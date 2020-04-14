using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decay : MonoBehaviour {
	public string[] toSpawn;
	public float[] chance;
	public float deathTime;

	private Material[] mat;
	private float totalChance = 0;
	private float timeElapsed;
	// Use this for initialization
	void Start () {
		for (int i = 0; i < chance.Length; i++) totalChance += chance[i];
		Renderer[] rend = transform.GetComponentsInChildren<Renderer>();
		mat = new Material[rend.Length];
		for(int i = 0;i < rend.Length; i++)
		{
			mat[i] = Instantiate(rend[i].sharedMaterial);
			rend[i].sharedMaterial = mat[i];
		}
	}
	
	// Update is called once per frame
	void Update () {
		timeElapsed += Time.deltaTime;

		foreach(Material m in mat)
		{
			m.color = Color.Lerp(Color.white, Color.black, timeElapsed/deathTime);
		}
		if(deathTime < timeElapsed)
		{
			Die();
		}
	}

	void Die()
	{
		//select a gameobject to spawn by weighted random
		float chose = Random.Range(0, totalChance);
		for(int i = 0;i < chance.Length; i++)
		{
			chose -= chance[i];
			if(chose < 0)
			{
				//TODO: messy code
				GameObject.FindObjectOfType<MoleculeCreator>().instantiateMolecule(GameObject.FindObjectOfType<DataManager>().loadMolecule(toSpawn[i], toSpawn[i].Substring(toSpawn[i].IndexOf("."))), transform.position);
				//Instantiate(toSpawn[i], transform.position, transform.rotation);
				Destroy(gameObject);
				return;//destroy doesn't act immidiately
			}
		}
	}
}
