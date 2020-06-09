
using System.Collections;
using System.Collections.Generic;

using System.Linq;

using UnityEngine;

public class AtomScript : MonoBehaviour { //TODO: Change AtomScript to Atom

    public int atomicNumber;

	private Rigidbody rb;

    public GameObject bondGameObject;
    public GameObject doubleBondGameObject;
	public GameObject stubBondGameObject;

	//TODO: change GameObject to AtomScript in List<>
	public List<GameObject> bondedAtoms = new List<GameObject>();



	private List<GameObject> bonds = new List<GameObject> ();

	//Stub bond cylanders depict where bonds can form
	private List<GameObject> stubBonds = new List<GameObject> ();

	public int nBondConnections = 0;
    private int availableBonds;

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

        //Maps the atomic number to the element name
        Dictionary<int, string> elementNameMap = new Dictionary<int, string>{
            { 1,"Hydrogen" },
            {3, "Lithium" },
            {4, "Carbon" },
            {5, "Nitrogen"},
            {6, "Oxygen" },
            {7, "Fluorine" },
            {11, "Sodium" },
            {13, "Aluinum" },
            {15, "Phosphorus" },
            {16, "Sulfur" },
            {17, "Chlorine" },
            {26, "Iron" }

        };

        //maps the atomic number to the elemental atom radius
        Dictionary<int, float> elementAtomRadiusMap = new Dictionary<int, float>{
            { 1, .075f },
            { 3, .1f },
            { 4, .1f },
            { 5, .1f},
            { 6, .1f },
            { 7, .1f },
            { 11, .1f },
            { 13, .1f },
            { 15, .1f },
            { 16, .1f },
            { 17, .1f },
            { 26, .1f}
        };
        
        //Maps the atomic number to the number of allowed bonds for each element
        Dictionary<int, int> elementBondNumMap = new Dictionary<int, int> {
            { 1, 1 },
            { 3, 1 },
            { 4, 4 },
            { 5, 3},
            { 6, 2 },
            { 7, 1 },
            { 11, 1 },
            { 13, 3 },
            { 15, 5 },
            { 16, 2 },
            { 17, 1 },
            { 26, 3}
        };
        col = gameObject.GetComponent<SphereCollider>();
       // col.radius = 0.5f;

        audioSrc = gameObject.AddComponent <AudioSource> ();
		audioSrc.playOnAwake = false;
		audioSrc.spatialBlend = .73f;


		int bondNum = 0;

        
        //TODO: Should the atom prefabs be one prefab?

        gameObject.name = elementNameMap[atomicNumber] + " Atom";
        bondNum = elementBondNumMap[atomicNumber];
        gameObject.transform.localScale = new Vector3(elementAtomRadiusMap[atomicNumber], elementAtomRadiusMap[atomicNumber], elementAtomRadiusMap[atomicNumber]);

        allowedBonds = bondNum;

