
using System.Collections;
using System.Collections.Generic;

using System.Linq;

using UnityEngine;

public class AtomScript : MonoBehaviour {

	Rigidbody rb;

	public GameObject bondGameObject;
	public GameObject stubBondGameObject;

	public List<GameObject> bondedAtoms = new List<GameObject>();



	private List<GameObject> bonds = new List<GameObject> ();

	private List<GameObject> stubBonds = new List<GameObject> ();

	public int nBondConnections = 0;

	private AudioSource audioSrc;
	public AudioClip bounceSound;
	public AudioClip bangSound;
	public AudioClip snapSound;

	private AudioClip moleculeNameSound;


	//hasBroken is if the atom has had a broken joint at one point in its lifetime. It's used to decide whether the atom should say its name when grabbed
	private bool hasBroken = false;
	//jointBreak is a flag thats set to true whenever a joint breaks. It's used to determine what happens after a joint breaks(deleting bond, for example)
	//It's set to true again after the routine is finished
	private bool jointBreak = false;

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


		audioSrc = gameObject.AddComponent <AudioSource> ();
		audioSrc.playOnAwake = false;
		audioSrc.spatialBlend = .73f;


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
		} else if (gameObject.name == "Sulfur Atom") {
			gameObject.transform.localScale = new Vector3(.1f,.1f,.1f);
			//gameObject.transform.localScale *= .1f;
			bondNum = 6;
		} 


		allowedBonds = bondNum;

		createStubBonds ();



		/*
		for (int i = 0; i < bonds.Count; i++) {
			for (int j = 0; j < bonds.Count; j++) {
				Physics.IgnoreCollision (bonds [i].GetComponentInChildren<CapsuleCollider> (), bonds [j].GetComponentInChildren<CapsuleCollider> (), true);
			}
		}*/
		//rb.velocity = gameObject.transform.forward * 20;
		//rb.velocity = transform.forward * speed;

		//rb.AddForce(0,1000f,0);
		//Debug.Log (ForceMode.Impulse);


	}




	// Update is called once per frame

	void Update (){

		bondForming = false;

		if (jointBreak) {
			bondedAtomsRecalibration ();
		}

		//if (bondedAtoms.Count< 

		//bondedAtomsRecalibration ();



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
		if (obj.tag == "ExplosiveBullet") {
			if(!hasDoubleBond()){
				CharacterJoint[] bonds = gameObject.GetComponents<CharacterJoint> ();
				for(int i = bonds.Length - 1; i >= 0; i--) {
					AtomScript script = bonds [i].connectedBody.GetComponent<AtomScript> ();
					script.breakBondWith (gameObject);
					Destroy (bonds[i]);
				}



			}
			rb.AddForce (1000f * obj.transform.forward);

			audioSrc.volume = .1f;
			audioSrc.clip = bangSound;
			audioSrc.Play ();

		}

		if (obj.tag == "Bullet"){
			audioSrc.volume = .2f;
			audioSrc.clip = bounceSound;
			audioSrc.Play ();
		}

		if (obj.tag == "Atom") {
			AtomScript script = obj.GetComponent<AtomScript> ();

			if (nBondConnections < allowedBonds && script.nBondConnections < script.allowedBonds 
				/*|| bondedAtoms.Count < allowedBonds && script.bondForming == true*/) {
				if (bondedAtoms.Contains(obj) == false){
					bondForming = true;

					int possibleBondsOfThisAtom = allowedBonds - bondedAtoms.Count;
					int possibleBondsOfCollidedAtom = script.allowedBonds - script.bondedAtoms.Count;
					int newBondOrder = Mathf.Min (possibleBondsOfThisAtom, possibleBondsOfCollidedAtom);

					GameObject newBond = Instantiate (bondGameObject, gameObject.transform.position, Quaternion.identity);
					newBond.GetComponent<Bond> ().formBond (gameObject, obj, 1);


					//	script.addBondedAtom (gameObject);
				}






			}

		}
		//bondForming = false;




	}

	void OnJointBreak() {
		hasBroken = true;
		jointBreak = true;
		Debug.Log (gameObject.name + " broke");

		audioSrc.volume = .1f;
		audioSrc.clip = snapSound;
		audioSrc.Play ();
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
			if (bonds [i].connectedBody.gameObject == atom) {
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

	public void setMoleculeNameSound(AudioClip clip){
		moleculeNameSound = clip;
	}

	public void playMoleculeNameSound(){

		if (hasBroken == false) {
			if (moleculeNameSound != null) {
				audioSrc.clip = moleculeNameSound;
				audioSrc.Play ();
			}
		}
	}

	public AudioClip getMoleculeNameSound(){
		return moleculeNameSound;
	}

	private void bondedAtomsRecalibration(){
		GameObject brokenBond = null;
		GameObject brokenBondWithAtom = null;
		List<GameObject> currentJointedAtoms = new List<GameObject> ();
		List<GameObject> currentBondedAtoms = new List<GameObject> ();
		CharacterJoint[] currentJoints = gameObject.GetComponents<CharacterJoint> ();



		for (int i = 0; i < currentJoints.Length; i++) {
			currentJointedAtoms.Add (currentJoints [i].connectedBody.gameObject);
		}

		for (int i = 0; i < bonds.Count; i++) {
			GameObject[] bondConnections = bonds[i].GetComponent<Bond>().getConnectedAtoms();
			foreach (GameObject atom in bondConnections) {
				if (atom != gameObject)
					currentBondedAtoms.Add (atom);
			}
		}

		for (int i = 0; i < currentBondedAtoms.Count; i++) {
			if (currentJointedAtoms.Contains(currentBondedAtoms[i]) == false)
				brokenBondWithAtom = currentBondedAtoms[i];

		}

		for (int i = 0; i < bonds.Count; i++) {
			GameObject[] bondConnections = bonds[i].GetComponent<Bond>().getConnectedAtoms();
			if (brokenBondWithAtom != null && (bondConnections [0] == brokenBondWithAtom  || bondConnections [1] == brokenBondWithAtom) )
				brokenBond = bonds [i];
		}

		if (brokenBond != null) {

			bondBreakage (brokenBond);
			brokenBondWithAtom.GetComponent<AtomScript> ().bondBreakage (brokenBond);

			bondedAtoms.Remove (brokenBondWithAtom);
			brokenBondWithAtom.GetComponent<AtomScript> ().breakBondWith (gameObject);

			Destroy (brokenBond);

		}
			





		/*
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
		}*/
	}

	private void createStubBonds(){
		rb = gameObject.GetComponent<Rigidbody> ();

		for (int i = 0; i < allowedBonds; i++) {
			Vector3 posMod;
			Vector3 rotMod;
			float tertahedralNum = 1;


			if (i == 0) {
				posMod = new Vector3 (tertahedralNum, Mathf.Sqrt (2), 0);

			} else if (i == 1) {
				posMod =  new Vector3 (-tertahedralNum, Mathf.Sqrt (2), 0);

			} else if (i == 2) {
				posMod =  new Vector3 (0, -Mathf.Sqrt (2), -tertahedralNum);
			} else if (i == 3) {
				posMod =  new Vector3 (0, -Mathf.Sqrt (2), tertahedralNum);
			} else {
				posMod = Random.onUnitSphere;
			}

			GameObject newStubBond = Instantiate (stubBondGameObject, posMod * transform.localScale.x * .5f + transform.position, Quaternion.identity);
			newStubBond.transform.parent = transform;
			stubBonds.Add (newStubBond);
			Bond bondScript = newStubBond.GetComponent<Bond> ();
			bondScript.setInitialAtom (gameObject);
			bondScript.posMod = posMod;
		}



	
	}

	public List<GameObject> makeBondConnection(GameObject atomToBond,GameObject bond, int bondOrder){
		addBondedAtom (atomToBond);
		bonds.Add (bond);

		Debug.Log (bondOrder);

		List<GameObject> stubBondReference = new List<GameObject> ();



		for (int i = 0; i < bondOrder; i++) {
			float bondDistance = 10000f;
			GameObject closestBondStub = null;
			foreach (GameObject bondStub in stubBonds) {
				Bond tempBondScript = bondStub.GetComponent<Bond> ();
				if (Vector3.Distance (bondStub.transform.position, atomToBond.transform.position) < bondDistance) {
					if (bondStub.activeSelf && stubBondReference.Contains(bondStub) == false) {
						bondDistance = Vector3.Distance (bondStub.transform.position, atomToBond.transform.position);
						closestBondStub = bondStub;
						Debug.Log ("HEfhfr");
					}
				

				}
			}
			stubBondReference.Add (closestBondStub);
			foreach (GameObject stub in stubBondReference) {
				stub.SetActive (false);
			}

			nBondConnections++;

		}


		return stubBondReference;
	}

	public void respawnStubs(GameObject bond){
		List<GameObject> stubBondReferences = bond.GetComponent<Bond> ().getStubBondReferences (gameObject);
		for (int i = 0; i < stubBondReferences.Count; i++) {
			for (int j = 0; j < stubBonds.Count; j++) {
				if (stubBondReferences [i] == stubBonds [j]) {
					stubBonds [j].SetActive (true);
					stubBonds [j].transform.position = bond.transform.position;
				}
					
			}
		}
	}

	public void bondBreakage(GameObject bond){
		respawnStubs (bond);
		nBondConnections -= bond.GetComponent<Bond> ().getOrder ();
		removeBond (bond);
	}

	public void removeBond(GameObject bond){
		bonds.Remove (bond);
	}

}

