using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSpin : MonoBehaviour
{
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device controller;
    public float degreesPerSecond = 15.0f;
    //public float amplitude = 0.5f;
    //public float frequency = 1f;
    // Use this for initialization
    //Vector3 posOffset = new Vector3();
    //Vector3 tempPos = new Vector3();
    void Start()
    {
        //controller = SteamVR_Controller.Input((int)trackedObj.index);

        //posOffset = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        //Debug.Log("This shouldnt be happening, why is this hapening");

        //transform.Rotate(new Vector3(0, 0, 3) * Time.deltaTime * degreesPerSecond);

        /*
        tempPos = posOffset;
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

        transform.position = tempPos;
        */

    }
    public void spinCard()
    {
        Debug.Log("Hopefully this works.");
        //StartCoroutine(longSpin());
        //transform.Rotate(new Vector3(0, 0, 3) * Time.deltaTime * degreesPerSecond);
    }
    
    public IEnumerator longSpin()
    {
      
        Debug.Log("Enumerated");
        transform.Rotate(new Vector3(0, 0, 3) * Time.deltaTime * degreesPerSecond);

        yield return new WaitForSeconds(3f); // waits 3 seconds
        transform.Rotate(new Vector3(0, 0, 3) * Time.deltaTime * degreesPerSecond);


    }

}