using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class RotateClass : MonoBehaviour
{

    public float turnSpeed = 90f;
    public float turnTime = 5f;

    private bool spinning = false;
    private Quaternion oldRot;
  

   
    public void startSpin()
    {
        if (!spinning)
            StartCoroutine(Spin());
    }
    IEnumerator Spin()
    {
        Debug.Log("Should start spinning");
        spinning = true;
        float timeSpun = 0;
        var turnIncrement = new Vector3(0, 0, turnSpeed * Time.deltaTime);
        //Turn towards the side.
        while (timeSpun < turnTime)
        {
            timeSpun += Time.deltaTime;
            transform.Rotate((new Vector3(0, 0, 3) * Time.deltaTime)+turnIncrement);
            yield return null;
        }
        
        spinning = false;

        
        transform.localEulerAngles = new Vector3(-20, 0, 0);
        
    }
}
