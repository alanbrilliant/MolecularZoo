using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wand : MonoBehaviour {

    public GameObject grabPosition;
    private SphereCollider col;

	private SteamVR_TrackedObject trackedObj;

	public GameObject bullets;
	public GameObject heavyBullets;

	private AudioSource audio;
	// GameObject gun;

	public FixedJoint grabJoint;
    //private SpringJoint pullJoint;

    private GameObject cards;
	private Vector3 grabbedObjectVelocity;
	private Vector3 previousGrabbedObjectPosition;

	private GameObject[] atomSpawns = new GameObject[4];

	private List<GameObject> gunChildObjects;
  


    public int controllerState;



	
	public AudioClip phaserSound;
	private AudioClip gunshot;
	AudioClip moleculeNameCooldown;
    public bool _isGrabbing;
    Animator anim;
    private float axisValue;
    private bool isHoldingTool = false;
    private Transform grabPos;

   /* if (leftWand  != null || rightWand != null)
        {
            if (leftWand == gameObject)
            {
                otherWand = rightWand.GetComponent<Wand>();
            }
            else
            {
                otherWand = leftWand.GetComponent<Wand>();
            }
        }*/
    private Wand otherWand
    {
        get {
            GameObject leftWand = GameObject.FindGameObjectWithTag("LeftWand");
            GameObject rightWand = GameObject.FindGameObjectWithTag("RightWand");
            if (leftWand == gameObject)

                return (rightWand == null ? null : rightWand.GetComponent<Wand>());
            else
                return (leftWand == null ? null : leftWand.GetComponent<Wand>());
            
        }
    }

    public SteamVR_Controller.Device controller
    {
        get {
            return (trackedObj.index == SteamVR_TrackedObject.EIndex.None ?
                null :
                SteamVR_Controller.Input((int)trackedObj.index));
        }
    }


    private enum arsenal {hands, tractor, pistol, heavyPistol, cards,};

    //enumerator with all molecule spawn types


    void Awake() {
		audio = gameObject.GetComponent<AudioSource> ();
		gunshot = audio.clip;

    }


    void Start() {
        isHoldingTool = false;
        SphereCollider[] possibleSphereColliders = GetComponentsInChildren<SphereCollider>();

        foreach (SphereCollider collider in possibleSphereColliders)
        {
            if (collider.tag == "Arm" && collider.isTrigger == true)
                col = collider;
        }
			

        anim = GetComponent<Animator>();

        initializeAtomSpawns();

        gunChildObjects = new List<GameObject>();
        //iniializing card spawner list
        controllerState = (int)arsenal.tractor;
        //Initializing card state to saturated fat

        //populating list of cards


        //Debug.Log(cardList.ToString());
        controllerState = 1;

        trackedObj = gameObject.GetComponent<SteamVR_TrackedObject>();

        //updateControllerState();


        //Gets objects in children with the gun component, then puts them into the gunList list. Then, puts the gameObject attached to each gun component into the gunChildObjects list 
        Gun[] gunList = gameObject.GetComponentsInChildren<Gun>(true);
        for (int i = 0; i < gunList.Length; i++)
        {
            gunList[i].initialize( this);
            gunList[i].setAudioSource(audio);
            //  if (gunList[i].name == gunList[i].GetComponentInParent<Transform>().name)

            gunChildObjects.Add(gunList[i].gameObject);
        }

        grabJoint = gameObject.AddComponent<FixedJoint>();

        //If the program adds two instances of the same object to gunChildObjects for some reason, this code will remove it
        for (int i = 0; i < gunChildObjects.Count; i++)
        {
            for (int j = 0; j < gunChildObjects.Count; j++)
            {
                if (gunChildObjects[i] == gunChildObjects[j] && i != j)
                {
                    gunChildObjects.Remove(gunChildObjects[j]);
                }
            }
        }

        
        anim.SetBool("IsGrabbing", true);
    }



	void Update () {


        axisValue = controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).x;
        anim.SetFloat("GrabbingFloat", axisValue);

        /*
        if (grabJoint.connectedBody != null && grabJoint.connectedBody.tag == "Atom")
        {
            if (otherWand != null && otherWand.grabJoint.connectedBody != null && otherWand.grabJoint.connectedBody.tag == "Atom")
            {
                FormDoubleBond();
            }
        }*/
        
        if (controllerState == 0)
        {
            if (controller.GetHairTriggerUp())
            {
                anim.SetBool("IsGrabbing", true);
            }

            grabObject();
            updatePositionAndVelocityOfGrabbedObject();

            if (grabJoint.connectedBody != null)
            {


                if (controller.GetHairTriggerUp() && isHoldingTool)
                {

                    
                    audio.clip = gunshot;
                    audio.volume = .05f;
                    audio.Play();

                    GameObject bullet;
                    bullet = bullets;

                    GameObject shot = Instantiate(bullet, grabJoint.connectedBody.transform.position + grabJoint.connectedBody.transform.forward * .2f, grabJoint.connectedBody.transform.rotation);
                    shot.AddComponent<Slug>();
                    shot.tag = "AtomBullet";
                    Rigidbody shotRB = shot.GetComponent<Rigidbody>();
                    shotRB.velocity = shotRB.transform.forward * 10;
                    shot.transform.Rotate(90, 0, 0);

                }

                if (grabJoint.connectedBody.CompareTag("Pistol"))
                {
                    Debug.Log("Holding the pistol!");
                }

            }

        } else
        {
            gunChildObjects[controllerState - 1].GetComponent<Gun>().setActive(true);

        }


        //Release of objects
        if (controller.GetHairTriggerUp () && grabJoint.connectedBody != null && (grabJoint.connectedBody.gameObject.tag == "Atom"|| grabJoint.connectedBody.gameObject.tag == "Tractorable")) {
			Rigidbody connectedRigidbody = grabJoint.connectedBody;
			grabJoint.connectedBody = null;


            connectedRigidbody.velocity = grabbedObjectVelocity;
			connectedRigidbody.gameObject.SendMessage("OnRelease", null, SendMessageOptions.DontRequireReceiver);
        }

        //Release of trigger when holding a tool
	/* Not sure what this code was trying to do.
        if (controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_Grip)&&isHoldingTool){
            Rigidbody connectedRigidbody = grabJoint.connectedBody;
            grabJoint.connectedBody = null;

            connectedRigidbody.velocity = grabbedObjectVelocity;
            controllerState = 3;
            updateControllerState();
        }
	*/


	// Advance to next tool if touchpad or grip button pressed.
        if (controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad) ||
	    controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_Grip))
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

    //TODO: Fix update controller state so that it accepts a variable representing the controller state that you want to go to, rather than-
    //just incrememnting the controller state by one 
	public void updateControllerState(){




        //If the controller state is at the end of the list of tools, then reset the controller back to 0
        if (controllerState == gunChildObjects.Count)
        {
            //Set to 0, becase the hand is always at the zero'th index
            controllerState = 0;
        } else
        {
            controllerState++;
        }
       

        for (int i = 0; i < gunChildObjects.Count; i++) {
            gunChildObjects[i].GetComponent<Gun>().onDisable();
            gunChildObjects[i].GetComponent<Gun>().setActive(false);
			gunChildObjects [i].SetActive (false);
            
			//if (activeWeaponName == gunChildObjects[i].name)
				//gunChildObjects [i].SetActive (true);
           
		}
        if (controllerState > 0)
        {
            gunChildObjects[controllerState - 1].SetActive(true);
            }



    }
    
    private void initializeAtomSpawns(){
		AtomSpawn[] preSpawns = gameObject.GetComponentsInChildren<AtomSpawn> ();
		for (int i = 0; i < preSpawns.Length; i++) {
			atomSpawns [i] = preSpawns [i].gameObject;
		}
	}


    private void grabObject() {
        if (controller.GetHairTriggerDown())
        {
            //Math.Abs(col.transform.localScale.x))
            //Find all the colliders in the sphere collider of the hand
            Collider[] collidersInRangeOfHand = Physics.OverlapSphere(col.transform.TransformPoint(col.center), col.radius * Math.Abs(col.transform.localScale.x));
          // GameObject testSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //testSphere.transform.position = col.transform.TransformPoint(col.center);
            
            //testSphere.transform.localScale = new Vector3( col.radius * Math.Abs(col.transform.localScale.x), col.radius * Math.Abs(col.transform.localScale.x), col.radius * Math.Abs(col.transform.localScale.x));

            //The next piece of code, up until the end of the for loop, checks to find the collider closest to the center of the hand collider
            //Also checks to make sure that collider's tag either contains atom, or is tractorable
            Collider closestCollider = null;

            //Large number placed in closestColliderDistance temporarily
            float closestColliderDistance = 10000000;


            foreach (Collider possibleCol in collidersInRangeOfHand)
            {
                if (Vector3.Distance(transform.TransformPoint(col.center), possibleCol.ClosestPoint(transform.TransformPoint(col.center))) < closestColliderDistance
                    && (possibleCol.tag.Contains("Atom") || possibleCol.tag == "Tractorable"||possibleCol.tag == "Pistol"))
                {

                    closestCollider = possibleCol;
                    closestColliderDistance = Vector3.Distance(transform.TransformPoint(col.center), possibleCol.ClosestPoint(transform.TransformPoint(col.center)));
                }
            }


            //If there are any colliders in range at all
            if (closestCollider != null)
            {
                //Debug.Log("Grabbing!!");
				GameObject closeObject = closestCollider.gameObject;
				GrabbableCollider tc = closeObject.GetComponent<GrabbableCollider>();
				if (tc != null) closeObject = tc.parent;

				//If the closest collider is an atom spawner, then the code will instantiate an atom, and attach that to the hand
				if (closeObject.tag == "AtomSpawn")
                {

                    if (grabJoint.connectedBody == null)
                    {
                        AtomSpawn spawnScript = closeObject.GetComponent<AtomSpawn>();
                        GameObject newAtom = Instantiate(spawnScript.associatedAtom, closeObject.transform.position, closeObject.transform.rotation);

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
                if (closeObject.tag == "Atom" || closeObject.tag == "Tractorable" || closeObject.tag == "Pistol")
                {
					if (grabJoint.connectedBody == null)
						GrabObject (closeObject);
                }
            }
        }
    }

	public void GrabObject(GameObject closeObject)
	{

		//  StartCoroutine(MoveOverSeconds(closeObject, grabPosition.transform.position, .25f));

		// Notify tractored object that it has been grabbed.
		// This is used to start an audio tutorial.
		closeObject.SendMessage("OnGrab", null, SendMessageOptions.DontRequireReceiver);

		grabJoint.connectedBody = closeObject.GetComponent<Rigidbody>();
		previousGrabbedObjectPosition = grabJoint.connectedBody.gameObject.transform.position;


		AtomScript atom = closeObject.GetComponent<AtomScript> ();
		if (atom != null && atom.getMoleculeNameSound() != moleculeNameCooldown)
		{
			atom.playMoleculeNameSound();
			moleculeNameCooldown = atom.getMoleculeNameSound();
		}


		if (closeObject.tag == "Pistol")
		{
			isHoldingTool = true;
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

            //Keeps objects from moving if they are too slow
            if (grabbedObjectVelocity.magnitude < .15f)
                grabbedObjectVelocity = Vector3.zero;
                
        }
    }

    /*
    IEnumerator MoveToPosition()
    {
        float timeWaited = 0;
        while (timeWaited < 2)
        {
            timeWaited += Time.deltaTime;

            yield return null;
        }


    }*/

    /*
    public IEnumerator MoveOverSpeed(GameObject objectToMove, Vector3 end, float speed)
    {
        // speed should be 1 unit per second
        while (objectToMove.transform.position != end)
        {
            objectToMove.transform.position = Vector3.MoveTowards(objectToMove.transform.position, end, speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }*/

    //Move 
    public IEnumerator MoveOverSeconds(GameObject objectToMove, Vector3 end, float seconds)
    {
        grabJoint.connectedBody = null;
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.transform.position;
        while (elapsedTime < seconds)
        {
            grabJoint.connectedBody = null;

            objectToMove.transform.position = Vector3.Lerp(startingPos, grabPosition.transform.position, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        objectToMove.transform.position = grabPosition.transform.position;


        
        grabJoint.connectedBody = objectToMove.GetComponent<Rigidbody>();

    }

    //Will do the math to set the controller state of the wand to whatever the empty hand is
    public void setControllerStateToHand() {
        controllerState = gunChildObjects.Count;
        updateControllerState();

    }

    /*
    private void FormDoubleBond() {
        GameObject myAtom = grabJoint.connectedBody.gameObject;
        GameObject otherAtom = otherWand.grabJoint.connectedBody.gameObject;
        AtomScript myAtomScript = myAtom.GetComponent<AtomScript>();
        AtomScript otherAtomScript = otherAtom.GetComponent<AtomScript>();
        
        if (Vector3.Distance(myAtom.transform.position,otherAtom.transform.position) < .5f)
        {
            if (myAtomScript.bondedAtoms.Contains(otherAtom))
            {
                if (myAtomScript.hasOpenBonds() && otherAtomScript.hasOpenBonds())
                {

                    myAtomScript.breakBondWith(otherAtom);
                    //otherAtomScript.breakBondWith(myAtom);
                    grabJoint.connectedBody = null;
                    // otherWand.grabJoint.connectedBody = null;
                    myAtomScript.bondWithAtom(otherAtom, 2);
                }     
            }
        } 
    }*/


    //Sets the active tool by passing in a name
    public void setToolByVoiceName(string toolName) {
        for(int i = 0; i < gunChildObjects.Count; i++)
        {
            if (gunChildObjects[i].GetComponent<Gun>().voiceName == toolName)
            {
                controllerState = i;
                updateControllerState();
                return;
            }
            
        }
        if (toolName == "Hand")
        {
            setControllerStateToHand();
            return;
        }
    }

}


