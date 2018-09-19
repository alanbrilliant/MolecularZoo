using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour {

    //ParticleSystem particles;
    // ParticleSystem.EmissionModule emission;
    // Use this for initialization
    GameObject particles;
	void Start () {
        //particles = GetComponent<ParticleSystem>();
        // emission = particles.emission;
        // emission.enabled = false; ;
        particles = GetComponentInChildren<ParticleSystem>(true).gameObject;

	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void createBlackHole() {
        // emission.enabled = true;
        particles.SetActive(true);
            StartCoroutine("playBeamParticleEffect");
    }

    IEnumerator playBeamParticleEffect() {
        yield return new WaitForSeconds(1);
        particles.SetActive(false);
        //emission.enabled = false;

        GameObject[] atoms = GameObject.FindGameObjectsWithTag("Atom");
        foreach (GameObject atom in atoms)
        {
            atom.GetComponent<Rigidbody>().velocity = (transform.position - atom.transform.position).normalized * 10f;
        }

    }
}
