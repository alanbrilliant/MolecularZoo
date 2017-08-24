 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Earth : MonoBehaviour {

	private AudioSource audio;

	// Use this for initialization

	void Awake() {
		audio = gameObject.GetComponent<AudioSource> ();
	}
	void Start () {
		gameObject.GetComponent<Rigidbody> ().AddTorque (new Vector3 (0, 100, 0));
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter(Collision col){
		GameObject obj = col.collider.gameObject;
		if (obj.tag == "Bullet" || obj.tag == "ExplosiveBullet") {
			audio.volume = .07f;
			if(audio.isPlaying == false)
				audio.Play ();
		}
	}
}
