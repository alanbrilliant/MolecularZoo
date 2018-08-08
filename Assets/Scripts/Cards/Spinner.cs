using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour {
    public float degreesPerSecond = 15.0f;

    // Use this for initialization
    void Start () {

    }

    // Update is called once per frame
    void Update () {
        transform.RotateAround(Vector3.zero, Vector3.forward, 20 * Time.deltaTime * degreesPerSecond);
        transform.Translate(-5*Vector3.forward * Time.deltaTime);
    }
}
