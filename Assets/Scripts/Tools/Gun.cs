using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {
    internal bool isActive;
    internal Wand wand;

    internal AudioSource wandAudio;

    internal SteamVR_Controller.Device controller {
        get {
            return wand.controller;
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setActive(bool active) {
        isActive = active;
    }

    public void initialize( Wand wandReference) {
        wand = wandReference;
    }

    public void setAudioSource(AudioSource audioSource) {
        wandAudio = audioSource;
    }

     public virtual void onDisable() {
        setActive(false);
    }

    virtual public string voiceName {
        get {
	    return gameObject.name;
	}
    }

}
