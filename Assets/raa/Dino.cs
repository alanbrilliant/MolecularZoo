using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dino : MonoBehaviour {
	public Transform home;
	public Rigidbody rig;
	public float timeToGoHome;//if the dino is left alone for too long, he will teleport home

	private bool grabbed;
	// Use this for initialization
	void Start () {
		
	}
	void OnGrab()
	{
		transform.SetParent(null, true);
		rig.isKinematic = false;
		grabbed = true;
	}
	void OnRelease()
	{
		grabbed = false;
		Invoke("CheckPosition", timeToGoHome);
	}
	void CheckPosition()
	{
		if (!grabbed)
		{
			rig.isKinematic = true;
			transform.parent = home;
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
		}
	}
	// Update is called once per frame
	void Update () {
		
	}
}
