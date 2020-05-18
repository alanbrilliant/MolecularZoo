using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Used for the dino. Spawns toSpawn when the velocity changes rapidly. 
 * The velocity is calculated based on the position changes.
 * This GameObject must be grabbed by a hand for spawning to occur.
 */
public class SpawnOnShake : MonoBehaviour {
	public GameObject toSpawn;
	public float velocityToSpawn;
	public float reload;
	public float launchSpeed;
	public int framesToSampleVelocity;

	//used to calculate if the velocity changed rapidly
	private Vector3 oldVelocity;
	//used to calculate the velocity
	private Vector3 oldPosition;
	//keep track of if this GameObject is grabbed
	private bool grabbed;
	//
	private int framesLeftToSampleVelocity;
	private float loadLeft;
	public float spawnDist;

	//update if this GameObject is grabbed
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
