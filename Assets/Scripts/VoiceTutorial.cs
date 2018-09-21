using System.Collections;
using System.Collections.Generic;
using UnityEngine.Windows.Speech;
using UnityEngine;

public class VoiceTutorial : MonoBehaviour {


    VoiceRecog voiceManager;

    AudioSource audio;

    private bool tutorialActive = false;
    private int nTutorialsPlayed = 0;

    public AudioClip create;
    public AudioClip molName;
    public AudioClip dictationExplanation; 

	// Use this for initialization
	void Start () {
        audio = GetComponent<AudioSource>();
        voiceManager = GameObject.FindGameObjectWithTag("VoiceManager").GetComponent<VoiceRecog>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnGrab() {
        if (!tutorialActive  && nTutorialsPlayed < 2)
        {
            tutorialActive = true;
            

            /* voiceManager.D_Recognizer.DictationResult += voiceManager.DictationRecognizer_DictationResult;
             voiceManager.D_Recognizer.DictationComplete += voiceManager.DictationRecognizer_DictationComplete;

             //Starts dictation recognizer
             voiceManager.D_Recognizer.Start();*/
            audio.clip = create;
            audio.Play();
            StartCoroutine(waitThenSpawnExampleMolecule());
        }
    }

    IEnumerator waitThenSpawnExampleMolecule() {
        yield return new WaitForSeconds(1);
        voiceManager.ProcessRecognizedPhrase("Create");
        yield return new WaitForSeconds(1);
        audio.clip = molName;
        audio.Play();
        yield return new WaitForSeconds(1);
        voiceManager.DictationRecognizer_DictationComplete(0);

        voiceManager.DictationRecognizer_DictationResult("Acetone", 0);
        yield return new WaitForSeconds(5f);
        audio.clip = dictationExplanation;
        audio.Play();
        yield return new WaitForSeconds(7);
        tutorialActive = false;
        nTutorialsPlayed++;
    }


}
