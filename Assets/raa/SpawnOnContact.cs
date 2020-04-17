using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnOnContact : MonoBehaviour {
	public static bool inProgress;//keep track of if spawning is in progress, if it is, don't allow new spawning to start

	public GameObject target;
	public DataManager dataManager;
	public MoleculeCreator script;
	public BoxCollider spawnArea;
	public string[] toSpawn;
	public int[] number;
	public float forceAtSpawnMolecule;
	public float forceAtSpawnAtoms;
	public AudioClip spawnClip;
	public float deletePerFrame;
	public float deleteReload;
	public float spawnPerFrame;
	public float spawnReload;

	private MoleculeData[] datas;
	private bool grabbed;
	void OnGrab() { grabbed = true; }
	void OnRelease() { grabbed = false; }
	// Use this for initialization
	void Start () {
		datas = new MoleculeData[toSpawn.Length];
		for (int i = 0; i < toSpawn.Length; i++)
		{
			datas[i] = dataManager.loadMolecule(toSpawn[i], toSpawn[i].Substring(0, toSpawn[i].IndexOf(".")));
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

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

	private IEnumerator DestroyAndSpawn()
	{
		inProgress = true;//set inProgress to indicate that molecules are currently being spawned
		yield return StartCoroutine(DestroyAllMolecules());
		yield return StartCoroutine(Spawn());
		inProgress = false;
	}

	private IEnumerator DestroyAllMolecules()
	{
		GameObject[] bonds = GameObject.FindGameObjectsWithTag("Bond");//get the bonds at the start
		GameObject[] molecules = GameObject.FindGameObjectsWithTag("Molecule");//get the bonds at the start
		GameObject[] atoms = GameObject.FindGameObjectsWithTag("Atom");//get the bonds at the start

		int deletionsSinceReload = 0;
		//delete bonds first, because the bonds script references the atoms. If atoms are deleted first, NullReferenceExceptions may occur.
		foreach (GameObject g in bonds)
		{
			if (g == null) continue;//this gameobject might have already been deleted
			Destroy(g);
			deletionsSinceReload++;
			if (deletionsSinceReload >= deletePerFrame)
			{
				deletionsSinceReload = 0;
				yield return new WaitForSeconds(deleteReload);

			}
		}
		//delete molecules
		foreach(GameObject g in molecules)
		{
			if (g == null) continue;//this gameobject might have already been deleted
			Destroy(g);
			deletionsSinceReload++;
			if (deletionsSinceReload >= deletePerFrame)
			{
				deletionsSinceReload = 0;
				yield return new WaitForSeconds(deleteReload);

			}
		}
		//by now, most atoms are deleted because they were part of molecules. There might be atoms not part of a molecule, so these need to be deleted.
		foreach (GameObject g in atoms)
		{
			if (g == null) continue;//this gameobject might have already been deleted
			Destroy(g);
			deletionsSinceReload++;
			if (deletionsSinceReload >= deletePerFrame)
			{
				deletionsSinceReload = 0;
				yield return new WaitForSeconds(deleteReload);

			}
		}
		yield return new WaitForEndOfFrame();

	}

	private IEnumerator Spawn()
	{
		int upTill = number.Length;
		int spawnsSinceReload = 0;
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
				GameObject g = script.instantiateMolecule(datas[i], pos);

				Vector3 theForceForThisMolecule = UnityEngine.Random.insideUnitSphere.normalized * forceAtSpawnMolecule;
				foreach (Rigidbody rig in g.GetComponentsInChildren<Rigidbody>())
				{
					rig.AddForce(theForceForThisMolecule + UnityEngine.Random.insideUnitSphere.normalized * forceAtSpawnAtoms);
				}
				spawnsSinceReload++;
				if(spawnsSinceReload >= spawnPerFrame)
				{
					spawnsSinceReload = 0;
					yield return new WaitForSeconds(spawnReload);
				}
			}
		}
		yield return new WaitForEndOfFrame();
		//foreach(GameObject g in GameObject.FindGameObjectsWithTag("Molecule"))
		//{
		//	//for this molecule, add the same force to each atom so that the whole molecule moves in the same direction
		//	Vector3 theForceForThisMolecule = UnityEngine.Random.insideUnitSphere.normalized * forceAtSpawnMolecule;
		//	foreach (Rigidbody rig in g.GetComponentsInChildren<Rigidbody>())
		//	{
		//		rig.AddForce(theForceForThisMolecule + UnityEngine.Random.insideUnitSphere.normalized * forceAtSpawnAtoms);
		//	}
		//}
	}
}
