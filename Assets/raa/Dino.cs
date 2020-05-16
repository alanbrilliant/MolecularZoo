using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dino : MonoBehaviour {
	public Transform home;
	public Rigidbody rig;
	public AudioSource audioSource;
	public float timeToGoHome;//if the dino is left alone for too long, he will teleport home

	private bool grabbed;
	// Use this for initialization
	void Start () {
		
	}

	void OnTractor()//should not set grabbed = true, but same as OnGrab apart from that
	{
		transform.SetParent(null, true);
		rig.isKinematic = false;
		StopAllCoroutines();
	}

	void OnGrab()
	{
		transform.SetParent(null, true);
		rig.isKinematic = false;
		grabbed = true;
		audioSource.Play();
		StopAllCoroutines();
	}
	void OnRelease()
	{
		grabbed = false;
		StartCoroutine("CheckPosition");
	}
	IEnumerator CheckPosition()
	{
		yield return new WaitForSeconds(timeToGoHome);
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
