using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Earth : MonoBehaviour {

	// Use this for initialization
	void Start () {
		gameObject.GetComponent<Rigidbody> ().AddTorque (new Vector3 (0, 100, 0));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