		createStubBonds ();
        availableBonds = allowedBonds;


    }


	void Update (){


		if (jointBreak) {
            figureOutWhichBondHasBrokenAndBreakThem();
		}

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

            
            


        } else if (obj.tag.Contains("Atom")) {

            
            bondWithAtom(obj, 1);
		}
		
	}


    public void bondWithAtom(GameObject obj, int bondOrder){
        AtomScript collidedAtomScript = obj.GetComponent<AtomScript>();
        if (nBondConnections < allowedBonds && collidedAtomScript.nBondConnections < collidedAtomScript.allowedBonds ) {
            //Debug.Log(obj.tag.Contains("Bullet"));
            //Debug.Log(obj.tag);

            if (obj.tag.Contains("Bullet"))
            {
                Slug slugScript = obj.GetComponent<Slug>();
                slugScript.resetToAtom();
            }
            else if (tag.Contains("Bullet"))
            {
                Slug slugScript = GetComponent<Slug>();
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




                Vector3 startAtomOldPosition = startAtom.transform.position;
                Quaternion startAtomOldRotation = startAtom.transform.rotation;

                Vector3 endAtomOldPosition = endAtom.transform.position;
                Quaternion endAtomOldRotation = endAtom.transform.rotation;


                GameObject endAtomStub = closestStubBond(endAtomScript, startAtomScript); //endAtomScript.getStubBondsList()[closestStubBondOnEndAtomIndex];
                GameObject startAtomStub = closestStubBond(startAtomScript, endAtomScript);




                Vector3 endAtomStubDirection = (endAtomStub.transform.position - endAtom.transform.position).normalized;

                Vector3 startAtomStubDirection = (startAtomStub.transform.position - startAtom.transform.position).normalized;

                /*
                //Checks to see if a double bond should be formed
                int possibleBondsOfThisAtom = allowedBonds - nBondConnections;
                int possibleBondsOfCollidedAtom = collidedAtomScript.allowedBonds - collidedAtomScript.nBondConnections;

                //selects the lowest number of open bonds
                int newBondOrder = Mathf.Min(possibleBondsOfThisAtom, possibleBondsOfCollidedAtom);
                //If the lowest number of open bonds is greater than 2, then it will default to 2, since triple bonds haven't been added yet
                if (newBondOrder > 2)
                    newBondOrder = 2;*/

                //bondPrefab is the placeholder for either a single or double bond
                GameObject bondPrefab;

               int  newBondOrder = bondOrder;
                if (newBondOrder == 1)
                {
                    bondPrefab = bondGameObject;
                }
                else if (newBondOrder == 2)
                {
                    bondPrefab = doubleBondGameObject;
                    GameObject endAtomSecondClosestStub = closestStubBond(endAtomScript, startAtomScript, endAtomStub);
                    GameObject startAtomSecondClosestStub = closestStubBond(startAtomScript, endAtomScript, startAtomStub);

                    endAtomStubDirection = ((endAtomStub.transform.position + endAtomSecondClosestStub.transform.position) / 2 - endAtom.transform.position).normalized;

                    startAtomStubDirection = ((startAtomStub.transform.position + startAtomSecondClosestStub.transform.position) / 2 - startAtom.transform.position).normalized;



                }
                else
                {
                    bondPrefab = bondGameObject;
                }


                if (startAtomIsSingletonAtom)
                {
                    Vector3 directionFromStartToEndAtom = endAtom.transform.position - startAtom.transform.position;
                    //Vector3 oldStartAtomStubDirection = (startAtomStub.transform.position - startAtom.transform.position).normalized;

                    startAtom.transform.rotation = Quaternion.FromToRotation(startAtomStubDirection, directionFromStartToEndAtom) * startAtomOldRotation;
                }

                if (endAtomIsSingletonAtom)
                {
                    Vector3 directionFromEndToStartAtom = startAtom.transform.position - endAtom.transform.position;
                    // Vector3 oldEndAtomStubDirection = (endAtomStub.transform.position - endAtom.transform.position).normalized;

                    endAtom.transform.rotation = Quaternion.FromToRotation(endAtomStubDirection, directionFromEndToStartAtom) * endAtomOldRotation;
                }



                //reset the bond directions

                if (newBondOrder == 2)
                {
                    GameObject endAtomSecondClosestStub = closestStubBond(endAtomScript, startAtomScript, endAtomStub);
                    GameObject startAtomSecondClosestStub = closestStubBond(startAtomScript, endAtomScript, startAtomStub);
                    endAtomStubDirection = ((endAtomStub.transform.position + endAtomSecondClosestStub.transform.position) / 2 - endAtom.transform.position).normalized;

                    startAtomStubDirection = ((startAtomStub.transform.position + startAtomSecondClosestStub.transform.position) / 2 - startAtom.transform.position).normalized;

                } else if (newBondOrder == 1){
                    endAtomStubDirection = (endAtomStub.transform.position - endAtom.transform.position).normalized;
                    startAtomStubDirection = (startAtomStub.transform.position - startAtom.transform.position).normalized;
                }




                //Set temporary position of atoms. This will be their real position once the joints drag them into position
                float bondDistanceModifier = (endAtom.transform.localScale.x + startAtom.transform.localScale.x) * (.65f);
                startAtom.transform.position = endAtom.transform.position + bondDistanceModifier  * endAtomStubDirection;
                startAtom.transform.rotation = Quaternion.FromToRotation(startAtomStubDirection, -endAtomStubDirection) * startAtomOldRotation;

                

                //The new bond is instantiated, and then formBond is called on the two connected atoms (along with bondOrder, which says whether the bond is a single, double, or triple bond)
				GameObject newBond = Instantiate (bondPrefab, gameObject.transform.position, Quaternion.identity);
                newBond.GetComponent<Bond> ().formBond (startAtom, endAtom, newBondOrder);

                startAtom.transform.position = startAtomOldPosition;

                startAtom.transform.rotation = startAtomOldRotation;


            }
        }
    }


    //Finds closest stub bond on atom1 to atom2
    //Excludes "exclude" object from search
    private GameObject closestStubBond(AtomScript atom1, AtomScript atom2, GameObject exclude) {

        //Temporarily set closestStubBondDistance to be a very large number, one that is likely larger than any of the stub bond distance
        float closestStubBondDistance = 10000000000;
        int closestStubBondIndex = 0;

        List<GameObject> atom1StubBonds = atom1.getStubBondsList();

        for (int i = 0; i < atom1.getStubBondsList().Count; i++)
        {
            if (Vector3.Distance(atom2.transform.position, atom1StubBonds[i].transform.position) < closestStubBondDistance
                && atom1StubBonds[i].activeSelf)
            {
                if (atom1StubBonds[i] != exclude)
                {
                    closestStubBondDistance = Vector3.Distance(atom2.transform.position, atom1StubBonds[i].transform.position);
                    closestStubBondIndex = i;
                }
            }
        }
        return atom1StubBonds[closestStubBondIndex];
    }

    //Overloads closestStubBond method without exclude parameter
    private GameObject closestStubBond(AtomScript atom1, AtomScript atom2) {
       return closestStubBond(atom1, atom2, null);
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
        /*
        GameObject brokenBond = null;

        for (int i = 0; i < bonds.Count; i++)
        {
            GameObject[] bondConnections = bonds[i].GetComponent<Bond>().getConnectedAtoms();
            if (atom != null && (bondConnections[0] == atom || bondConnections[1] == atom))
                brokenBond = bonds[i];
        }

     //   OnJointBreak(0f);
        if (brokenBond != null)
        {
            breakBond(brokenBond);
            atom.GetComponent<AtomScript>().breakBond(brokenBond);
            bondedAtoms.Remove(atom);
            atom.GetComponent<AtomScript>().breakBondWith(gameObject);


            Destroy(brokenBond);
        } else
        {
            jointBreak = false;
        }*/




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
			else
			{
				print("this atom has no sound");
			}
		}
		else
		{
			print("this atom broke off of the molecule, thus no sound");
		}
	}

	public AudioClip getMoleculeNameSound(){
		return moleculeNameSound;
	}


	//Figure out which joints broke and break bonds
	private void figureOutWhichBondHasBrokenAndBreakThem(){

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
        for (int i = 0; i < bonds.Count; i++)
        {
            GameObject[] bondConnections = bonds[i].GetComponent<Bond>().getConnectedAtoms();
            if (brokenBondWithAtom != null && (bondConnections[0] == brokenBondWithAtom || bondConnections[1] == brokenBondWithAtom))
                brokenBond = bonds[i];
        }

        //Break the bonds
        if (brokenBond != null)
        {

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

            if (closestBondStub != null)
            {
                stubBondReference.Add(closestBondStub);
            }

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

    public bool hasOpenBonds() {
        if (nBondConnections < allowedBonds)
        {
            return true;
        } else
        {
            return false;
        }
    }
    
}

