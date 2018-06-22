using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {
    public float degreesPerSecond = 15.0f;
    //public float amplitude = 0.5f;
    //public float frequency = 1f;
    // Use this for initialization
    //Vector3 posOffset = new Vector3();
    //Vector3 tempPos = new Vector3();
    void Start () {
        //posOffset = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        transform.Rotate (new Vector3(15,30,45) *Time.deltaTime*degreesPerSecond);
        /*
        tempPos = posOffset;
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

        transform.position = tempPos;
        */
        
    }
}
