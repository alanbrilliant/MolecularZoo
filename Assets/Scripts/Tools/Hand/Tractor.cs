using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tractor : Gun {

    private GameObject tractoredObject;
    public AudioClip phaserSound;

    private GameObject laser;
    private LaserScript laserScript;
    // Use this for initialization
    void Start () {
        
        laserScript = gameObject.GetComponentInChildren<LaserScript>();
        laser = laserScript.gameObject;

    }


    override public string voiceName
    {
        get { return "Tractor"; }
    }

    // Update is called once per frame
    void Update() {

        if (base.isActive == true)
        {

            if (controller.GetHairTriggerDown())
            {
                if (tractoredObject != null)
                    tractoredObject = null;
            }



            if (controller.GetHairTrigger())
            {


                RaycastHit hit;

                base.wandAudio.clip = phaserSound;
                base.wandAudio.volume = .1f;
                if (base.wandAudio.isPlaying == false)
                    base.wandAudio.Play();

                if (tractoredObject == null)
                {
                    Ray tractorBeamRay = new Ray(transform.position + transform.forward * .2f, transform.forward);
                    if (Physics.Raycast(tractorBeamRay, out hit))
                    {
                        if (hit.collider.gameObject.tag == "Atom" || hit.collider.tag == "Tractorable" || hit.collider.tag == "Pistol")
                        {
                            tractoredObject = hit.collider.gameObject;
							//If the GameObject tractored has a GrabbableCollider script, the tractoredObject should be the GrabbableCollider.parent instead of the object hit.
							//This allows child colliders, otherwise grabbing a child collider grabs the child instead of the parent. See GrabbableCollider.cs for more info.
							GrabbableCollider tc = tractoredObject.GetComponent<GrabbableCollider>();
							if (tc != null) tractoredObject = tc.parent;

							//Send the message that this GameObject is being tractored
							tractoredObject.SendMessage("OnTractor", null, SendMessageOptions.DontRequireReceiver);

							GameObject hitAtom = tractoredObject;
                            Rigidbody hitAtomRB = tractoredObject.GetComponent<Rigidbody>();
                            hitAtomRB.velocity = hitAtomRB.velocity.magnitude * (transform.position - hitAtom.transform.position).normalized;


                        }
                        else
                        {

                            laserScript.enableLaser(transform.position, hit.point);
                        }
                    }
                }

            }

            if (controller.GetHairTriggerUp())
            {
                if (tractoredObject == null)
                {
                    tractoredObject = null;
                    laserScript.disableLaser();
                }
            }

            if (tractoredObject != null)
            {
                Rigidbody tractoredObjRB = tractoredObject.GetComponent<Rigidbody>();
                float targetSpeed = (2f * Vector3.Distance(tractoredObject.transform.position, wand.transform.position)) + .2f;
                float currentAtomSpeed = Vector3.Dot((transform.position - tractoredObject.transform.position).normalized, tractoredObjRB.velocity);
                tractoredObjRB.velocity = targetSpeed * (wand.transform.position - tractoredObjRB.transform.position).normalized;

                laserScript.enableLaser(transform.position, tractoredObject.transform.position);


                if (Vector3.Distance(tractoredObjRB.transform.position, wand.transform.position) < (.05f*0 + tractoredObject.transform.localScale.x/2 ))
                {
					wand.GrabObject (tractoredObject);
                    base.wand.setControllerStateToHand();
                    laserScript.disableLaser();
                    tractoredObject = null;
                    base.wandAudio.Stop();
     
                }

            }
        }
    }


    public override void onDisable() {
        base.onDisable();
        tractoredObject = null;
        laserScript.disableLaser ();
    }
}
