using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmLine : MonoBehaviour {


    LineRenderer ln;
    // Use this for initialization

    GameObject rightWand;
    GameObject leftWand;

	void Start () {
        ln = GetComponent<LineRenderer>();

        rightWand = GameObject.FindGameObjectWithTag("RightWand");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
