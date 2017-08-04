using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wand : MonoBehaviour {


	private SteamVR_TrackedObject trackedObj;
	private SteamVR_Controller.Device controller;
	public GameObject slugs;
	public GameObject redSlugs;
	private AudioSource audio;
	private GameObject gun;
	private GameObject redGun;
	private FixedJoint grabJoint;
	private Vector3 grabbedObjectVelocity;
	private Vector3 previousGrabbedObjectPosition;

	private int controllerState;

	void Awake() {
		audio = gameObject.GetComponent<AudioSource> ();
	}
	// Use this for initialization
	void Start () {
		controllerState = 0;
		trackedObj = gameObject.GetComponent<SteamVR_TrackedObject>();
		controller = SteamVR_Controller.Input ((int)trackedObj.index);
		gun = gameObject.GetComponentInChildren<Gun> ().gameObject;
		grabJoint = gameObject.AddComponent<FixedJoint> ();
	}


	void Update () {
		if (grabJoint.connectedBody != null) {
			//Debug.Log (grabJoint.connectedBody.velocity);
			Vector3 currentPosition = grabJoint.connectedBody.transform.position;
			grabbedObjectVelocity = (currentPosition - previousGrabbedObjectPosition) / Time.deltaTime;
			previousGrabbedObjectPosition = currentPosition;
		}
			
			
		if (controller.GetHairTriggerUp () && grabJoint.connectedBody != null) {
			Rigidbody connectedRigidbody = grabJoint.connectedBody;
			grabJoint.connectedBody = null;
			connectedRigidbody.velocity = grabbedObjectVelocity;
		}
		if (controller.GetPressDown (Valve.VR.EVRButtonId.k_EButton_Grip)) {
			if (controllerState == 2){
				controllerState = 0;
			} else {
				controllerState++;
			}
			
		}
			
		if (controllerState == 0) {
				gun.SetActive (true);

		} else if (controllerState ==2) {
				gun.SetActive (false);

		}



		
			
		if (controller.GetHairTriggerDown()) {
			
			if (controllerState == 0 || controllerState == 1) {

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

				GameObject bullet;
				if (controllerState == 0) {
					bullet = slugs;
				} else {
					bullet = redSlugs;
				}

				GameObject shot = Instantiate (bullet, transform.position + transform.forward * .2f, transform.rotation);

				Rigidbody shotRB = shot.GetComponent<Rigidbody> ();
				shotRB.velocity = shotRB.transform.forward * 10;
				shot.transform.Rotate (90, 0, 0);
			} 
				
		}


	}

	void OnTriggerStay(Collider other) {

		if (controller.GetHairTriggerDown ()) {

			if (controllerState == 2) {

				if (other.gameObject.tag == "Atom") {


					grabJoint.connectedBody = other.attachedRigidbody;
					previousGrabbedObjectPosition = grabJoint.connectedBody.gameObject.transform.position;
				}

			}

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


