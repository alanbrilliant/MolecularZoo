using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Sets a parameter of an Animator component
public class SetAnimatorParameter : MonoBehaviour {
	public enum typeOfParameter
	{
		Bool, Integer, Trigger, Float
	}

	[Tooltip("the animator to set the parameter of")]
	public Animator anim;
	[Tooltip("the name of the parameter to change")]
	public string parameter;
	[Tooltip("how often (the reload time) this script should change the parameter")]
	public float updateTime;
	[Tooltip("The type of animator parameter to change. bool, int, trigger, float.")]
	public typeOfParameter t;
	[Tooltip("The value to change the parameter to if t=bool")]
	public bool boolValue;
	[Tooltip("The value to change the parameter to if t=float")]
	public float floatValue;
	[Tooltip("Should the floatValue be decided on randomly?")]
	public bool floatRandom;
	[Tooltip("If floatRandom, the range for the random float")]
	public float floatMin, floatMax;
	[Tooltip("The value to change the parameter to if t=integer")]
	public int intValue;
	[Tooltip("Should the intValue be decided on randomly?")]
	public bool intRandom;
	[Tooltip("If intRandom, the range for the random int")]
	public int intMin, intMax;

	[Tooltip("The extra weight of 0 in the random int selection")]
	/*
	* For the dino, I made a parameter in the animator that controls the animation played.
	* When the value is 0, a basic idle is played. It was annoying that the dino would rarely play the basic idle, instead biting and rolling way too often.
	* This intWeightOf0 adds a [extra] chance that the random int will be 0, causing the dino's basic idle to be played more often
	*/
	public int intWeightOf0;

	private float countDown;

	// Update is called once per frame
	void Update () {
		//reduce coundDown by time
		countDown -= Time.deltaTime;

		//check if countDown is done (<0). Otherwise stop Update(); by returning
		if(countDown < 0)
		{
			countDown += updateTime;
		}
		else
		{
			return;
		}

		//WARNING: Nothing below this will be executed unless countDown was < 0!

		//set a bool parameter
		if (t == typeOfParameter.Bool)
		{
			anim.SetBool(parameter, boolValue);
		}
		//float
		else if (t == typeOfParameter.Float)
		{
			//if a random float is desired, randomly select a float
			if (floatRandom) floatValue = Random.Range(floatMin, floatMax);
			anim.SetFloat(parameter, floatValue);
		}
		//integer
		else if (t == typeOfParameter.Integer)
		{
			//See comment on intWeightOf0 declaration, this code selects an int but gives 0 a weight of intWeightOf0
			if (intRandom) intValue = Random.Range(intMin, intMax + intWeightOf0);
			if (intValue > intMax) intValue = 0;
			anim.SetInteger(parameter, intValue);
		}
		//trigger
		else if (t == typeOfParameter.Trigger)
		{
			anim.SetTrigger(parameter);
		}
	}
}
