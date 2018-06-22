using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    MoleculeData saturatedfatData = new MoleculeData();
    public GameObject gameController;
    bool touched = false;
    // Use this for initialization
    public AudioClip saw;
    void Start()
    {
        GetComponent<AudioSource>().playOnAwake = false;
        Debug.Log("We've got a run!");

        gameController = GameObject.FindWithTag("GameController");
        DataManager script1 = gameController.GetComponent<DataManager>();

        saturatedfatData = script1.loadMolecule("saturatedfatdata.json", "SaturatedFat");

    }


    void OnTriggerEnter(Collider other)
    {

        Debug.Log("We've got a trigger!");
        if (other.CompareTag("Arm"))
        {
            touched = true;


        }
        
    }
    void OnCollisionEnter(Collision other)
    {
        Debug.Log("We've got a Collision!");

        if (other.gameObject.CompareTag("Floor") && touched)
        {
            MoleculeCreator script = gameObject.GetComponent<MoleculeCreator>();
            script.instantiateMolecule(saturatedfatData, GetComponent<Rigidbody>().position);
            //AudioSource audio = GetComponent<AudioSource>();
            // audio.Play();
            Debug.Log("Audio should be playing!");
            GetComponent<AudioSource>().Play();
            touched = false;
        }
    }
}