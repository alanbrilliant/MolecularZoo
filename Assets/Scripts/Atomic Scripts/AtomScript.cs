
using System.Collections;
using System.Collections.Generic;

using System.Linq;

using UnityEngine;

public class AtomScript : MonoBehaviour { //TODO: Change AtomScript to Atom
	

	Rigidbody rb;

	public GameObject bondGameObject;
	public GameObject stubBondGameObject;

	//TODO: change GameObject to AtomScript in List<>
	public List<GameObject> bondedAtoms = new List<GameObject>();



	private List<GameObject> bonds = new List<GameObject> ();

	//Stub bond cylanders depict where bonds can form
	private List<GameObject> stubBonds = new List<GameObject> ();

	public int nBondConnections = 0;

	private AudioSource audioSrc;
	public AudioClip bounceSound;
	public AudioClip bangSound;
	public AudioClip snapSound;

	private AudioClip moleculeNameSound;



	//hasBroken is if the atom has had a broken joint at one point in its lifetime. It's used to decide whether the atom should say its name when grabbed
	//TODO: Identify when any bond is broken in a molecule and don't say its name, or exclude hydrogens??
	private bool hasBroken = false;

	//jointBreak is a flag thats set to true whenever a joint breaks. It's used to determine what happens after a joint breaks(deleting bond, for example)
	//It's set to true again after the routine is finished
	private bool jointBreak = false;

	private int allowedBonds {
		get;
		set;
	}

    SphereCollider col;


    public float speed;

	void Awake () {
        col = gameObject.GetComponent<SphereCollider>();
       // col.radius = 0.5f;

        audioSrc = gameObject.AddComponent <AudioSource> ();
		audioSrc.playOnAwake = false;
		audioSrc.spatialBlend = .73f;


		//Strip off (1) from the Oxygen Atom (1)
		if (gameObject.name.Contains("("))
			gameObject.name = gameObject.name.Substring (0, gameObject.name.IndexOf ("Atom") +4);
		//transform.localScale = new Vector3 (.1f, .1f, .1f);
		int bondNum = 0;

		//TODO: Have method for setting element number, name, bond count and radius
		//TODO: Should the atom prefabs be one prefab?
		if (gameObject.name == "Oxygen Atom") {
			gameObject.transform.localScale = new Vector3(.1f,.1f,.1f);
			//gameObject.transform.localScale *= .1f;
			bondNum = 2;

		} else if (gameObject.name == "Carbon Atom") {
			gameObject.transform.localScale = new Vector3(.1f,.1f,.1f);
			bondNum = 4;

		} else if (gameObject.name == "Nitrogen Atom") {
			gameObject.transform.localScale = new Vector3(.1f,.1f,.1f);
			bondNum = 3;

		} else if (gameObject.name == "Hydrogen Atom") {
			gameObject.transform.localScale = new Vector3(.075f,.075f,.075f);
			bondNum = 1;

		} else if (gameObject.name == "Phosphorus Atom") {
			gameObject.transform.localScale = new Vector3(.1f,.1f,.1f);
			bondNum = 5;

		} else if (gameObject.name == "Sulfur Atom") {
			gameObject.transform.localScale = new Vector3(.1f,.1f,.1f);
			bondNum = 6;
		} 


		allowedBonds = bondNum;

		createStubBonds ();


        //col.radius = 0.5f;


        

    }




	// Update is called once per frame

	void Update (){


		if (jointBreak) {
			//TODO: Change to easier to understand name? EX: breakBonds, figureOutWhichBondHasBrokenAndBreakThem
			bondedAtomsRecalibration ();
		}
      //  col.radius = 0.5f;

    }

