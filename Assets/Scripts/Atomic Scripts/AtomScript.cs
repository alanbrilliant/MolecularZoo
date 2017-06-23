using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtomScript : MonoBehaviour {

	Rigidbody rb;

	public float speed;
	void Start () {

		rb = gameObject.GetComponent<Rigidbody> ();
		//rb.velocity = gameObject.transform.forward * 2;
		rb.velocity = transform.forward * speed;

		//rb.AddForce(transform.forward * force, ForceMode.Impulse);
		//Debug.Log (ForceMode.Impulse);
	}




	// Update is called once per frame

	void FixedUpdate () {
		//rb.AddForce (force * Vector3.up);
		//rb.velocity = gameObject.transform.forward * 10;
		//transform.rotation.x = 5;
		rb.velocity = rb.velocity.normalized * speed;


	}
}
