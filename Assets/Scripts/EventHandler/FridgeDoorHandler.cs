using UnityEngine;
using System.Collections;

public class FridgeDoorHandler : MonoBehaviour 
{
	private bool isOpen = false;
	
	void OnMouseDown()
	{
		if (!isOpen) 
		{
			isOpen = true;
			animation.Play ("OpenFridge");
		} 

		else 
		{
			isOpen = false;
			animation.Play ("CloseFridge");
		}
	}
	
}