    private void LateUpdate()
    {
       // col.radius = 0.5f;

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


        //Checks if the tag of the collided object contains a certain string
        //.contains is used instead of == incase the collided object contains two tags
        //Such as a bullet that is also an atom
		if (obj.tag.Contains( "ExplosiveBullet")) {
			//TODO: Move this to explosiveBulletCollide method, and others
			//TODO: Break single bonds even if double bonds exist?
			if (!hasDoubleBond ()) {
				
				CharacterJoint[] joints = gameObject.GetComponents<CharacterJoint> ();
				for (int i = joints.Length - 1; i >= 0; i--) {
					AtomScript script = joints [i].connectedBody.GetComponent<AtomScript> ();
					script.breakBondWith (gameObject);
					Destroy (joints [i]);
				}



			}
			rb.AddForce (1000f * obj.transform.forward);


			//TODO: Change audio playing to method
			audioSrc.volume = .1f;
			audioSrc.clip = bangSound;
			audioSrc.Play ();

            
            


        } else if (obj.tag.Contains( "Bullet")) {
			audioSrc.volume = .2f;
			audioSrc.clip = bounceSound;
			audioSrc.Play ();

            
            


        } else if (obj.tag.Contains( "Atom")) {


            bondWithAtom(obj);
		}
		
	}


    public void bondWithAtom(GameObject obj){
        AtomScript collidedAtomScript = obj.GetComponent<AtomScript>();

        if (nBondConnections < allowedBonds && collidedAtomScript.nBondConnections < collidedAtomScript.allowedBonds ) {
            if (obj.tag.Contains("Bullet"))
            {
                Slug slugScript = obj.GetComponent<Slug>();
                slugScript.resetToAtom();
            }


            if (bondedAtoms.Contains(obj) == false) {  //Ignore collision with atom I am already bonded to

                GameObject startAtom = gameObject;
                AtomScript startAtomScript = this;
                GameObject endAtom = obj;
                AtomScript endAtomScript = collidedAtomScript;
                
                //Has no bonds
                bool startAtomIsSingletonAtom = startAtomScript.getBondedAtoms().Count == 0;
                bool endAtomIsSingletonAtom = endAtomScript.getBondedAtoms().Count == 0;

                //Temporarily set closestStubBondDistance to be a very large number, one that is likely larger than any of the stub bond distance
                float closestStubBondOnEndAtomDistance = 100000000000;
                int closestStubBondOnEndAtomIndex = 0;

                //Find closest stub bond to start atom from end atom
                for (int i = 0; i < endAtomScript.getStubBondsList().Count; i++)
                {
                    if (Vector3.Distance(startAtom.transform.position, endAtomScript.getStubBondsList()[i].transform.position) < closestStubBondOnEndAtomDistance
                        && endAtomScript.getStubBondsList()[i].activeSelf)
                    {

                        closestStubBondOnEndAtomDistance = Vector3.Distance(startAtom.transform.position, endAtomScript.getStubBondsList()[i].transform.position);
                        closestStubBondOnEndAtomIndex = i;
                    }
                }


                float closestStubBondOnStartAtomDistance = 100000000000;
                int closestStubBondOnStartAtomIndex = 0;

                

                //Find closest stub bond to end atom from start atom
                for (int i = 0; i < startAtomScript.getStubBondsList().Count; i++)
                {
                    if (Vector3.Distance(endAtom.transform.position, startAtomScript.getStubBondsList()[i].transform.position) < closestStubBondOnStartAtomDistance
                        && startAtomScript.getStubBondsList()[i].activeSelf)
                    {

                        closestStubBondOnStartAtomDistance = Vector3.Distance(endAtom.transform.position, startAtomScript.getStubBondsList()[i].transform.position);
                        closestStubBondOnStartAtomIndex = i;
                    }
                }


                Vector3 startAtomOldPosition = startAtom.transform.position;
                Quaternion startAtomOldRotation = startAtom.transform.rotation;

                Vector3 endAtomOldPosition = endAtom.transform.position;
                Quaternion endAtomOldRotation = endAtom.transform.rotation;


                GameObject endAtomStub = endAtomScript.getStubBondsList()[closestStubBondOnEndAtomIndex];
                GameObject startAtomStub = startAtomScript.getStubBondsList()[closestStubBondOnStartAtomIndex];

                
              //  Debug.Log(Vector3.Angle(oldEndAtomStubDirection, oldStartAtomStubDirection));






                if (startAtomIsSingletonAtom)
                {
                    Vector3 directionFromStartToEndAtom = endAtom.transform.position - startAtom.transform.position;
                    Vector3 oldStartAtomStubDirection = (startAtomStub.transform.position - startAtom.transform.position).normalized;
                    startAtom.transform.rotation = Quaternion.FromToRotation(oldStartAtomStubDirection, directionFromStartToEndAtom) * startAtomOldRotation;
                }

                if (endAtomIsSingletonAtom)
                {
                    Vector3 oldEndAtomStubDirection = (endAtomStub.transform.position - endAtom.transform.position).normalized;

                    Vector3 directionFromEndToStartAtom = startAtom.transform.position - endAtom.transform.position;
                    endAtom.transform.rotation = Quaternion.FromToRotation(oldEndAtomStubDirection, directionFromEndToStartAtom) * endAtomOldRotation;
                }

                

                Vector3 endAtomStubDirection = (endAtomStub.transform.position - endAtom.transform.position).normalized;
                
                Vector3 startAtomStubDirection = (startAtomStub.transform.position - startAtom.transform.position).normalized;


                Debug.Log(Vector3.Angle(endAtomStubDirection, startAtomStubDirection));

                
                //Set temporary position of atoms. THis will be their real position once the joints drag them into positioj
                float bondDistanceModifier = (endAtom.transform.localScale.x + startAtom.transform.localScale.x) * (.65f);
                startAtom.transform.position = endAtom.transform.position + bondDistanceModifier  * endAtomStubDirection;
                startAtom.transform.rotation = Quaternion.FromToRotation(startAtomStubDirection, -endAtomStubDirection) * startAtomOldRotation;

                //TODO: THis code not used yet
                int possibleBondsOfThisAtom = allowedBonds - bondedAtoms.Count;
				int possibleBondsOfCollidedAtom = collidedAtomScript.allowedBonds - collidedAtomScript.bondedAtoms.Count;
				int newBondOrder = Mathf.Min (possibleBondsOfThisAtom, possibleBondsOfCollidedAtom);
                //TODO ends here----

				GameObject newBond = Instantiate (bondGameObject, gameObject.transform.position, Quaternion.identity);
                newBond.GetComponent<Bond> ().formBond (startAtom, endAtom, 1);

                startAtom.transform.position = startAtomOldPosition;

                startAtom.transform.rotation = startAtomOldRotation;


            }
        }
    }


