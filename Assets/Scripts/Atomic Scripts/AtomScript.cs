using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtomScript : MonoBehaviour {

	Rigidbody rb;

	public GameObject bondGameObject;

	public List<GameObject> bondedAtoms = new List<GameObject>();



	private List<GameObject> bonds = new List<GameObject> ();

	private int allowedBonds {
		get;
		set;
	}

	private bool bondForming  {
		get;
		set;
	}

	public float speed;
	void Awake () {
		bondForming = false;
		if (gameObject.name.Contains("("))
			gameObject.name = gameObject.name.Substring (0, gameObject.name.IndexOf ("Atom") +4);
		//transform.localScale = new Vector3 (.1f, .1f, .1f);
		int bondNum = 0;

		if (gameObject.name == "Oxygen Atom") {
			gameObject.transform.localScale = new Vector3(.1f,.1f,.1f);
			//gameObject.transform.localScale *= .1f;
			bondNum = 2;
		} else if (gameObject.name == "Carbon Atom") {
			gameObject.transform.localScale = new Vector3(.1f,.1f,.1f);
			//gameObject.transform.localScale *= .1f;
			bondNum = 4;
		} else if (gameObject.name == "Nitrogen Atom") {
			gameObject.transform.localScale = new Vector3(.1f,.1f,.1f);
			//gameObject.transform.localScale *= .1f;
			bondNum = 3;
		} else if (gameObject.name == "Hydrogen Atom") {
			gameObject.transform.localScale = new Vector3(.075f,.075f,.075f);
			bondNum = 1;
		} else if (gameObject.name == "Phosphorus Atom") {
			gameObject.transform.localScale = new Vector3(.1f,.1f,.1f);
			//gameObject.transform.localScale *= .1f;
			bondNum = 5;
		}


		allowedBonds = bondNum;

		rb = gameObject.GetComponent<Rigidbody> ();

		for (int i = 0; i < allowedBonds; i++) {
			Vector3 posMod;;
			if (i == 0) {
				posMod = Vector3.right;
			} else if (i == 1) {
				posMod = Vector3.Cross (Vector3.right, Vector3.up);
			} else if (i == 2) {
				posMod = Vector3.up;
			} else if (i == 3) {
				posMod = Vector3.down;
			} else {
				posMod = Random.onUnitSphere;
			}

			GameObject newBond= Instantiate (bondGameObject,posMod * transform.localScale.x * .5f + transform.position, Quaternion.identity);
			newBond.transform.parent = transform;
			bonds.Add (newBond);
			Bond bondScript = newBond.GetComponent<Bond> ();
			bondScript.setInitialAtom (gameObject);

		}

		for (int i = 0; i < bonds.Count; i++) {
			for (int j = 0; j < bonds.Count; j++) {
				Physics.IgnoreCollision (bonds [i].GetComponentInChildren<CapsuleCollider> (), bonds [j].GetComponentInChildren<CapsuleCollider> (), true);
			}
		}
		//rb.velocity = gameObject.transform.forward * 20;
		//rb.velocity = transform.forward * speed;

		//rb.AddForce(0,1000f,0);
		//Debug.Log (ForceMode.Impulse);


	}




	// Update is called once per frame

	void Update (){
		bondForming = false;

		List<GameObject> recalibratedBondedAtoms = new List<GameObject> ();
		CharacterJoint[] currentJoints = gameObject.GetComponents<CharacterJoint> ();
		for (int i = 0; i < currentJoints.Length; i++) {
			recalibratedBondedAtoms.Add (currentJoints [i].connectedBody.gameObject);

		}
		bondedAtoms = recalibratedBondedAtoms;
		for (int i = 0; i < bonds.Count; i++) {
			Bond bondScript = bonds [i].GetComponent<Bond> ();
			bool removeBondEndAtom = true;
			GameObject[] bondConnectedAtoms = bondScript.getConnectedAtoms ();
			for (int j = 0; j < bondedAtoms.Count; j++) {
				if(bondedAtoms[j].Equals(bondConnectedAtoms[1]))
					removeBondEndAtom = false;
			}
			if (removeBondEndAtom == true) {
				bondScript.setConnectedAtom (null);
			}
		}

	}

	void FixedUpdate () {

		//Debug.Log (gameObject.name+": " + bondForming);
		//rb.AddForce (force * Vector3.up);
		//rb.velocity = gameObject.transform.forward * 10;
		//transform.rotation.x = 5;
		//rb.velocity = rb.velocity.normalized * speed;

		//Debug.Log (rb.velocity.magnitude);


	}

	public void addBondedAtom(GameObject atom) {
		bondedAtoms.Add (atom);
	}

	public void removeBondedAtom(GameObject atom) {
	
		bondedAtoms.Remove (atom);
	
	}

	public List<GameObject> getBondedAtoms(){
		return bondedAtoms;
	}


	 void OnCollisionEnter(Collision col){
		GameObject obj = col.collider.gameObject;
		if (obj.tag == "Projectile") {
			if(!hasDoubleBond()){
				CharacterJoint[] bonds = gameObject.GetComponents<CharacterJoint> ();
				for(int i = bonds.Length - 1; i >= 0; i--) {
					AtomScript script = bonds [i].connectedBody.GetComponent<AtomScript> ();
					script.breakBondWith (gameObject);
					Destroy (bonds[i]);
				}



			}
			rb.AddForce (1000f * obj.transform.forward);

		}

		if (obj.tag == "Atom") {
			AtomScript script = obj.GetComponent<AtomScript> ();

			if (bondedAtoms.Count < allowedBonds && script.bondedAtoms.Count < script.allowedBonds 
				|| bondedAtoms.Count < allowedBonds && script.bondForming == true) {

				Debug.Log ("Hit@!");
				bondForming = true;
				
				CharacterJoint newJoint = gameObject.AddComponent<CharacterJoint> ();
				newJoint.connectedBody = obj.GetComponent<Rigidbody>();
				newJoint.breakForce = 5000f;


			//	script.addBondedAtom (gameObject);
				addBondedAtom (obj);


				float bondDistance = 10000f;
				GameObject closestBond = null;
				foreach (GameObject bond in bonds) {
					Bond tempBondScript = bond.GetComponent<Bond> ();
					if (tempBondScript.getConnectedAtoms() [1] == null) {
						if (Vector3.Distance (bond.transform.position, obj.transform.position) < bondDistance) {
							bondDistance = Vector3.Distance (bond.transform.position, obj.transform.position);
							closestBond = bond;
						}
					}
				}

				Bond bondScript = closestBond.GetComponent<Bond> ();
				bondScript.setConnectedAtom (obj);




			}
		
		}
		//bondForming = false;




	}

	void OnJointBreak() {
		List<GameObject> recalibratedBondedAtoms = new List<GameObject> ();
		CharacterJoint[] currentJoints = gameObject.GetComponents<CharacterJoint> ();
		for (int i = 0; i < currentJoints.Length; i++) {
			recalibratedBondedAtoms.Add (currentJoints [i].connectedBody.gameObject);

		}
		bondedAtoms = recalibratedBondedAtoms;
		for (int i = 0; i < bonds.Count; i++) {
			Bond bondScript = bonds [i].GetComponent<Bond> ();
			bool removeBondEndAtom = true;
				GameObject[] bondConnectedAtoms = bondScript.getConnectedAtoms ();
			for (int j = 0; j < bondedAtoms.Count; j++) {
				if(bondedAtoms[j].Equals(bondConnectedAtoms[1]))
					removeBondEndAtom = false;
			}
			if (removeBondEndAtom == true) {
				bondScript.setConnectedAtom (null);
			}
		}
		Debug.Log (gameObject.name + " broke");
	}

	/*

	public void breakAllBonds(){
		FixedJoint joint = gameObject.GetComponent<FixedJoint> ();
		if (joint != null) {
			bondedAtoms.Remove (joint.connectedBody.gameObject);
			Destroy (gameObject.GetComponent<FixedJoint> ());

		} 
	}*/

	public void breakBondWith(GameObject atom){
		bondedAtoms.Remove (atom);
		CharacterJoint[] bonds = gameObject.GetComponents<CharacterJoint> ();
		for (int i = 0; i < bonds.Length; i++) {
			if (bonds [i].connectedBody == gameObject) {
				Destroy (bonds [i]);
			}
		}


	}

	private bool hasDoubleBond(){
		for (int i = 0; i < bondedAtoms.Count; i++) {
			for (int j = 0; j < bondedAtoms.Count; j++){
				if(bondedAtoms[i] == bondedAtoms[j]&& i != j){
					return true;
				}
			}
		}
		return false;
	
	}

	public List<GameObject> getBonds(){
		return bonds;
	}


}
