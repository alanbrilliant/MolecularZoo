using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wand : MonoBehaviour {


	private SteamVR_TrackedObject trackedObj;
	private SteamVR_Controller.Device controller;
	public GameObject slugs;
	private AudioSource audio;

	void Awake() {
		audio = gameObject.GetComponent<AudioSource> ();
	}
	// Use this for initialization
	void Start () {
		trackedObj = gameObject.GetComponent<SteamVR_TrackedObject>();
		controller = SteamVR_Controller.Input ((int)trackedObj.index);
	}


	void Update () {
		if (controller.GetHairTriggerDown ()) {

			audio.volume = .05f;
			audio.Play ();

			Debug.Log ("Trigger Press");


			/*
			GameObject cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
			Vector3 position = gameObject.transform.position;
			position = incrementDimension (position, 5, 'y');
			position = incrementDimension (position, 5, 'x');
			position = incrementDimension (position, 5, 'z');

			Ray ray = new Ray (transform.position, transform.forward);
			position = ray.GetPoint (2);

			cube.transform.position = position;
			cube.AddComponent<Rigidbody> ();
			*/

			GameObject shot = Instantiate (slugs, transform.position+ transform.forward*.2f, transform.rotation);

			Rigidbody shotRB = shot.GetComponent<Rigidbody> ();
			shotRB.velocity = shotRB.transform.forward * 10;
			shot.transform.Rotate (90, 0, 0);

		}
	}

	private Vector3 incrementDimension(Vector3 initial, float value, char dimension){
		Vector3 final = new Vector3 (0, 0, 0);

		float initX = initial.x;
		float initY = initial.y;
		float initZ = initial.z;

		if (dimension == 'x') {
			initX += value;
			final = new Vector3 (initX, initY, initZ);
		} else if (dimension == 'y') {
			initY += value;
			final = new Vector3 (initX, initY, initZ);
		} else if (dimension == 'z') {
			initZ += value;
			final = new Vector3 (initX, initY, initZ);
		}


		return final;
	}
			

	

}


