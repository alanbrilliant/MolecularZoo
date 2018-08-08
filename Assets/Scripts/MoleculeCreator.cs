using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleculeCreator : MonoBehaviour
{


    public GameObject hydrogenPrefab;
    public GameObject oxygenPrefab;
    public GameObject carbonPrefab;
    public GameObject nitrogenPrefab;
    public GameObject phosphorusPrefab;
    public GameObject sulfurPrefab;
    public GameObject chlorinePrefab;
    public GameObject fluorinePrefab;
    public GameObject ironPrefab;
    public GameObject sodiumPrefab;
    public GameObject aluminiumPrefab;
    public GameObject lithiumPrefab;


    public GameObject bondModel;
    public GameObject doubleBondModel;

    public List<AudioClip> moleculeNameSounds;

    private float molSize = .1f;
    // Use this for initialization

    public void instantiateMolecule(MoleculeData molecule, Vector3 startingPos)
    {
        Debug.Log(startingPos);
        Debug.Log(molecule.ToString ());
        //Debug.Log(molecule);


        List<int> atomList = molecule.atom.element;
        List<int> bondStart = molecule.bond.aid1;
        List<int> bondEnd = molecule.bond.aid2;
        List<int> bondOrder = molecule.bond.order;
        List<float> xCoords = molecule.conf.x;
        List<float> yCoords = molecule.conf.y;
        List<float> zCoords = molecule.conf.z;

        string moleculeName = molecule.name;

        List<GameObject> instantiatedAtoms = new List<GameObject>();

        GameObject parentMol = new GameObject();
        parentMol.transform.position = Vector3.zero;
        parentMol.name = moleculeName;


        for (int i = 0; i < atomList.Count; i++)
        {
            GameObject atomPrefab = null;
            if (atomList[i] == 8)
            {
                atomPrefab = oxygenPrefab;

            }
            else if (atomList[i] == 1)
            {
                atomPrefab = hydrogenPrefab;
            }
            else if (atomList[i] == 3)
            {
                atomPrefab = lithiumPrefab;
            }

            else if (atomList[i] == 6)
            {
                atomPrefab = carbonPrefab;
            }
            else if (atomList[i] == 7)
            {
                atomPrefab = nitrogenPrefab;
            }
            else if (atomList[i] == 9)
            {
                atomPrefab = fluorinePrefab;
            }
            else if (atomList[i] == 11)
            {
                atomPrefab = sodiumPrefab;
            }
            else if (atomList[i] == 13)
            {
                atomPrefab = aluminiumPrefab;
            }
            else if (atomList[i] == 15)
            {
                atomPrefab = phosphorusPrefab;
            }
            else if (atomList[i] == 16)
            {
                atomPrefab = sulfurPrefab;
            }
            else if (atomList[i] == 17)
            {
                atomPrefab = chlorinePrefab;
            }
            else if (atomList[i] == 26)
            {
                atomPrefab = ironPrefab;
            }
            else
            {
                Debug.Log("Trying to instantiate atom without prefab. Atomic Number: " + atomList[i]);
            }

            Debug.Log("This is the StartingPos" + startingPos);
            //GameObject newAtom = GameObject.Instantiate (atomPrefab, new Vector3 (xCoords [i]+ startingPos.x, yCoords [i] + startingPos.y, zCoords [i] + startingPos.z) * molSize, Quaternion.identity);
            GameObject newAtom = GameObject.Instantiate(atomPrefab, new Vector3(xCoords[i] * molSize + startingPos.x, yCoords[i] * molSize + startingPos.y, zCoords[i] * molSize + startingPos.z), Quaternion.identity);

            //newAtom.transform.localScale *= molSize;
            newAtom.transform.parent = parentMol.transform;
            instantiatedAtoms.Add(newAtom);



        }
        for (int i = 0; i < bondStart.Count; i++)
        {
            Debug.Log("Making Bond! "+i);
            int startAtomID = bondStart[i];
            int endAtomID = bondEnd[i];


            GameObject startAtom = instantiatedAtoms[startAtomID - 1];
            GameObject endAtom = instantiatedAtoms[endAtomID - 1];

            AtomScript startScript = startAtom.GetComponent<AtomScript>();
            AtomScript endScript = endAtom.GetComponent<AtomScript>();

            //Ray bondLine = new Ray (startAtom.transform.position, endAtom.transform.position);
            //Vector3 bondPos = bondLine.GetPoint (Vector3.Distance (startAtom.transform.position, endAtom.transform.position));
            Vector3 bondPos = (startAtom.transform.position + endAtom.transform.position) / 2;

            Rigidbody startAtomRB = startAtom.GetComponent<Rigidbody>();
            Rigidbody endAtomRB = endAtom.GetComponent<Rigidbody>();




            GameObject newBond;

            if (bondOrder[i] == 1)
            {
                newBond = Instantiate(bondModel, startAtom.transform.position, Quaternion.identity);
            }
            else if (bondOrder[i] == 2)
            {
                newBond = Instantiate(doubleBondModel, startAtom.transform.position, Quaternion.identity);
            }
            else
            {
                newBond = Instantiate(bondModel, startAtom.transform.position, Quaternion.identity);
            }

            Bond newBondScript = newBond.GetComponent<Bond>();
            //Debug.Log (newBond + " || " + newBondScript);
            newBondScript.formBond(startAtom, endAtom, bondOrder[i]);
            newBond.transform.SetParent(parentMol.transform);

            newBond.transform.name = "bond" + i;



            /*
			List<GameObject> startAtomBonds = startScript.getBonds ();
			for (int j = 0; j < bondOrder [i]; j++) {
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
				*/
            /*
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

            }*/
        }



















        for (int i = 0; i < instantiatedAtoms.Count; i++)
        {
            /*
			CharacterJoint[] atomJoints = instantiatedAtoms [i].GetComponents<CharacterJoint> ();
			for (int j = 0; j < atomJoints.Length; j++) {
				atomJoints [j].breakForce = 5000f;
			}*/
            for (int j = 0; j < moleculeNameSounds.Count; j++)
            {
                string soundNameRepaired = moleculeNameSounds[j].name.Substring(0, moleculeNameSounds[j].name.IndexOf("Sound"));
                if (soundNameRepaired == molecule.name)
                {

                    instantiatedAtoms[i].GetComponent<AtomScript>().setMoleculeNameSound(moleculeNameSounds[j]);
                }
            }
        }



    }
}
