using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slug : MonoBehaviour {


    bool hasCollidedWithAtom = false;

    Rigidbody rb;

	// Use this for initialization
	float time;
	void Start () {
        rb = GetComponent<Rigidbody>();
        if (gameObject.tag == "Bullet")
        {

        }

        //Sets the layer to be the Projectile layer
        gameObject.layer = 8;
		GameObject[] walls = GameObject.FindGameObjectsWithTag ("Wall");
		/*foreach (GameObject wall in walls) {
			Physics.IgnoreCollision(wall.GetComponent<BoxCollider>(), gameObject.GetComponent<SphereCollider>());
		}
		time = 0;
        */
	}


	
	void Update (){
		time += Time.deltaTime;
		if (time >= 5) {
			Destroy (gameObject);
		}
	}


    
    public void resetToAtom()
    {
        gameObject.layer = 0;

        tag = "Atom";


        Destroy(GetComponent<Slug>());

    }
}
