using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterInventory : MonoBehaviour {
	public List<Items> items;

	void Start ()
	{
		items = new List<Items> ();
	}


	void Awake () 
	{
		
	}

	public void DoSomething()
	{
		Debug.Log ("Hey there.");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
