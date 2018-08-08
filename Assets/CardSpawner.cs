using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    MoleculeData molData = new MoleculeData();
    MoleculeData saturatedfatData = new MoleculeData();
    private GameObject gameController;
    //bool touched = false;
    public Rigidbody rb;
    private DataManager script1;
    private string testString;
    private string nullString;

    void Start()
    {
        
        rb = GetComponent<Rigidbody>();
        

        /*Functional spawning, test suite
        gameController = GameObject.FindWithTag("GameController");
        script1 = gameController.GetComponent<DataManager>();
        saturatedfatData = script1.loadMolecule("saturatedfatdata.json", "SaturatedFat");
        */
        /*
        DataManager script1 = gameController.GetComponent<DataManager>();
        saturatedfatData = script1.loadMolecule("saturatedfatdata.json", "SaturatedFat");
        */

    }

    public void setMoleculeToSpawn(string molName)
    {
        
        gameController = GameObject.FindWithTag("GameController");
        

    
        script1 = gameController.GetComponent<DataManager>();
        
        molData = script1.loadMolecule(molName.ToLower() + "data.json", molName);

        //saturatedfatData = script1.loadMolecule("saturatedfatdata.json", "SaturatedFat");

        


    }

    void update()
    {

        transform.Rotate(10*Vector3.up * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Arm"))
        {
           
        }

    }

    void OnCollisionEnter(Collision other)
    {
        /*
        Debug.Log("(((((((((((((((((((((((((((((");
        Debug.Log(testString);
        Debug.Log(nullString);
        Debug.Log("(((((((((((((((((((((((((((((");
        Debug.Log(saturatedfatData.ToString());
        */

        if (other.gameObject.CompareTag("Floor") 
            
           )
        {
            MoleculeCreator script = gameObject.GetComponent<MoleculeCreator>();
            
            //Debug.Log("Script: "+script);
            
            script.instantiateMolecule(molData, transform.position);
            
           
        }
        
        else { Destroy(gameObject);
        }
}

        
        

    }

    