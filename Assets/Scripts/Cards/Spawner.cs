using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    MoleculeData saturatedfatData = new MoleculeData();
    public GameObject gameController;
    bool touched = false;

    public AudioClip potassium;
    void Start()
    {
        GetComponent<AudioSource>().playOnAwake = false;

        gameController = GameObject.FindWithTag("GameController");
        DataManager script1 = gameController.GetComponent<DataManager>();

        saturatedfatData = script1.loadMolecule("saturatedfatdata.json", "SaturatedFat");

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
            MoleculeCreator script = gameObject.GetComponent<MoleculeCreator>();
            //GetComponent<Rigidbody>().position
            //GameObject.Find("Your_Name_Here").transform.position;
            script.instantiateMolecule(saturatedfatData, GameObject.Find("Ammo").transform.position);
            Debug.Log(GameObject.Find("Ammo").transform.position);

            GetComponent<AudioSource>().clip = potassium;
            GetComponent<AudioSource>().Play();
        }
        touched = false;
        /*else
        {
            touched = false;
        }*/
    }
}