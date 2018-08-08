using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    MoleculeData saturatedfatData = new MoleculeData();
    private GameObject gameController;
    //bool touched = false;
    public Rigidbody rb;


    void Start()
    {
        //GetComponent<AudioSource>().playOnAwake = false;

        gameController = GameObject.FindWithTag("GameController");
        DataManager script1 = gameController.GetComponent<DataManager>();

        saturatedfatData = script1.loadMolecule("saturatedfatdata.json", "SaturatedFat");
        rb = GetComponent<Rigidbody>();
    }

    void update()
    {
        transform.Rotate(10*Vector3.up * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Arm"))
        {
            //touched = true;
        }

    }

    void OnCollisionEnter(Collision other)
    {

        if (other.gameObject.CompareTag("Floor") 
            //&& touched
           )
        {
            MoleculeCreator script = gameObject.GetComponent<MoleculeCreator>();
            script.instantiateMolecule(saturatedfatData, transform.position);
            
            //Debug.Log(GameObject.Find("ThrowingCard").transform.position);

            // GetComponent<AudioSource>().Play();
        }
        
        else { Destroy(gameObject);
        }
}

        
        //touched = false;

    }

    