using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {
    internal bool isActive;
    internal SteamVR_Controller.Device controller;
    internal Wand wand;

    internal AudioSource wandAudio;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setActive(bool active) {
        isActive = active;
    }

    public void initialize(SteamVR_Controller.Device controller, Wand wandReference) {
        this.controller = controller;
        wand = wandReference;
    }

    public void setAudioSource(AudioSource audioSource) {
        wandAudio = audioSource;
    }

     public virtual void onDisable() {
        setActive(false);
    }
}
