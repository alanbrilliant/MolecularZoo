using System;
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

	private List<GameObject> gunChildObjects;

	private int controllerState;

	private GameObject tractoredObject;

	private GameObject laser;
	private LaserScript laserScript;
	public AudioClip phaserSound;
	private AudioClip gunshot;

	AudioClip moleculeNameCooldown;

	private enum arsenal {hands, tractor, pistol, heavyPistol};


	void Awake() {
		audio = gameObject.GetComponent<AudioSource> ();
		gunshot = audio.clip;
	}

	void Start () {
		laserScript = gameObject.GetComponentInChildren<LaserScript> ();
		laser = laserScript.gameObject;
		gunChildObjects = new List<GameObject> ();
		controllerState = (int)arsenal.tractor;
		Gun[] gunList = gameObject.GetComponentsInChildren<Gun>(true) ;

		for (int i = 0; i < gunList.Length; i++) {

			gunChildObjects.Add (gunList [i].gameObject);
		}
			

		controllerState = 0;
		updateControllerState ();
		trackedObj = gameObject.GetComponent<SteamVR_TrackedObject>();
		controller = SteamVR_Controller.Input ((int)trackedObj.index);

		grabJoint = gameObject.AddComponent<FixedJoint> ();

	}


	void Update () {

		if (controllerState == (int)arsenal.hands) {
			if (grabJoint.connectedBody != null) {
				//Debug.Log (grabJoint.connectedBody.velocity);
				Vector3 currentPosition = grabJoint.connectedBody.transform.position;
				grabbedObjectVelocity = (currentPosition - previousGrabbedObjectPosition) / Time.deltaTime;
				previousGrabbedObjectPosition = currentPosition;
			}
		}

		if (controllerState == (int)arsenal.pistol) {
			if (controller.GetHairTriggerDown()) {
				audio.clip = gunshot;
				audio.volume = .05f;
				audio.Play ();

				Debug.Log ("Trigger Press");


				GameObject bullet;
				bullet = slugs;

				GameObject shot = Instantiate (bullet, transform.position + transform.forward * .2f, transform.rotation);

				Rigidbody shotRB = shot.GetComponent<Rigidbody> ();
				shotRB.velocity = shotRB.transform.forward * 10;
				shot.transform.Rotate (90, 0, 0);
			}
		}

		if (controllerState == (int)arsenal.heavyPistol) {
			if (controller.GetHairTriggerDown()) {
				audio.clip = gunshot;
				audio.volume = .05f;
				audio.Play ();

				Debug.Log ("Trigger Press");


				GameObject bullet;
				bullet =  redSlugs;

				GameObject shot = Instantiate (bullet, transform.position + transform.forward * .2f, transform.rotation);

				Rigidbody shotRB = shot.GetComponent<Rigidbody> ();
				shotRB.velocity = shotRB.transform.forward * 10;
				shot.transform.Rotate (90, 0, 0);
			}
		}

		if (controllerState == (int)arsenal.tractor){



			if (controller.GetHairTriggerDown ()) {
				if (tractoredObject != null)
					tractoredObject = null;
			}



			if (controller.GetHairTrigger()) {


				RaycastHit hit;

				audio.clip = phaserSound;
				audio.volume = .1f;
				if (audio.isPlaying == false)
					audio.Play ();

				if (tractoredObject == null) {
					Ray tractorBeamRay = new Ray (transform.position + transform.forward * .2f, transform.forward);
					if (Physics.Raycast (tractorBeamRay, out hit)) {
						if (hit.collider.gameObject.tag == "Atom") {
							tractoredObject = hit.collider.gameObject;
							GameObject hitAtom = hit.collider.gameObject;
							Rigidbody hitAtomRB = hit.collider.gameObject.GetComponent<Rigidbody> ();
							hitAtomRB.velocity = hitAtomRB.velocity.magnitude * (transform.position - hitAtom.transform.position).normalized;


						} else {

							laserScript.enableLaser (transform.position, hit.point);
						}
					}
				}

			}

			if (controller.GetHairTriggerUp ()) {
				if (tractoredObject == null) {
					tractoredObject = null;
					laserScript.disableLaser ();
				}
			}

			if ( tractoredObject != null){
				Rigidbody tractoredObjRB = tractoredObject.GetComponent<Rigidbody>();
				float targetSpeed = 2f * Vector3.Distance (tractoredObject.transform.position, transform.position);
				float currentAtomSpeed = Vector3.Dot ((transform.position - tractoredObject.transform.position).normalized, tractoredObjRB.velocity);
				tractoredObjRB.velocity = targetSpeed * ( transform.position - tractoredObjRB.transform.position).normalized;

				laserScript.enableLaser (transform.position, tractoredObject.transform.position);


				float tractoredAtomDistance = 0;
				if (controller.GetHairTrigger ()) {
					tractoredAtomDistance = .2f;
				} else {
					tractoredAtomDistance = .04f;
				}
					

				if (Vector3.Distance (tractoredObjRB.transform.position, transform.position) < .2f) {
					controllerState = Enum.GetValues (typeof(arsenal)).Length - 1;
					updateControllerState ();
					laserScript.disableLaser ();
					tractoredObject = null;
					audio.Stop ();



				}

			}




		}



		if (controller.GetHairTriggerUp () && grabJoint.connectedBody != null) {
			Rigidbody connectedRigidbody = grabJoint.connectedBody;
			grabJoint.connectedBody = null;
			connectedRigidbody.velocity = grabbedObjectVelocity;
		}
		if (controller.GetPressDown (Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad) || controller.GetPressDown (Valve.VR.EVRButtonId.k_EButton_Grip) ) {
			updateControllerState ();
			
		}
			

		if (controller.GetHairTriggerUp ()) {
			
			if (audio.isPlaying) {
				if (audio.clip != gunshot)
					audio.Stop ();
			}
		}


	}

	void OnTriggerStay(Collider other) {

		if (controller.GetHairTrigger ()) {
			
			if (controllerState == (int)arsenal.hands) {

				if (other.gameObject.tag == "Atom") {
					if (grabJoint.connectedBody == null) {



						grabJoint.connectedBody = other.attachedRigidbody;
						previousGrabbedObjectPosition = grabJoint.connectedBody.gameObject.transform.position;

						if (other.GetComponent<AtomScript> ().getMoleculeNameSound () != moleculeNameCooldown) {
							other.GetComponent<AtomScript> ().playMoleculeNameSound ();
							moleculeNameCooldown = other.GetComponent<AtomScript> ().getMoleculeNameSound ();
						}
							

					}
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

	private void updateControllerState(){
		tractoredObject = null;
		laserScript.disableLaser ();

		if (audio.isPlaying) {
			if (audio.clip != gunshot)
				audio.Stop ();
		}
		if (controllerState == Enum.GetValues(typeof(arsenal)).Length - 1){
			controllerState = 0;
		} else {
			controllerState++;
		}


		string activeWeaponName = "";


		switch (controllerState)
		{
		case (int)arsenal.hands:
			break;

		case (int)arsenal.tractor:
			activeWeaponName = "Tractor";
			break;
		case (int)arsenal.pistol:
			activeWeaponName = "Pistol";
			break;
		case (int)arsenal.heavyPistol:
			activeWeaponName = "HeavyPistol";
			break;
		}

		for (int i = 0; i < gunChildObjects.Count; i++) {
			gunChildObjects [i].SetActive (false);
			if (activeWeaponName == gunChildObjects[i].name)
				gunChildObjects [i].SetActive (true);
		}
			
	}
			

	

}


