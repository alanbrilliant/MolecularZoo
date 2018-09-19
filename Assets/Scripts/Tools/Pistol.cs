using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Gun {


    public AudioClip gunshot;
   public  GameObject bullet;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (base.isActive == true)
        {/*
            anim.SetBool("IsGrabbing", true);
            anim.SetFloat("GrabbingFloat", 0);*/

            if (base.controller.GetHairTriggerDown())
            {

                base.wandAudio.clip = gunshot;
                base.wandAudio.volume = .05f;
                base.wandAudio.Play();
                // Debug.Log ("Trigger Press");




                GameObject shot = Instantiate(bullet, transform.position + transform.forward * .2f, transform.rotation);
                shot.AddComponent<Slug>();

                shot.tag = "AtomBullet";

                Rigidbody shotRB = shot.GetComponent<Rigidbody>();
                shotRB.velocity = shotRB.transform.forward * 10;
                shot.transform.Rotate(90, 0, 0);
            }
        }
		
	}

    public override void onDisable() {
        base.onDisable();
    }



}
