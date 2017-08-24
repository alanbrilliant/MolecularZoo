using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserScript : MonoBehaviour {

	LineRenderer lnRenderer;
	private Vector3 startPoint;
	private Vector3 endPoint;

	void Start () {
		lnRenderer = gameObject.GetComponent<LineRenderer> ();
		lnRenderer.SetWidth (.2f, .2f);
		lnRenderer.enabled = false;

		
	}
	
	// Update is called once per frame
	void Update () {
		lnRenderer.SetPosition (0, startPoint);
		lnRenderer.SetPosition (1, endPoint);
	}

	public void enableLaser(Vector3 start, Vector3 end){
		startPoint = start;
		endPoint = end;
		lnRenderer.enabled = true;
	}

	public void disableLaser(){
		lnRenderer.enabled = false;
	}
}
