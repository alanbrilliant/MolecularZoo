using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {


	private List<GameObject> nitrogenList;
	private List<GameObject> oxygenList;
	private List<GameObject> hydrogenList;
	private List<GameObject> carbonList;

	public GameObject nitrogenAtom;
	public GameObject oxygenAtom;
	public GameObject carbonAtom;
	public GameObject hydrogenAtom;

	void Start () {
		
		nitrogenList = new List<GameObject> ();
		oxygenList = new List<GameObject> ();
		hydrogenList = new List<GameObject> ();
		carbonList = new List<GameObject> ();

		addAtoms (780, nitrogenAtom, nitrogenList);
		addAtoms (200, oxygenAtom, oxygenList);
		//addAtoms (, carbonAtom, carbonList);
		addAtoms (10, hydrogenAtom, hydrogenList);



	}






	private void addAtoms(int amount, GameObject atomType, List<GameObject> atomList){
		for (int i = 0; i < amount; i++) {
			float randY = Random.Range (1f, 8f);
			float randX = Random.Range (-6f, 6f);
			float randZ = Random.Range (-6f, 5f);
			Vector3 startPos = new Vector3 (randX, randY, randZ);
			startPos = Vector3.up;
			GameObject newAtom = (GameObject)GameObject.Instantiate (atomType, startPos, Quaternion.identity) as GameObject;
			newAtom.transform.localScale *= .232f;
			atomList.Add (newAtom);
			//GameObject newAatom = (GameObject)GameObject.Instantiate (atomType, startPos, Random.rotation) as GameObject;

		}



		for (int i = 0; i < atomList.Count; i += 2) {
			GameObject atom1 = atomList [i];
			GameObject atom2 = atomList [i+1];
			FixedJoint atom1Joint = atom1.AddComponent<FixedJoint> ();
			//atom1Joint.spring = 80;
			//atom1Joint.damper = 0;
			//SphereCollider nitCol = nit1.GetComponent<SphereCollider> ();

			AtomScript atom1Script = atom1.GetComponent<AtomScript> ();
			AtomScript atom2Script = atom2.GetComponent<AtomScript> ();

			atom1Script.addBondedAtom (atom2);
			atom2Script.addBondedAtom (atom1);


			float radius = atom1.transform.localScale.x * 2f;

			Collider col = atom2.GetComponent<SphereCollider> ();


			atom2.transform.position = atom1.transform.position + new Vector3(0,radius/2 * .5f,0);
			atom1Joint.connectedBody = atom2.GetComponent<Rigidbody>();

			atom2.GetComponent<Rigidbody> ().AddForce (1000f, 0, 0);
			atom1.GetComponent<Rigidbody> ().AddForce (-1000f, 0, 0);

			//atom1.GetComponent<Rigidbody> ().mass = 100;

			atom2.GetComponent<Rigidbody> ().velocity =  new Vector3(0,10f, 0);
			atom1.GetComponent<Rigidbody> ().velocity =  new Vector3(0,-10f, 0);

		}
		
	}

	/*
	private void addN2(){

		for (int i = 0; i < 500; i++) {
			float randY = Random.Range (1f, 3f);
			float randX = Random.Range (-2f, 2f);
			float randZ = Random.Range (-2f, 2f);
			Vector3 startPos = new Vector3 (randX, randY, randZ);
			GameObject nitroAtom = (GameObject)GameObject.Instantiate (nitrogenMolecule, startPos, Random.rotation) as GameObject;
			nitrogenList.Add (nitroAtom);
		}



		for (int i = 0; i < nitrogenList.Count; i += 2) {
			GameObject nit1 = nitrogenList [i];
			GameObject nit2 = nitrogenList [i+1];
			FixedJoint nit1Joint = nit1.AddComponent<FixedJoint> ();
			//SphereCollider nitCol = nit1.GetComponent<SphereCollider> ();

			AtomScript nit1Script = nit1.GetComponent<AtomScript> ();
			AtomScript nit2Script = nit2.GetComponent<AtomScript> ();

			nit1Script.addBondedAtom (nit2);
			nit2Script.addBondedAtom (nit1);


			float radius = nit1.transform.localScale.x * .5f;

			Collider col = nit2.GetComponent<SphereCollider> ();


			nit2.transform.position = nit1.transform.position + new Vector3(0,radius,0);
			nit1Joint.connectedBody = nit2.GetComponent<Rigidbody>();

		} 
	}*/



	
	// Update is called once per frame
	void Update () {
		
		
	}
}
