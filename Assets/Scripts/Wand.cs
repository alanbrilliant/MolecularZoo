using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wand : MonoBehaviour {


	private SteamVR_TrackedObject trackedObj;
	private SteamVR_Controller.Device controller;

	public GameObject bullets;
	public GameObject heavyBullets;
    public GameObject throwCards;

	private AudioSource audio;
	// GameObject gun;
	private GameObject redGun;
	private FixedJoint grabJoint;
    private GameObject cards;
	private Vector3 grabbedObjectVelocity;
	private Vector3 previousGrabbedObjectPosition;

	private GameObject[] atomSpawns = new GameObject[4];

	private List<GameObject> gunChildObjects;
    //list of card spawners
    private List<GameObject> cardChildObjects;


    public int controllerState;
    
    //Int tracking which card is active
    private int cardState;
    
	private GameObject tractoredObject;

	private GameObject laser;
	private LaserScript laserScript;
	public AudioClip phaserSound;
	private AudioClip gunshot;
    private GameObject CardSpinner;
	AudioClip moleculeNameCooldown;
    private string activeCardName;
    public bool _isGrabbing;
    Animator anim;
    public SteamVR_Controller.Device mDevice;
    private float axisValue;

    private enum arsenal {hands, tractor, pistol, heavyPistol, cards,};

    //enumerator with all molecule spawn types
    private enum cardDeck { saturatedFat, water, carbonDioxide, };


    void Awake() {
		audio = gameObject.GetComponent<AudioSource> ();
		gunshot = audio.clip;
	}

	void Start () {
        anim = GetComponent<Animator>();

        initializeAtomSpawns();
		laserScript = gameObject.GetComponentInChildren<LaserScript> ();
		laser = laserScript.gameObject;
		gunChildObjects = new List<GameObject> ();
        //iniializing card spawner list
        cardChildObjects = new List<GameObject>();
	    controllerState = (int)arsenal.tractor;
        //Initializing card state to saturated fat
        cardState = (int)cardDeck.saturatedFat;

        //populating list of cards
        
        card[] cardList = gameObject.GetComponentsInChildren<card>(true);
        //Debug.Log("Card list is this long "+cardList.Length);
        for (int i = 0; i < cardList.Length; i++)
        {

            cardChildObjects.Add(cardList[i].gameObject);
        }
        
        Gun[] gunList = gameObject.GetComponentsInChildren<Gun>(true) ;

		for (int i = 0; i < gunList.Length; i++) {

			gunChildObjects.Add (gunList [i].gameObject);
		}
        //Debug.Log(cardList.ToString());
        controllerState = 0;
        cardState = 1;


        updateControllerState ();
        updateCardControllerState();
		trackedObj = gameObject.GetComponent<SteamVR_TrackedObject>();
		controller = SteamVR_Controller.Input ((int)trackedObj.index);

		grabJoint = gameObject.AddComponent<FixedJoint> ();
     // _isGrabbing = false;
        anim.SetBool("IsGrabbing", true);
        
    }


	void Update () {
        axisValue = controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).x;
        anim.SetFloat("GrabbingFloat", axisValue);

        //Debug.Log("This is the axisValue" + axisValue);
        if (controllerState == (int)arsenal.hands) {
			if (grabJoint.connectedBody != null) {
				//Debug.Log (grabJoint.connectedBody.velocity);
				Vector3 currentPosition = grabJoint.connectedBody.transform.position;
				grabbedObjectVelocity = (currentPosition - previousGrabbedObjectPosition) / Time.deltaTime;
				previousGrabbedObjectPosition = currentPosition;
			}



        }

        if (controllerState == (int)arsenal.cards)
        {
            anim.SetBool("IsGrabbing", true);
            anim.SetFloat("GrabbingFloat", 0);

            if (controller.GetHairTriggerDown())
            {
                audio.clip = gunshot;
                audio.volume = .05f;
                audio.Play();
                Debug.Log("hhf43");
                //Problem Line
                
                     for (int i = 0; i < cardChildObjects.Count; i++)
                {
                    
                    if (activeCardName == cardChildObjects[i].name)
                        CardSpinner= cardChildObjects[i];
                }
                //CardSpinner = GameObject.Find(activeCardName);
                Debug.Log(CardSpinner);

                
                CardSpinner.GetComponent<RotateClass>().startSpin();

                Debug.Log("Trigger Press");

                GameObject shot = Instantiate(throwCards, transform.position + transform.forward * .2f, transform.rotation);
                shot.GetComponent<CardSpawner>().setMoleculeToSpawn(activeCardName.Substring(0, activeCardName.Length - 4));

                Rigidbody shotRB = shot.GetComponent<Rigidbody>();
                shotRB.velocity = shotRB.transform.forward * 10;
                shot.transform.Rotate(90, 0, 0);
            }



        }
        if (controllerState == (int)arsenal.pistol) {
            anim.SetBool("IsGrabbing", true);
            anim.SetFloat("GrabbingFloat", 0);

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
            anim.SetBool("IsGrabbing", true);
            anim.SetFloat("GrabbingFloat", 0);

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
            anim.SetBool("IsGrabbing", true);
            anim.SetFloat("GrabbingFloat", 0);




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

        if (controller.GetHairTriggerUp())
        {
            anim.SetBool("IsGrabbing", true);
        }



        if (controller.GetHairTriggerUp () && grabJoint.connectedBody != null) {
			Rigidbody connectedRigidbody = grabJoint.connectedBody;
			grabJoint.connectedBody = null;
            
            connectedRigidbody.velocity = grabbedObjectVelocity;
		}

        //Looks inportant, ask alan
		if (controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x>.5 &&(controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad) || controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_Grip)))
        {
            Debug.Log("card switching");
            updateCardControllerState ();
        }else if (controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad) || controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_Grip))
        {
            updateControllerState();

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
                _isGrabbing = true;
                anim.SetBool("IsGrabbing", false);

                if (other.gameObject.tag == "AtomSpawn") {
					
					if (grabJoint.connectedBody == null ) {
                        AtomSpawn spawnScript = other.gameObject.GetComponent<AtomSpawn> ();
						GameObject newAtom = Instantiate (spawnScript.associatedAtom, other.gameObject.transform.position,other.gameObject.transform.rotation);

						//TODO: Fix below and stuff under if statement that checks and atoms tag to make more efficient, i.e. don't repeat code

						grabJoint.connectedBody = newAtom.GetComponent<Rigidbody>();
						previousGrabbedObjectPosition = grabJoint.connectedBody.gameObject.transform.position;
						if (newAtom.GetComponent<AtomScript> ().getMoleculeNameSound () != moleculeNameCooldown) {
							newAtom.GetComponent<AtomScript> ().playMoleculeNameSound ();
							moleculeNameCooldown = newAtom.GetComponent<AtomScript> ().getMoleculeNameSound ();
						}

					}
				}

				if (other.gameObject.tag == "Atom" || other.gameObject.tag == "Tractorable") {
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

	public void updateControllerState(){
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
    private void updateCardControllerState()
    {
        //This stuff probably isnt needed
        tractoredObject = null;
        laserScript.disableLaser();

        if (audio.isPlaying)
        {
            if (audio.clip != gunshot)
                audio.Stop();
        }
        if (cardState == Enum.GetValues(typeof(cardDeck)).Length - 1)
        {
            cardState = 0;
        }
        else
        {
            cardState++;
        }



        //setting name of active card

        activeCardName = "";

        switch (cardState)
        {

            case (int)cardDeck.saturatedFat:
                activeCardName = "SaturatedFatCard";
                break;
            case (int)cardDeck.water:
                activeCardName = "WaterCard";
                break;
            case (int)cardDeck.carbonDioxide:
                activeCardName = "CarbonDioxideCard";
                break;

        }

        //Setting the active card

        for (int i = 0; i < cardChildObjects.Count; i++)
        {
            cardChildObjects[i].SetActive(false);
            if (activeCardName == cardChildObjects[i].name)
                cardChildObjects[i].SetActive(true);
        }
        Debug.Log("This should probably happen");
        //Setting Molecule to spawn
       


    }
    private void initializeAtomSpawns(){
		AtomSpawn[] preSpawns = gameObject.GetComponentsInChildren<AtomSpawn> ();
		for (int i = 0; i < preSpawns.Length; i++) {
			atomSpawns [i] = preSpawns [i].gameObject;
		}
	}
			

	

}


