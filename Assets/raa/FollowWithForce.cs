using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//used by the spawn balls, follows a target by adding a force to the rigidbody
public class FollowWithForce : MonoBehaviour {
	[Tooltip("The target to approach")]
	public Transform target;
	[Tooltip("The force to add to the rigidbody (multiplied by the distacne from target)")]
	public float force;
	[Tooltip("The distance at which the target is considered met")]
	public float minDistance;

	//the rigidbody attached
	private Rigidbody rig;
	// Use this for initialization
	void Start () {
		//get the rigidbody
		rig = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//find the difference
		Vector3 difference = target.position - transform.position;
		//if close enough, don't add a force
		if (difference.magnitude < minDistance) return;
		//add a force to reduce the distance
		Vector3 forceVector = difference * Time.fixedDeltaTime * force;
		rig.AddForce(forceVector);
	}
}
