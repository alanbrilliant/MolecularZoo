using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDeck : Gun {

    public GameObject throwCards;
    //list of card spawners
    private List<GameObject> cardChildObjects = new List<GameObject>();

    private string activeCardName;

    //Int tracking which card is active
    private int cardState;


    override public string voiceName
    {
        get { return "Cards"; }
    }

    public AudioClip gunshot;

    private enum cardDeck { saturatedFat, water, carbonDioxide, };

 

    // Use this for initialization
    void Start () {
        updateCardControllerState();

        card[] cardList = gameObject.GetComponentsInChildren<card>(true);
        //Debug.Log("Card list is this long "+cardList.Length);
        for (int i = 0; i < cardList.Length; i++)
        {

            cardChildObjects.Add(cardList[i].gameObject);
        }

        cardState = (int)cardDeck.saturatedFat;
        cardState = 1;


    }

    // Update is called once per frame
    void Update () {
        
        

        if (base.isActive == true)
        {
            if (controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x > .5 && (controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad)
            || controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_Grip)))
            {
                Debug.Log("card switching");
                updateCardControllerState();
            }

            if (controller.GetHairTriggerDown())
            {
                wandAudio.clip = gunshot;
                wandAudio.volume = .05f;
                wandAudio.Play();
                //Problem Line
                GameObject CardSpinner = null;

                for (int i = 0; i < cardChildObjects.Count; i++)
                {

                    if (activeCardName == cardChildObjects[i].name)
                        CardSpinner = cardChildObjects[i];
                }
                //CardSpinner = GameObject.Find(activeCardName);


                CardSpinner.GetComponent<RotateClass>().startSpin();


                GameObject shot = Instantiate(throwCards, transform.position + transform.forward * .2f, transform.rotation);
                shot.GetComponent<CardSpawner>().setMoleculeToSpawn(activeCardName.Substring(0, activeCardName.Length - 4));

                Rigidbody shotRB = shot.GetComponent<Rigidbody>();
                shotRB.velocity = shotRB.transform.forward * 10;
                shot.transform.Rotate(90, 0, 0);
            }
        }
    }

    private void updateCardControllerState() {


       
        if (cardState == Enum.GetValues(typeof(cardDeck)).Length - 1)
        {
            cardState = 0;
        }
        else
        {
            cardState++;
        }



        //setting name of active card

        activeCardName = "";

        switch (cardState)
        {

            case (int)cardDeck.saturatedFat:
                activeCardName = "SaturatedFatCard";
                break;
            case (int)cardDeck.water:
                activeCardName = "WaterCard";
                break;
            case (int)cardDeck.carbonDioxide:
                activeCardName = "CarbonDioxideCard";
                break;

        }

        //Setting the active card

        for (int i = 0; i < cardChildObjects.Count; i++)
        {
            cardChildObjects[i].SetActive(false);
            if (activeCardName == cardChildObjects[i].name)
                cardChildObjects[i].SetActive(true);
        }

        //Setting Molecule to spawn



    }
}
