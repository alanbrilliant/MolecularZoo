using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wand : MonoBehaviour {


    private SphereCollider col;

	private SteamVR_TrackedObject trackedObj;
	private SteamVR_Controller.Device controller;

	public GameObject bullets;
	public GameObject heavyBullets;
    public GameObject throwCards;

	private AudioSource audio;
	private GameObject gun;
	private GameObject redGun;
	private FixedJoint grabJoint;
    private GameObject cards;
	private Vector3 grabbedObjectVelocity;
	private Vector3 previousGrabbedObjectPosition;

	private GameObject[] atomSpawns = new GameObject[4];

	private List<GameObject> gunChildObjects;

	private int controllerState;
    
	private GameObject tractoredObject;

	private GameObject laser;
	private LaserScript laserScript;
	public AudioClip phaserSound;
	private AudioClip gunshot;

	AudioClip moleculeNameCooldown;


	private enum arsenal {hands, tractor, pistol, heavyPistol, cards,};


	void Awake() {
		audio = gameObject.GetComponent<AudioSource> ();
		gunshot = audio.clip;
	}

	void Start () {
        SphereCollider[] possibleSphereColliders = GetComponentsInChildren<SphereCollider>();

        foreach (SphereCollider collider in possibleSphereColliders)
        {
            if (collider.tag == "Arm" && collider.isTrigger == true)
                col = collider;
        }


		initializeAtomSpawns ();
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

        if (controllerState == (int)arsenal.cards)
        {
            if (controller.GetHairTriggerDown())
            {
                audio.clip = gunshot;
                audio.volume = .05f;
                audio.Play();

                Debug.Log("Trigger Press");


                GameObject bullet;
                bullet = throwCards;

                GameObject shot = Instantiate(bullet, transform.position + transform.forward * .2f, transform.rotation);

                Rigidbody shotRB = shot.GetComponent<Rigidbody>();
                shotRB.velocity = shotRB.transform.forward * 10;
                shot.transform.Rotate(90, 0, 0);
            }



        }
        if (controllerState == (int)arsenal.pistol) {
			if (controller.GetHairTriggerDown()) {
				audio.clip = gunshot;
				audio.volume = .05f;
				audio.Play ();

				Debug.Log ("Trigger Press");


				GameObject bullet;
				bullet = bullets;

				GameObject shot = Instantiate (bullet, transform.position + transform.forward * .2f, transform.rotation);
                shot.AddComponent<Slug>();

                shot.tag = "AtomBullet";

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
				bullet =  heavyBullets;

				GameObject shot = Instantiate (bullet, transform.position + transform.forward * .2f, transform.rotation);
                shot.AddComponent<Slug>();

                shot.tag = "AtomExplosiveBullet";

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
						if (hit.collider.gameObject.tag == "Atom" || hit.collider.tag == "Tractorable") {
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

        if (controllerState == (int)arsenal.hands)
        {

            grabObject();
            updatePositionAndVelocityOfGrabbedObject();    

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
        case (int)arsenal.cards:
            activeWeaponName = "Cards";
             break;
        }

		for (int i = 0; i < gunChildObjects.Count; i++) {
			gunChildObjects [i].SetActive (false);
			if (activeWeaponName == gunChildObjects[i].name)
				gunChildObjects [i].SetActive (true);
		}
			
	}

	private void initializeAtomSpawns(){
		AtomSpawn[] preSpawns = gameObject.GetComponentsInChildren<AtomSpawn> ();
		for (int i = 0; i < preSpawns.Length; i++) {
			atomSpawns [i] = preSpawns [i].gameObject;
		}
	}


    private void grabObject() {
        if (controller.GetHairTrigger())
        {
            //Find all the colliders in the sphere collider of the hand
            Collider[] collidersInRangeOfHand = Physics.OverlapSphere(transform.TransformPoint(col.center), col.radius * col.transform.localScale.x);

            //The next piece of code, up until the end of the for loop, checks to find the collider closest to the center of the hand collider
            //Also checks to make sure that collider's tag either contains atom, or is tractorable
            Collider closestCollider = null;

            //Large temporary number placed in closestColliderDistance temporarily
            float closestColliderDistance = 10000000;

            foreach (Collider possibleCol in collidersInRangeOfHand)
            {
                if (Vector3.Distance(transform.TransformPoint(col.center), possibleCol.ClosestPoint(transform.TransformPoint(col.center))) < closestColliderDistance
                    && (possibleCol.tag.Contains("Atom") || possibleCol.tag == "Tractorable"))
                {

                    closestCollider = possibleCol;
                    closestColliderDistance = Vector3.Distance(transform.TransformPoint(col.center), possibleCol.ClosestPoint(transform.TransformPoint(col.center)));
                }
            }
            //If there are any colliders in range at all
            if (closestCollider != null)
            {

                



                //If the closest collider is an atom spawner, then the code will instantiate an atom, and attach that to the hand
                if (closestCollider.gameObject.tag == "AtomSpawn")
                {

                    if (grabJoint.connectedBody == null)
                    {
                        AtomSpawn spawnScript = closestCollider.gameObject.GetComponent<AtomSpawn>();
                        GameObject newAtom = Instantiate(spawnScript.associatedAtom, closestCollider.gameObject.transform.position, closestCollider.gameObject.transform.rotation);

                        //TODO: Fix below and stuff under if statement that checks and atoms tag to make more efficient, i.e. don't repeat code

                        grabJoint.connectedBody = newAtom.GetComponent<Rigidbody>();
                        previousGrabbedObjectPosition = grabJoint.connectedBody.gameObject.transform.position;
                        if (newAtom.GetComponent<AtomScript>().getMoleculeNameSound() != moleculeNameCooldown)
                        {
                            newAtom.GetComponent<AtomScript>().playMoleculeNameSound();
                            moleculeNameCooldown = newAtom.GetComponent<AtomScript>().getMoleculeNameSound();
                        }

                    }
                }

                //If the closest collider is an atom or some other "Tractorable" object, then it can be grabbed. If the object is an atom, then it will play it's molecule name if the cooldown has run out
                //Examples of "Tractorable" objects include the green cube and the reset sphere
                if (closestCollider.gameObject.tag == "Atom" || closestCollider.gameObject.tag == "Tractorable")
                {
                    if (grabJoint.connectedBody == null)
                    {


                        grabJoint.connectedBody = closestCollider.attachedRigidbody;
                        previousGrabbedObjectPosition = grabJoint.connectedBody.gameObject.transform.position;

                        if (closestCollider.tag == "Atom" && closestCollider.GetComponent<AtomScript>().getMoleculeNameSound() != moleculeNameCooldown)
                        {
                            closestCollider.GetComponent<AtomScript>().playMoleculeNameSound();
                            moleculeNameCooldown = closestCollider.GetComponent<AtomScript>().getMoleculeNameSound();
                        }


                    }
                }
            }
        }
    }

    //Updates the field for the previous object velocity and position of the grabbed object
    //This is necessary to throw an object, as otherwise when the trigger is released, the object will have no velocity and will not be thrown
    private void updatePositionAndVelocityOfGrabbedObject() {
        if (grabJoint.connectedBody != null)
        {
            //Debug.Log (grabJoint.connectedBody.velocity);
            Vector3 currentPosition = grabJoint.connectedBody.transform.position;
            grabbedObjectVelocity = (currentPosition - previousGrabbedObjectPosition) / Time.deltaTime;
            previousGrabbedObjectPosition = currentPosition;
            if (grabbedObjectVelocity.magnitude < .15f)
                grabbedObjectVelocity = Vector3.zero;
        }
    }
			

	

}


