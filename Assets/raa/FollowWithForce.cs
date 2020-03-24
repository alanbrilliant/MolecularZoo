using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowWithForce : MonoBehaviour {
	public Transform target;
	public float force;
	public float minDistance;

	private Rigidbody rig;
	// Use this for initialization
	void Start () {
		rig = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Vector3 difference = target.position - transform.position;
		if (difference.magnitude < minDistance) return;
		Vector3 forceVector = difference * Time.fixedDeltaTime * force;
		rig.AddForce(forceVector);
	}
}
