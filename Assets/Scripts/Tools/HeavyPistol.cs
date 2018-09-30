using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyPistol : Gun {

    public AudioClip gunshot;
    public GameObject heavyBullet;

    override public string voiceName {
        get { return "Blaster"; }
    }
    
    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void Update() {

        if (base.isActive == true)
        {

            if (controller.GetHairTriggerDown())
            {

                wandAudio.clip = gunshot;
                wandAudio.volume = .05f;
                wandAudio.Play();

                //Debug.Log ("Trigger Press");


               

                GameObject shot = Instantiate(heavyBullet, transform.position + transform.forward * .2f, transform.rotation);
                shot.AddComponent<Slug>();

                shot.tag = "AtomExplosiveBullet";

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
