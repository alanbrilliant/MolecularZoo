using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleculeCreator : MonoBehaviour {


	public GameObject hydrogenPrefab;
	public GameObject oxygenPrefab;
	public GameObject carbonPrefab;
	public GameObject nitrogenPrefab;
	public GameObject phosphorusPrefab;

	public GameObject bondModel;

	private float molSize = .1f;
	// Use this for initialization

	public void instantiateMolecule(MoleculeData molecule, Vector3 startingPos){
		molecule.ToString ();



		List<int> atomList = molecule.atom.element;
		List<int> bondStart = molecule.bond.aid1;
		List<int> bondEnd = molecule.bond.aid2;
		List<int> bondOrder = molecule.bond.order;
		List<float> xCoords = molecule.conf.x;
		List<float> yCoords = molecule.conf.y;
		List<float> zCoords = molecule.conf.z;

		string moleculeName = molecule.name;

		List<GameObject> instantiatedAtoms = new List<GameObject> ();

		GameObject parentMol = new GameObject ();
		parentMol.transform.position = Vector3.zero;
		parentMol.name = moleculeName;


		for (int i = 0; i < atomList.Count; i++) {
			GameObject atomPrefab = null;
			if (atomList [i] == 8) {
				atomPrefab = oxygenPrefab;

			}
			if (atomList [i] == 1) {
				atomPrefab = hydrogenPrefab;
			}

			if (atomList [i] == 6) {
				atomPrefab = carbonPrefab;
			}
			if (atomList [i] == 7) {
				atomPrefab = nitrogenPrefab;
			}
			if (atomList [i] == 15) {
				atomPrefab = phosphorusPrefab;
			} 


			GameObject newAtom = GameObject.Instantiate (atomPrefab, new Vector3 (xCoords [i]+ startingPos.x, yCoords [i] + startingPos.y, zCoords [i] + startingPos.z) * molSize, Quaternion.identity);
			//newAtom.transform.localScale *= molSize;
			newAtom.transform.parent = parentMol.transform;
			instantiatedAtoms.Add(newAtom);


		
		}
		for (int i = 0; i < bondStart.Count; i++) {
			int startAtomID = bondStart [i];
			int endAtomID = bondEnd [i];


			GameObject startAtom = instantiatedAtoms [startAtomID - 1];
			GameObject endAtom = instantiatedAtoms [endAtomID - 1];

			AtomScript startScript = startAtom.GetComponent<AtomScript> ();
			AtomScript endScript = endAtom.GetComponent<AtomScript> ();

			//Ray bondLine = new Ray (startAtom.transform.position, endAtom.transform.position);
			//Vector3 bondPos = bondLine.GetPoint (Vector3.Distance (startAtom.transform.position, endAtom.transform.position));
			Vector3 bondPos = (startAtom.transform.position + endAtom.transform.position) /2;

			Rigidbody startAtomRB = startAtom.GetComponent<Rigidbody> ();
			Rigidbody endAtomRB = endAtom.GetComponent<Rigidbody> ();


			for (int j = 0; j < bondOrder [i]; j++) {
				CharacterJoint joint1 = startAtom.AddComponent<CharacterJoint> ();
				joint1.connectedBody = endAtomRB;

				startScript.addBondedAtom (endAtom);

				CharacterJoint joint2 = endAtom.AddComponent<CharacterJoint> ();
				joint2.connectedBody = startAtomRB;
				endScript.addBondedAtom (startAtom);

				List<GameObject> startAtomBonds = startScript.getBonds ();
				for (int k = 0; k < startAtomBonds.Count; k++) {
					
					Bond bondScript = startAtomBonds [k].GetComponent<Bond> ();
					GameObject[] newBondConnectedAtoms = bondScript.getConnectedAtoms ();
					if (newBondConnectedAtoms [1] == null) {
						bondScript.setConnectedAtom (endAtom);

						if (bondOrder [i] == 1) {
							bondScript.setOrderModifier (0);
						} else if (bondOrder [i] == 2) {
							if (j == 0)
								bondScript.setOrderModifier (1);
							if (j == 1)
								bondScript.setOrderModifier (-1);
						} else if (bondOrder [i] == 3) {
							if (j == 0)
								bondScript.setOrderModifier (0);
							if (j == 1)
								bondScript.setOrderModifier (-1);
							if (j == 2)
								bondScript.setOrderModifier (1);
						}
						break;
					}
				
				}

				List<GameObject> endAtomBonds = endScript.getBonds ();


				for (int k = 0; k < endAtomBonds.Count; k++) {

					Bond bondScript = endAtomBonds [k].GetComponent<Bond> ();
					GameObject[] newBondConnectedAtoms = bondScript.getConnectedAtoms ();
					if (newBondConnectedAtoms [1] == null) {
						bondScript.setConnectedAtom (startAtom);

						if (bondOrder [i] == 1) {
							bondScript.setOrderModifier (0);
						} else if (bondOrder [i] == 2) {
							if (j == 0)
								bondScript.setOrderModifier (1);
							if (j == 1)
								bondScript.setOrderModifier (-1);
						} else if (bondOrder [i] == 3) {
							if (j == 0)
								bondScript.setOrderModifier (0);
							if (j == 1)
								bondScript.setOrderModifier (-1);
							if (j == 2)
								bondScript.setOrderModifier (1);
						}
						break;
					}

				}


				//Bond bondScript = newBond.GetComponent<Bond> ();
				//bondScript.setConnectedAtoms (startAtom, endAtom);
				//newBond.transform.parent = parentMol.transform;


						
					


			}




		}



		for (int i = 0; i < instantiatedAtoms.Count; i++) {
			CharacterJoint[] atomJoints = instantiatedAtoms [i].GetComponents<CharacterJoint> ();
			for (int j = 0; j < atomJoints.Length; j++) {
				atomJoints [j].breakForce = 5000f;
			}
		}


		
	}

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
