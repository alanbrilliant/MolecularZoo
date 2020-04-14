using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAnimatorParameter : MonoBehaviour {
	public enum typeOfParameter
	{
		Bool, Integer, Trigger, Float
	}
	public Animator anim;
	public string parameter;
	public bool waitTillAnimationSwitch;
	public float updateTime;
	public int layer;
	public typeOfParameter t;
	public bool boolValue;
	public float floatValue;
	public bool floatRandom;
	public float floatMin, floatMax;
	public int intValue;
	public bool intRandom;
	public int intMin, intMax;
	public int intWeightOf0;

	private float countDown;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		countDown -= Time.deltaTime;
		if(countDown < 0)
		{
			countDown += updateTime;
		}
		else
		{
			return;
		}
		if(t == typeOfParameter.Bool)
		{
			anim.SetBool(parameter, boolValue);
		}else if (t == typeOfParameter.Float)
		{
			if (floatRandom) floatValue = Random.Range(floatMin, floatMax);
			anim.SetFloat(parameter, floatValue);
		}
		else if (t == typeOfParameter.Integer)
		{
			if (intRandom) intValue = Random.Range(intMin, intMax + intWeightOf0);
			if (intValue > intMax) intValue = 0;
			anim.SetInteger(parameter, intValue);
		}
		else if (t == typeOfParameter.Trigger)
		{
			anim.SetTrigger(parameter);
		}
	}
}
