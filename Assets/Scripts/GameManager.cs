using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {


	private List<GameObject> nitrogenList;
	private List<GameObject> oxygenList;

	public GameObject nitrogenMolecule;
	public GameObject oxygenAtom;

	void Start () {
		nitrogenList = new List<GameObject> ();
		oxygenList = new List<GameObject> ();

		addN2 ();
		addO2 ();
	}


	private void addN2(){

		for (int i = 0; i < 2000; i++) {
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

			float radius = nit1.transform.localScale.x * .5f;

			Collider col = nit2.GetComponent<SphereCollider> ();


			nit2.transform.position = nit1.transform.position + new Vector3(0,radius,0);
			nit1Joint.connectedBody = nit2.GetComponent<Rigidbody>();

		} 
	}

	private void addO2(){
		for (int i = 0; i < 200; i++) {
			float randY = Random.Range (1f, 3f);
			float randX = Random.Range (-2f, 2f);
			float randZ = Random.Range (-2f, 2f);
			Vector3 startPos = new Vector3 (randX, randY, randZ);

			GameObject oxyAtom = (GameObject)GameObject.Instantiate (oxygenAtom, startPos, Random.rotation) as GameObject;
			oxygenList.Add (oxyAtom);
		}

		for (int i = 0; i < oxygenList.Count; i += 2) {
			GameObject oxy1 = oxygenList [i];
			GameObject oxy2 = oxygenList [i+1];
			FixedJoint oxy1Joint = oxy1.AddComponent<FixedJoint> ();
			SphereCollider oxyCol = oxy1.GetComponent<SphereCollider> ();
			float radius = oxy1.transform.localScale.x * oxyCol.radius;
			oxy2.transform.position = oxy1.transform.position + new Vector3(0,radius,0);
			oxy1Joint.connectedBody = oxy2.GetComponent<Rigidbody>();


		}
	}
	
	// Update is called once per frame
	void Update () {
		
		
	}
}
