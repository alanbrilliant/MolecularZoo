using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Bond : MonoBehaviour {

	public GameObject[] connectedAtoms = new GameObject[2];
	private int orderModifier = 0;
	private Quaternion rotation;
	public GameObject bondShared;
	public Vector3 posMod;
	public List<GameObject> stubBondReferenceStartAtom;
	public List<GameObject> stubBondReferenceEndAtom;

	bool stuffSet = false;
	float time = 0;
	// Use this for initialization
	void Start () {
		posMod = transform.localPosition   * transform.parent.transform.localScale.x * .5f;
	}
	
	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;
		GameObject startAtom = connectedAtoms [0];
		GameObject endAtom = connectedAtoms[1];
		if (connectedAtoms [0] != null && connectedAtoms [1] != null) {
			Vector3 bondDirection = (startAtom.transform.position - endAtom.transform.position).normalized;
			Quaternion bondDirectionQuat = Quaternion.LookRotation (bondDirection);
			//Ray bondLine = new Ray (startAtom.transform.position, endAtom.transform.position);
			//Vector3 bondPos = bondLine.GetPoint (Vector3.Distance (startAtom.transform.position, endAtom.transform.position));
			//if (time >= .05f) {  
				Vector3 bondPos = (startAtom.transform.position + endAtom.transform.position) / 2;
				transform.rotation = bondDirectionQuat;
				transform.position = bondPos;
				//transform.localPosition = transform.localPosition + new Vector3 (0, orderModifier * .07f, 0);
				time = 0;
			//}


		} else if (connectedAtoms [0] != null && connectedAtoms [1] == null) {

			if (Vector3.Distance (connectedAtoms [0].transform.position, transform.position) > connectedAtoms [0].transform.localScale.x * .5) {
				//transform.Translate ((connectedAtoms [0].transform.position - transform.position).normalized * .01f);
				//Debug.Log(posMod);
				transform.position = Vector3.MoveTowards(transform.position,connectedAtoms[0].transform.position + posMod,.01f);
			}
			transform.rotation = Quaternion.LookRotation((connectedAtoms[0].transform.position- transform.position).normalized);

			//transform.position = connectedAtoms [0].transform.position + connectedAtoms [0].transform.localScale;
			
		
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

	public void formBond(GameObject startAtom, GameObject endAtom,int bondOrder){
		Rigidbody endAtomRB = endAtom.GetComponent<Rigidbody> ();
		Rigidbody startAtomRB = startAtom.GetComponent<Rigidbody> ();

		CharacterJoint joint1 = startAtom.AddComponent<CharacterJoint> ();
		joint1.connectedBody = endAtomRB;

		joint1.breakForce = 1000f * bondOrder;
	

		CharacterJoint joint2 = endAtom.AddComponent<CharacterJoint> ();
		joint2.connectedBody = startAtomRB;

		joint2.breakForce = 1000f * bondOrder;

		setConnectedAtoms (startAtom, endAtom);

		orderModifier = bondOrder;
		stubBondReferenceStartAtom = startAtom.GetComponent<AtomScript> ().makeBondConnection (endAtom, gameObject, bondOrder);
		stubBondReferenceStartAtom.Add (startAtom);

		stubBondReferenceEndAtom = endAtom.GetComponent<AtomScript> ().makeBondConnection (startAtom, gameObject, bondOrder);
		stubBondReferenceEndAtom.Add (endAtom);
		//gameObject.transform.parent = startAtom.transform;


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
}
