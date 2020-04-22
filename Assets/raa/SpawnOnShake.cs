using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnOnShake : MonoBehaviour {
	public GameObject toSpawn;
	public float velocityToSpawn;
	public float reload;
	public float launchSpeed;
	public int framesToSampleVelocity;


	private Vector3 oldVelocity;
	private Vector3 oldPosition;
	private bool grabbed;
	private int framesLeftToSampleVelocity;
	private float loadLeft;
	public float spawnDist;

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
		framesLeftToSampleVelocity--;
		loadLeft -= Time.fixedDeltaTime;

		if (framesLeftToSampleVelocity <= 0)
		{
			framesLeftToSampleVelocity = framesToSampleVelocity;

			Vector3 velocity = (transform.position - oldPosition) / Time.fixedDeltaTime;

			if (loadLeft < 0 && velocity.magnitude > velocityToSpawn)
			{
				GameObject g = Instantiate(toSpawn, transform.position + velocity.normalized * spawnDist, transform.rotation);
				Rigidbody rig = g.GetComponent<Rigidbody>();
				if (rig != null)
				{
					rig.velocity = velocity.normalized * launchSpeed;
				}
				loadLeft = reload;
			}

			oldPosition = transform.position;
			//oldVelocity = velocity;
		}
	}
}
