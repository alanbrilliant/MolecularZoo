using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dino : MonoBehaviour {

	[Tooltip("The dino can teleport back to home")]
	public Transform home;
	[Tooltip("The rigidbody of the dino")]
	public Rigidbody rig;
	[Tooltip("The Audio to play when grabbed")]
	public AudioSource audioSource;
	[Tooltip("If the dino is left alone for longer than this, he will go home")]
	public float timeToGoHome;

	//called when the tractor beam hits
	void OnTractor()
	{
		//release the dino
		transform.SetParent(null, true);
		rig.isKinematic = false;
		//cancel ReturnToHome(); since the dino is being tractored and should not teleport back
		StopCoroutine("ReturnToHome");
	}

	//called when a hand grabs
	void OnGrab()
	{
		//release the dino
		transform.SetParent(null, true);
		rig.isKinematic = false;
		//play the dino noise
		audioSource.Play();
		//cancel ReturnToHome(); since the dino is being grabbed and should not teleport back
		StopCoroutine("ReturnToHome");
	}

	//called when a hand releases
	void OnRelease()
	{
		//after some time, the dino should return home
		StartCoroutine("ReturnToHome");
	}

	//Returns the dino to home after some time
	IEnumerator ReturnToHome()
	{
		yield return new WaitForSeconds(timeToGoHome);//don't go home immediately, wait some time
		//The dino shouldn't move based on physics any more
		rig.isKinematic = true;
		//teleport the dino back home
		transform.parent = home;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
	}
}
