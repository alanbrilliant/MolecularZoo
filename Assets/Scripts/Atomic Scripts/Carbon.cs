using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carbon : MonoBehaviour {

	Rigidbody rb;

	public float speed;
	void Start () {

		rb = gameObject.GetComponent<Rigidbody> ();
		rb.velocity = gameObject.transform.forward * 10;

		//rb.AddForce(transform.forward * force, ForceMode.Impulse);
		//Debug.Log (ForceMode.Impulse);
	}



	// Update is called once per frame
	void FixedUpdate () {
		//rb.AddForce (force * Vector3.up);
		//rb.velocity = gameObject.transform.forward * 10;
		//transform.rotation.x = 5;
		rb.velocity = rb.velocity.normalized * speed;
		Debug.Log(rb.velocity.magnitude);
	}
}