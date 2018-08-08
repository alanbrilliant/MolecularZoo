using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Bond : MonoBehaviour {

	public GameObject[] connectedAtoms = new GameObject[2];
	private int orderModifier = 0;
	private Quaternion rotation;
	public GameObject bondShared;
	public Vector3 bondStubDirection;
	public List<GameObject> stubBondReferenceStartAtom;
	public List<GameObject> stubBondReferenceEndAtom;
 
	bool stuffSet = false;
	float time = 0;
	void Start () {
       
    }
	
	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;
		GameObject startAtom = connectedAtoms [0];
		GameObject endAtom = connectedAtoms[1];

        //if the connected atoms of this stub are both not null, then the stub acts as a bond between two atoms
		if (connectedAtoms [0] != null && connectedAtoms [1] != null) {
			Vector3 bondDirection = (startAtom.transform.position - endAtom.transform.position).normalized;
			Quaternion bondDirectionQuat = Quaternion.LookRotation (bondDirection);
			Vector3 bondPos = (startAtom.transform.position + endAtom.transform.position) / 2;
			transform.rotation = bondDirectionQuat;
			transform.position = bondPos;
			time = 0;
			


        //If connectedAtoms[1] is null, then the stub isn't connected to anything, and is only a stub
        //ConnectedAtoms[0] is the bond's source atom. It should never be null
		} else if (connectedAtoms [0] != null && connectedAtoms [1] == null) {
            Vector3 posMod = connectedAtoms[0].transform.TransformVector(bondStubDirection);
			if (Vector3.Distance (connectedAtoms [0].transform.position + posMod, transform.position) > connectedAtoms [0].transform.localScale.x / 10) {

				transform.position = Vector3.MoveTowards(transform.position,connectedAtoms[0].transform.position + posMod/2,.01f);
			}
			transform.rotation = Quaternion.LookRotation((connectedAtoms[0].transform.position- transform.position).normalized);
           			
		
		}else
        {
            Debug.Log("Something is wrong. This isn't supposed to happen");
        }


	}

	public void setConnectedAtoms(GameObject atom1, GameObject atom2){
		connectedAtoms [0] = atom1;
		connectedAtoms [1] = atom2;
	}

	public void setInitialAtom(GameObject atom){
		connectedAtoms [0] = atom;
	}

	public void setConnectedAtom(GameObject atom){
		connectedAtoms [1] = atom;
	}

	public GameObject[] getConnectedAtoms(){
		return connectedAtoms;
	}


	public void clearConnectedAtoms(){
		Array.Clear (connectedAtoms, 0, 2);
	}

	public void setOrderModifier(int ordMod){
		orderModifier = ordMod;
	}

    //Form bonds where the two atoms currently are
	public void formBond(GameObject startAtom, GameObject endAtom,int bondOrder){
		Rigidbody endAtomRB = endAtom.GetComponent<Rigidbody> ();
		Rigidbody startAtomRB = startAtom.GetComponent<Rigidbody> ();

        AtomScript endAtomScript = endAtom.GetComponent<AtomScript>();


        


        CharacterJoint joint1 = startAtom.AddComponent<CharacterJoint> ();
		joint1.connectedBody = endAtomRB;

		joint1.breakForce = 1000f * bondOrder;

        SoftJointLimit jointLowLimit = new SoftJointLimit();
        jointLowLimit.limit = -180;

        SoftJointLimit jointHighLimit = new SoftJointLimit();
        jointHighLimit.limit = 180;

        

		CharacterJoint joint2 = endAtom.AddComponent<CharacterJoint> ();
		joint2.connectedBody = startAtomRB;

		joint2.breakForce = 1000f * bondOrder;

		setConnectedAtoms (startAtom, endAtom);


        if (bondOrder == 1)
        {
            /*
            joint1.lowTwistLimit = jointLowLimit;
            joint1.highTwistLimit = jointHighLimit;

            joint2.lowTwistLimit = jointLowLimit;
            joint2.highTwistLimit = jointHighLimit;*/
            //joint1.swing2Limit = jointHighLimit;
            //joint2.swing2Limit = jointHighLimit;

            //joint1.swing1Limit = jointHighLimit;
            //joint2.swing1Limit = jointHighLimit;

        }

        orderModifier = bondOrder;
		stubBondReferenceStartAtom = startAtom.GetComponent<AtomScript> ().makeBondConnection (endAtom, gameObject, bondOrder);
		stubBondReferenceStartAtom.Add (startAtom);

		stubBondReferenceEndAtom = endAtom.GetComponent<AtomScript> ().makeBondConnection (startAtom, gameObject, bondOrder);
		stubBondReferenceEndAtom.Add (endAtom);
		//gameObject.transform.parent = startAtom.transform;


	}

    //Form bond at specific location of start and end atom
    public void formBond(GameObject startAtom, GameObject endAtom, int bondOrder,Vector3 startAtomJointLocation, Vector3 endAtomJointLocation) {
        Rigidbody endAtomRB = endAtom.GetComponent<Rigidbody>();
        Rigidbody startAtomRB = startAtom.GetComponent<Rigidbody>();

        AtomScript endAtomScript = endAtom.GetComponent<AtomScript>();





        CharacterJoint joint1 = startAtom.AddComponent<CharacterJoint>();
        joint1.connectedBody = endAtomRB;

        joint1.breakForce = 1000000f * bondOrder;
        joint1.autoConfigureConnectedAnchor = false;
        joint1.connectedAnchor = endAtomJointLocation;
        
       
        
        CharacterJoint joint2 = endAtom.AddComponent<CharacterJoint>();
        joint2.connectedBody = startAtomRB;

        joint2.breakForce = 1000000f * bondOrder;
        joint2.autoConfigureConnectedAnchor = false;
        joint2.connectedAnchor = startAtomJointLocation;


        setConnectedAtoms(startAtom, endAtom);

        orderModifier = bondOrder;
        stubBondReferenceStartAtom = startAtom.GetComponent<AtomScript>().makeBondConnection(endAtom, gameObject, bondOrder);
        stubBondReferenceStartAtom.Add(startAtom);

        stubBondReferenceEndAtom = endAtom.GetComponent<AtomScript>().makeBondConnection(startAtom, gameObject, bondOrder);
        stubBondReferenceEndAtom.Add(endAtom);
    }

    


	public List<GameObject> getStubBondReferences(GameObject parentAtom){

		if (stubBondReferenceStartAtom [stubBondReferenceStartAtom.Count - 1] == parentAtom) {
			return stubBondReferenceStartAtom;

		} else if (stubBondReferenceEndAtom [stubBondReferenceEndAtom.Count - 1] == parentAtom) {
			return stubBondReferenceEndAtom;
		} else {
			Debug.Log (stubBondReferenceStartAtom [stubBondReferenceStartAtom.Count - 1] + " || " + parentAtom);
			Debug.Log (stubBondReferenceEndAtom [stubBondReferenceEndAtom.Count - 1] + " || " + parentAtom);
			Debug.LogError ("Tried to get stub reference from bond, but stub reference did not exist, or was not called properly");
			throw new Exception ();
		}
	
	}

	public int getOrder(){
		return orderModifier;
	}

    //Direction of the bond stub
    public void setBondStubDirection(Vector3 newPosMod) {

        bondStubDirection =  newPosMod;
    }
}
