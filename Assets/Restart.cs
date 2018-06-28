using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    
    bool touched = false;


    void Start()
    {
        

       

    }


    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Arm"))
        {
            touched = true;
        }

    }
    void OnCollisionEnter(Collision other)
    {

        if (other.gameObject.CompareTag("Floor") && touched)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
           
        }
        touched = false;
    }
}