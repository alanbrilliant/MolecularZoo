using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//rigidbody is required for setting spawned molecule velocity
[RequireComponent(typeof(Rigidbody))]
public class Decay : MonoBehaviour {

	[Tooltip("The objects to spawn once the time is up")]
	public string[] toSpawn;
	[Tooltip("The chances of each object from \"toSpawn\" spawning. A weighted random is taken based on this. Only one object will spawn, as if the chances all add up to 1")]
	public float[] chance;
	[Tooltip("The time before decaying. Decaying will delete this GameObject and spawn from \"toSpawn\"")]
	public float deathTime;

	//the materials to change
	private Material[] mat;
	//the total chance, i.e. summation of float[] chance
	private float totalChance = 0;
	//how much time has passed. Used to determine the color of the materials and if it should "Die"
	private float timeElapsed;

	private Rigidbody rig;

	// Use this for initialization
	void Start () {
		rig = GetComponent<Rigidbody>();
		//Set totalChance to the summation of chance
		for (int i = 0; i < chance.Length; i++) totalChance += chance[i];

		//get all renderers
		Renderer[] rend = transform.GetComponentsInChildren<Renderer>();

		//get all materials (1 per renderer only).
		//however, to avoid one instance of Decay's color affecting another GameObject with the same materials and also Decay.
		//to do this, copies of the materials are stored instead of the original. The renderers are also updated to the copies.
		mat = new Material[rend.Length];
		for(int i = 0;i < rend.Length; i++)
		{
			mat[i] = Instantiate(rend[i].sharedMaterial);
			rend[i].sharedMaterial = mat[i];
		}
	}
	
	// Update is called once per frame
	void Update () {
		//update the time that has passed.
		timeElapsed += Time.deltaTime;

		//update the materials' colors by linear interpolation of colors from white to black based on (time) / (lifetime), i.e. the fraction of it's life that has been spent.
		foreach(Material m in mat)
		{
			m.color = Color.Lerp(Color.white, Color.black, timeElapsed/deathTime);
		}
		//if time is up, die
		if(deathTime < timeElapsed)
		{
			Die();
		}
	}

	//destroys the GameObject and spawns the object from toSpawn
	void Die()
	{
		//select a gameobject to spawn by weighted random. 0 < chose < (totalChance = summation of chance)
		float chose = Random.Range(0, totalChance);
		//go through all the chance array
		for(int i = 0;i < chance.Length; i++)
		{
			//subtract chance[i]. This will cause chose to become negative at some point since chose < (totalChance = summation of chance)
			chose -= chance[i];
			//by checking when chose is negative, a weighted random is achieved. A higher value in the chance array will cause a higher chance that chose is negative in this iteration
			if(chose < 0)
			{

				//These lines of code just spawn the molecule
				DataManager dataManager = GameObject.FindObjectOfType<DataManager>();
				MoleculeData moleculeData = dataManager.loadMolecule(toSpawn[i] + "data.json", toSpawn[i]);
				//using Dino.mainMoleculeCreator, instantiate the molecule
				GameObject spawned = MoleculeCreator.main.instantiateMolecule(moleculeData, transform.position);
				foreach (Rigidbody r in spawned.GetComponentsInChildren<Rigidbody>())
				{
					//for each atom (each Rigidbody), add the force for this molecule plus some atom-individual random force
					r.velocity = rig.velocity;
				}
				//get an atom from the molecule
				AtomScript atom = spawned.GetComponentInChildren<AtomScript>();
				//play the noise of the atom. This if statement should never be false, since a molecule should have atoms.
				if (atom != null)
				{
					atom.playMoleculeNameSound();
				}
				//now that the molecule has been spawned, destroy the GameObject
				Destroy(gameObject);
				return;//destroy doesn't act immidiately, so return even though the GameObject (including this script) will be deleted.
			}
		}
	}
}
