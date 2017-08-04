using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[System.Serializable]
public class MoleculeData {

	//public object[] PC_Compounds = new object[]{ new atoms(),  new bonds(),  new coords()};
	//public List<object> PC_Compounds{get; set;}
	//public PC_Compounds compound{ get; private set; }


	public atoms atom;
	public bonds bond;
	public conformers conf;

	public string name;

	/*
	public List<int> element;
	public List<int> aid1;
	public List<int> aid2;
	public List<int> order;
	public List<float> x;
	public List<float> y;
	public List<float> z;
	*/

	//private string name;

	/*
	public MoleculeData(atoms newAtoms, bonds newBonds, coords newCoords, string moleculeName){
		atomData = newAtoms;
		bondData = newBonds;
		coordData = newCoords;
		name = moleculeName;

	
	}*/

	
	public string ToString() {
		
		string final = "g: ";

		//final += atomData.element [0];
		//final += compound;
		return final;
	}



}
