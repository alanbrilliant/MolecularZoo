using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slug : MonoBehaviour {

	// Use this for initialization
	float time;
	void Start () {

		time = 0;
	}
	
	void Update (){
		time += Time.deltaTime;
		if (time >= 5) {
			Destroy (gameObject);
		}
	}
}
