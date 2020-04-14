using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnOnShake : MonoBehaviour {
	public GameObject toSpawn;
	public float accelerationToSpawn;
	public float reload;

	private Vector3 oldVelocity;
	private Vector3 oldPosition;
	private bool grabbed;
	private float loadLeft;
	void OnGrab() { grabbed = true; }
	void OnRelease() { grabbed = false; }
	// Use this for initialization
	void Start () {
		oldPosition = transform.position;
		oldVelocity = Vector3.zero;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (!grabbed) return;//must be grabbed to do anything
		Vector3 velocity = (transform.position - oldPosition) / Time.fixedDeltaTime;
		loadLeft -= Time.fixedDeltaTime;

		if(loadLeft < 0 &&(velocity-oldVelocity).magnitude > accelerationToSpawn)
		{
			GameObject g = Instantiate(toSpawn, transform.position, transform.rotation);
			Rigidbody rig = g.GetComponent<Rigidbody>();
			if (rig != null)
			{
				rig.velocity = oldVelocity-velocity;
			}
			loadLeft = reload;
		}

		oldPosition = transform.position;
		oldVelocity = velocity;
	}
}
