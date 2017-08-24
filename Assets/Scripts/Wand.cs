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



	void Awake() {
		audio = gameObject.GetComponent<AudioSource> ();
		gunshot = audio.clip;
	}
	// Use this for initialization
	void Start () {
		laserScript = gameObject.GetComponentInChildren<LaserScript> ();
		laser = laserScript.gameObject;
		gunChildObjects = new List<GameObject> ();
		controllerState = 2;
		Gun[] gunList = gameObject.GetComponentsInChildren<Gun>(true) ;

		for (int i = 0; i < gunList.Length; i++) {

			gunChildObjects.Add (gunList [i].gameObject);
		}

		for (int i = 0; i < gunChildObjects.Count; i++) {
			if (i != controllerState)
				gunChildObjects[i].SetActive(false);
		
		}

		controllerState = 2;
		trackedObj = gameObject.GetComponent<SteamVR_TrackedObject>();
		controller = SteamVR_Controller.Input ((int)trackedObj.index);

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
		if (controller.GetPressDown (Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad) || controller.GetPressDown (Valve.VR.EVRButtonId.k_EButton_Grip) ) {
			updateControllerState ();
			
		}
			




		
			
		if (controller.GetHairTriggerDown ()) {
			
			if (controllerState == 0 || controllerState == 1) {


				audio.clip = gunshot;
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
				} else if (controllerState == 1) {
					bullet = redSlugs;
				} else {
					bullet = slugs;
				}

				GameObject shot = Instantiate (bullet, transform.position + transform.forward * .2f, transform.rotation);

				Rigidbody shotRB = shot.GetComponent<Rigidbody> ();
				shotRB.velocity = shotRB.transform.forward * 10;
				shot.transform.Rotate (90, 0, 0);
			} 
		}
		if (controller.GetHairTrigger()){

			if (controllerState ==2) {
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
				if ( tractoredObject != null){
					Rigidbody tractoredObjRB = tractoredObject.GetComponent<Rigidbody>();
					float targetSpeed = 2f * Vector3.Distance (tractoredObject.transform.position, transform.position);
					float currentAtomSpeed = Vector3.Dot ((transform.position - tractoredObject.transform.position).normalized, tractoredObjRB.velocity);
					tractoredObjRB.velocity = targetSpeed * ( transform.position - tractoredObjRB.transform.position).normalized;

					laserScript.enableLaser (transform.position, tractoredObject.transform.position);

					if (Vector3.Distance (tractoredObjRB.transform.position, transform.position) < .3f) {
						laserScript.disableLaser ();
						updateControllerState ();

					}

				}

				/*
						if (currentAtomSpeed < targetSpeed) {
							hitAtomRB.AddForce (7f * (transform.position - hitAtom.transform.position).normalized, ForceMode.Acceleration);
						} else if (currentAtomSpeed > targetSpeed) {
							hitAtomRB.AddForce (-9f * (transform.position - hitAtom.transform.position).normalized, ForceMode.Acceleration);
						}
						/*
						if (hitAtomRB.velocity.magnitude.CompareTo. < 10f) {

							hitAtomRB.AddForce ((transform.position - hitAtom.transform.position).normalized * 50f);
						} else {
							hitAtomRB.AddForce ((transform.position - hitAtom.transform.position).normalized * -50f);
						}*/
					

			}
				
		}

		if (controller.GetHairTriggerUp ()) {
			tractoredObject = null;
			laserScript.disableLaser ();
			if (audio.isPlaying)
				audio.Stop ();
		}


	}

	void OnTriggerStay(Collider other) {

		if (controller.GetHairTrigger ()) {
			
			if (controllerState == 3) {

				if (other.gameObject.tag == "Atom") {
					if (grabJoint.connectedBody == null) {



						grabJoint.connectedBody = other.attachedRigidbody;
						previousGrabbedObjectPosition = grabJoint.connectedBody.gameObject.transform.position;

						other.GetComponent<AtomScript> ().playMoleculeNameSound ();
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
		if (controllerState == 3){
			controllerState = 0;
		} else {
			controllerState++;
		}

		for (int i = 0; i < gunChildObjects.Count; i++) {
			gunChildObjects [i].SetActive (false);
			if (i == controllerState)
				gunChildObjects [i].SetActive (true);
		}
	}
			

	

}


