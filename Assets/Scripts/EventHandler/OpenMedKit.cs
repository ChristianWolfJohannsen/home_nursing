using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OpenMedKit : MonoBehaviour 
{
	bool isEmpty = false;
	bool isOpen = false;
	public AnimationClip boxOpen;
	public AnimationClip boxClose;
	
	private Vector3 defaultRot;
	private Vector3 openRot;
	//public List<Items> items;
	//private GameObject gameCharacter;
	//private GameObject gameCharacterInventory;
	
	void Start () 
	{
		//top = gameObject.transform.GetChild(0).transform;
		//defaultRot = top.transform.eulerAngles;
		//openRot = new Vector3 (defaultRot.x + 90.0f, defaultRot.y, defaultRot.z);
		//items = new List<Items> ();
		//take the medics
		/*Inj Insulatard 42 IE dgl
		T. marevan e. skema 
		T. Selozok 100 mg x2 dgl
		T. Furix 40 mg x 2 dgl
		T. Simvastatin 40 mg x 1 dgl*/
		/*items.Add(new Items("Inj Insulatard 42 IE dgl", 1, 1));
		items.Add (new Items ("T. marevan e. skema", 1, 0));
		items.Add (new Items("T. Selozok 100 mg", 2, 2));
		items.Add (new Items ("T. Furix 40 mg", 2, 2));
		items.Add (new Items ("T. Simvastatin 40 mg", 1, 2));
		
		//gameCharacter = GameObject.Find ("First Person Controller").GetComponent (Transform);
		//gameCharacterInventory = GameObject.Find ("CharacterInventory").GetComponent (Transform);*/
	}
	
	void OnMouseDown()
	{
		if (Input.GetMouseButtonDown (0) && !isOpen) 
		{
			Debug.Log ("You open medkit");
		}
		//Debug.Log ("You open medkit");
		if (Input.GetMouseButtonDown(0) && !isEmpty && !isOpen) 
		{
			//Debug.Log ("You open medkit");
			//Debug.Log ("The medkit is full");
			MedKit();
			isEmpty = true;
			//OpenKit();
			animation.Play("BoxOpen");
			isOpen = true;
			
		}
		else if (Input.GetMouseButtonDown(0) && isEmpty && !isOpen) 
		{
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
		
	}
}
