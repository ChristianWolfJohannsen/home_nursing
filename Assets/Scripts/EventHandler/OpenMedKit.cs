using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OpenMedKit : MonoBehaviour 
{
	bool isEmpty;
	bool isOpen;
	
	public List<Items> items;
	private GameObject gameCharacter;
	private CharacterInventory cInv;

	public AnimationClip boxOpen;
	public AnimationClip boxClose;
	
	private Vector3 defaultRot;
	private Vector3 openRot;
	
	void Start () 
	{
		isEmpty =  false;
		isOpen = false;
		items = new List<Items> ();
		gameCharacter = GameObject.Find("First Person Controller");
		cInv = (CharacterInventory)gameCharacter.GetComponent (typeof(CharacterInventory));
		//The medicines
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
	
	void OnMouseDown()
	{
		
		if (Input.GetMouseButtonDown(0) && !isEmpty && !isOpen) 
		{
			Debug.Log ("You open medkit");
			Debug.Log ("The medkit is full");

			MedKit();
			isEmpty = true;
			animation.Play("BoxOpen");
			isOpen = true;
			
		}
		else if (Input.GetMouseButtonDown(0) && isEmpty && !isOpen) 
		{
			Debug.Log ("You open medkit");
			Debug.Log ("The medkit is empty");
			
			animation.Play("BoxOpen");
			isOpen = true;
		}
		else if (Input.GetMouseButtonDown(0) && isOpen) 
		{
			Debug.Log ("You close medkit");

			animation.Play("BoxClose");
			isOpen = false;
		}
	}
	void MedKit()
	{
		Debug.Log ("The medkit is now empty");
		
		cInv.items = items;
		cInv.DoSomething ();
		
	}
}
