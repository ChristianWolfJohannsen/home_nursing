using UnityEngine;
using System.Collections;

public class MenuObject : MonoBehaviour {
	
	public bool isQuit = false;
	
	
	void OnMouseEnter()
	{
		
		this.renderer.material.color = Color.green;
	}
	
	void OnMouseExit()
	{
		renderer.material.color = Color.red;
	}
	
	void OnMouseDown()
	{
		if (isQuit) 
		{
			//Ved ikke hvorfor den ikke virker!!!
			Application.Quit();
		}
		else
		{
			Application.LoadLevel(1);
		}
	}
}