	void OnJointBreak(float breakforce) {
        // Debug.Log(gameObject.name + " broke");

        hasBroken = true;
		jointBreak = true;

		audioSrc.volume = .1f;
		audioSrc.clip = snapSound;
		audioSrc.Play ();
	}
		


	public void breakBondWith(GameObject atom){ 
		bondedAtoms.Remove (atom);
		CharacterJoint[] joints = gameObject.GetComponents<CharacterJoint> ();
		for (int i = 0; i < joints.Length; i++) {
			if (joints[i].connectedBody.gameObject == atom) {
				Destroy (joints[i]);
			}
		}

        OnJointBreak(0f);


	}

	//TODO: Atoms are not placed in bondedAtoms twice, so this will never work
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


	//Figure out which joints broke and break bonds
	private void bondedAtomsRecalibration(){

        GameObject brokenBond = null;
		GameObject brokenBondWithAtom = null;
		List<GameObject> currentJointedAtoms = new List<GameObject> ();
		List<GameObject> currentBondedAtoms = new List<GameObject> ();
		CharacterJoint[] currentJoints = gameObject.GetComponents<CharacterJoint> ();

		//TODO: Loop over bonds and call b.deleteBondIfJointBroken 

		//Find which joint broke
		for (int i = 0; i < currentJoints.Length; i++) {
			currentJointedAtoms.Add (currentJoints [i].connectedBody.gameObject);
            //currentJoints[i].breakForce = 1000f/*Mathf.Infinity*/;
		}

		for (int i = 0; i < bonds.Count; i++) {
			GameObject[] bondConnections = bonds[i].GetComponent<Bond>().getConnectedAtoms();
			foreach (GameObject atom in bondConnections) {
				if (atom != gameObject)
					currentBondedAtoms.Add (atom);
			}
		}

		//Find bond with no joint
		//TODO: What if two bonds have no joint?
		for (int i = 0; i < currentBondedAtoms.Count; i++) {
            if (currentJointedAtoms.Contains(currentBondedAtoms[i]) == false)
            {
                brokenBondWithAtom = currentBondedAtoms[i];

            }

        }

		for (int i = 0; i < bonds.Count; i++) {
			GameObject[] bondConnections = bonds[i].GetComponent<Bond>().getConnectedAtoms();
			if (brokenBondWithAtom != null && (bondConnections [0] == brokenBondWithAtom  || bondConnections [1] == brokenBondWithAtom) )
				brokenBond = bonds [i];
		}


		//Break the bonds
		if (brokenBond != null) {

            

			breakBond (brokenBond);
			brokenBondWithAtom.GetComponent<AtomScript> ().breakBond (brokenBond);

			bondedAtoms.Remove (brokenBondWithAtom);
			brokenBondWithAtom.GetComponent<AtomScript> ().breakBondWith (gameObject);

			Destroy (brokenBond);
           


        } else
        {
           jointBreak= false;
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

    //Sets tetrahedral direction of stub bonds
	private void createStubBonds(){
		rb = gameObject.GetComponent<Rigidbody> ();

		for (int i = 0; i < allowedBonds; i++) {
			Vector3 posMod; //direction bond points in

			if (i == 0) {
				posMod = new Vector3 (1, 0, -1 / Mathf.Sqrt (2));

			} else if (i == 1) {
				posMod =  new Vector3 (-1, 0, -1 / Mathf.Sqrt(2));

			} else if (i == 2) {
				posMod =  new Vector3 (0, 1, 1 / Mathf.Sqrt (2));

			} else if (i == 3) {
				posMod =  new Vector3 (0, -1, 1 / Mathf.Sqrt (2));

			} else {
				posMod = Random.onUnitSphere;
			}

            //Creates stub bonds a small distance away from atom
			GameObject newStubBond = Instantiate (stubBondGameObject, posMod * transform.localScale.x * .5f + transform.position, Quaternion.identity);
			newStubBond.transform.parent = transform;
			stubBonds.Add (newStubBond);
			Bond bondScript = newStubBond.GetComponent<Bond> ();
			bondScript.setInitialAtom (gameObject);
            bondScript.setBondStubDirection(transform.InverseTransformDirection(newStubBond.transform.position - transform.position).normalized);
        }
	}


	//Hides stub bonds and returns a list of the stub bonds that were hidden
	public List<GameObject> makeBondConnection(GameObject atomToBond,GameObject bond, int bondOrder){
		addBondedAtom (atomToBond);
		bonds.Add (bond);

		List<GameObject> stubBondReference = new List<GameObject> (); //Hidden stub bonds



		for (int i = 0; i < bondOrder; i++) {
			float bondDistance = 10000f;
			GameObject closestBondStub = null;
			foreach (GameObject bondStub in stubBonds) {
				Bond tempBondScript = bondStub.GetComponent<Bond> ();
				if (Vector3.Distance (bondStub.transform.position, atomToBond.transform.position) < bondDistance) {
					if (bondStub.activeSelf && stubBondReference.Contains(bondStub) == false) {
						bondDistance = Vector3.Distance (bondStub.transform.position, atomToBond.transform.position);
						closestBondStub = bondStub;
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

	//Show hidden stubs
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

	public void breakBond(GameObject bond){
		respawnStubs (bond);
		nBondConnections -= bond.GetComponent<Bond> ().getOrder ();
		removeBond (bond);
	}

	public void removeBond(GameObject bond){
		bonds.Remove (bond);
	}

    public List<GameObject> getStubBondsList()
    {
        return stubBonds;
    }

}

