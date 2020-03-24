using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repel : MonoBehaviour {
	public Rigidbody target;
	public float forceMultiplier;
	public float forceMax;

	private Rigidbody rig;
	// Use this for initialization
	void Start () {
		rig = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Vector3 difference = target.transform.position - transform.position;
		float mult = (1 / difference.magnitude) * forceMultiplier;
		if (mult > forceMax) mult = forceMax;
		Vector3 force = difference * Time.fixedDeltaTime * mult;
		rig.AddForce(-force);
		target.AddForce(force);

	}
}
