using UnityEngine;
using System.Collections;

public class MenuObject : MonoBehaviour {
	
	public bool isQuit = false;
	public bool loadNotes = false;
	
	
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
			Debug.Log("FISK");
			//Ved ikke hvorfor den ikke virker!!!
			Application.Quit();
		}
		else
		{
			Application.LoadLevel(1);
		}
	}
}
