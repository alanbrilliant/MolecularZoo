using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slug : MonoBehaviour {

	// Use this for initialization
	float time;
	void Start () {
		GameObject[] walls = GameObject.FindGameObjectsWithTag ("Wall");
		foreach (GameObject wall in walls) {
			Physics.IgnoreCollision(wall.GetComponent<BoxCollider>(), gameObject.GetComponent<CapsuleCollider>());
		}
		time = 0;
	}
	
	void Update (){
		time += Time.deltaTime;
		if (time >= 5) {
			Destroy (gameObject);
		}
	}
}
