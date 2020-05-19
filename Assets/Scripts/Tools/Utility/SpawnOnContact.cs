using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Spawns objects when touching target.
 * The spawning is spread out over multiple frames to reduce lag.
 * The spawning will only occur if this object is grabbed (by a hand).
 * All existing molecules are deleted first.
 */
public class SpawnOnContact : MonoBehaviour {
	public static bool inProgress;//Keep track of if spawning is in progress, if it is, don't allow new spawning to start

	[Tooltip("Spawning will occur if hitting the target")]
	public GameObject target;
	[Tooltip("The DataManager to load molecule data")]
	public DataManager dataManager;
	[Tooltip("A box collider representing the counds to spawn in")]
	public BoxCollider spawnArea;
	[Tooltip("MUST be same length as number. The molecules to spawn. These names MUST be the name of the json file. E.g. for methane, use \"methane.json\"")]
	public string[] toSpawn;
	[Tooltip("MUST be same length as toSpawn. The number of molecules to spawn of each, corresponding to the same index in toSpawn")]
	public int[] number;
	[Tooltip("The force to add to all atoms of a spawned molecule in the same direction. Used so the molecules travel around.")]
	public float forceAtSpawnMolecule;
	[Tooltip("The force to add to each atom in an individually randomized direction. Used so that the molecules vibrate and rotate a bit.")]
	public float forceAtSpawnAtoms;
	[Tooltip("The audio clip to play when spawning")]
	public AudioClip spawnClip;
	[Tooltip("The number of atoms/bonds/things to delete each frame")]
	public float deletePerFrame;
	[Tooltip("The time to wait between deleting deletePerFrame atoms/bonds/things")]
	public float deleteReload;
	[Tooltip("The number of molecules to spawn per frame")]
	public float spawnPerFrame;
	[Tooltip("The time to wait between spawning spawnPerFrame molecules")]
	public float spawnReload;

	//The molecular datas to spawn molecules
	private MoleculeData[] datas;
	//Is this GameObject grabbed?
	private bool grabbed;

	//update if this GameObject is grabbed
	void OnGrab() { grabbed = true; }
	void OnRelease() { grabbed = false; }

	// Use this for initialization
	void Start () {
		//load the datas for each molecule in toSpawn ahead of time
		datas = new MoleculeData[toSpawn.Length];
		for (int i = 0; i < toSpawn.Length; i++)
		{
			datas[i] = dataManager.loadMolecule(toSpawn[i] + "data.json", toSpawn[i]);
		}
	}

	//Called when colliding with a collider that is not isTrigger
	private void OnCollisionEnter(Collision col)
	{
		//cannot be already spawning molecules
		//this gameobject must be grabbed
		//must be colliding with the target gameobject
		if(!inProgress && grabbed && target == col.gameObject)
		{
			StartCoroutine(DestroyAndSpawn());
			AudioSource audio = target.GetComponent<AudioSource>();
			if (audio!= null && spawnClip !=null) {
				audio.clip = spawnClip;
				audio.Play();
			}
		}
	}

	//Destroy all molecules and spawn new ones
	private IEnumerator DestroyAndSpawn()
	{
		inProgress = true;//set inProgress to indicate that molecules are currently being spawned
		yield return StartCoroutine(DestroyAllMolecules());
		yield return StartCoroutine(Spawn());
		inProgress = false;
	}

	//Destroyes all molecules over a few frames
	private IEnumerator DestroyAllMolecules()
	{
		//find everything that needs to be destroyed
		GameObject[] bonds = GameObject.FindGameObjectsWithTag("Bond");//get the bonds at the start
		GameObject[] molecules = GameObject.FindGameObjectsWithTag("Molecule");//get the bonds at the start
		GameObject[] atoms = GameObject.FindGameObjectsWithTag("Atom");//get the bonds at the start

		//destroy
		yield return StartCoroutine(DestroyArrayOfGameObjects(bonds));
		yield return StartCoroutine(DestroyArrayOfGameObjects(molecules));
		yield return StartCoroutine(DestroyArrayOfGameObjects(atoms));

		//wait 1 more frame
		yield return new WaitForEndOfFrame();
	}

	//Destroy an aray of GameObjects toDestroy based on deletePerFrame and deleteReload
	private IEnumerator DestroyArrayOfGameObjects(GameObject[] toDestroy)
	{
		int deletionsSinceReload = 0;
		//delete bonds first, because the bonds script references the atoms. If atoms are deleted first, NullReferenceExceptions may occur.
		foreach (GameObject g in toDestroy)
		{
			if (g == null) continue;//this gameobject might have already been deleted, so check if null
			Destroy(g);
			deletionsSinceReload++;
			//after deleting deletePerFrame GameObjects, wait deleteReload
			if (deletionsSinceReload >= deletePerFrame)
			{
				deletionsSinceReload = 0;
				yield return new WaitForSeconds(deleteReload);

			}
		}
	}
	
	//Spawns all the molecules in toSpawn, each molecule is spawned number[the index in toSpawn] times
	private IEnumerator Spawn()
	{
		int spawnsSinceReload = 0;
		//do through all the molecules in toSpawn
		for(int i = 0;i < toSpawn.Length; i++)
		{
			//repeat number[i] times for each toSpawn[i]
			for (int j = 0; j < number[i]; j++)
			{
				//get random position
				float x = UnityEngine.Random.Range(-spawnArea.size.x / 2, spawnArea.size.x / 2);
				float y = UnityEngine.Random.Range(-spawnArea.size.y / 2, spawnArea.size.y / 2);
				float z = UnityEngine.Random.Range(-spawnArea.size.z / 2, spawnArea.size.z / 2);
				Vector3 pos = spawnArea.transform.position + spawnArea.center;
				pos += new Vector3(x, y, z);
				//spawn a molecule
				GameObject g = MoleculeCreator.main.instantiateMolecule(datas[i], pos);

				//randomly get a direction and force for this molecule
				Vector3 theForceForThisMolecule = UnityEngine.Random.insideUnitSphere.normalized * forceAtSpawnMolecule;
				foreach (Rigidbody rig in g.GetComponentsInChildren<Rigidbody>())
				{
					//for each atom (each Rigidbody), add the force for this molecule plus some atom-individual random force
					rig.AddForce(theForceForThisMolecule + UnityEngine.Random.insideUnitSphere.normalized * forceAtSpawnAtoms);
				}

				spawnsSinceReload++;
				//after spawning spawnPerFrame molecules, wait spawnReload
				if(spawnsSinceReload >= spawnPerFrame)
				{
					spawnsSinceReload = 0;
					yield return new WaitForSeconds(spawnReload);
				}
			}
		}

		//wait 1 frame
		yield return new WaitForEndOfFrame();
	}
}
