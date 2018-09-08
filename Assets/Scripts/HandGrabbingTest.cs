using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandGrabbingTest : MonoBehaviour {
    Animator anim;
    public bool isGrabbing;
	// Use this for initialization
	void Start () {
        isGrabbing = false;
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        anim.SetBool("IsGrabbing", isGrabbing);
	}
}
