using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleculeCreator : MonoBehaviour {


	public GameObject hydrogenPrefab;
	public GameObject oxygenPrefab;
	public GameObject carbonPrefab;
	public GameObject nitrogenPrefab;
	public GameObject phosphorusPrefab;

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

			if (bondOrder [i] == 1) {
				CharacterJoint joint1 = startAtom.AddComponent<CharacterJoint> ();
				joint1.connectedBody = endAtom.GetComponent<Rigidbody> ();

				startScript.addBondedAtom (endAtom);

				CharacterJoint joint2 = endAtom.AddComponent<CharacterJoint> ();
				joint2.connectedBody = startAtom.GetComponent<Rigidbody> ();
				endScript.addBondedAtom (startAtom);

				/*
				JointLimits limits = joint1.limits;
				limits.max = 5;

				joint1.limits = limits;
				joint2.limits = limits;
				joint1.useLimits = true;
				joint2.useLimits = true;
				*/
			} else if (bondOrder [i] == 2 ) {
				
				CharacterJoint joint1 = startAtom.AddComponent<CharacterJoint> ();
				joint1.connectedBody = endAtom.GetComponent<Rigidbody> ();
				CharacterJoint joint2 = startAtom.AddComponent<CharacterJoint> ();
				joint2.connectedBody = endAtom.GetComponent<Rigidbody> ();
				startScript.addBondedAtom (endAtom);
				startScript.addBondedAtom (endAtom);


				CharacterJoint joint3 = endAtom.AddComponent<CharacterJoint> ();
				joint3.connectedBody = startAtom.GetComponent<Rigidbody> ();
				CharacterJoint joint4 = endAtom.AddComponent<CharacterJoint> ();
				joint4.connectedBody = startAtom.GetComponent<Rigidbody> ();
				endScript.addBondedAtom (startAtom);
				endScript.addBondedAtom (startAtom);


			
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
