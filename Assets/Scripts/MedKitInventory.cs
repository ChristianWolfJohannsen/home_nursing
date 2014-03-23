using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MedKitInventory : MonoBehaviour {

	public List<Items> items;


	// Use this for initialization
	void Start () 
	{
		items = new List<Items> ();
		//take the medics
		/*Inj Insulatard 42 IE dgl
		T. marevan e. skema 
		T. Selozok 100 mg x2 dgl
		T. Furix 40 mg x 2 dgl
		T. Simvastatin 40 mg x 1 dgl*/
		items.Add(new Items("Inj Insulatard 42 IE dgl", 1, 1));
		items.Add (new Items ("T. marevan e. skema", 1, 0));
		items.Add (new Items("T. Selozok 100 mg", 2, 2));
		items.Add (new Items ("T. Furix 40 mg", 2, 2));
		items.Add (new Items ("T. Simvastatin 40 mg", 1, 2));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
