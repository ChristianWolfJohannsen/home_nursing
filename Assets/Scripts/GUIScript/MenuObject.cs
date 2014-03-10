using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class MenuObject : MonoBehaviour {
	
	public bool isQuit = false;
	public bool loadNotes = false;
	public bool isLevel1 = false;
	
	
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
			try
        	{
				
				string[] filepaths = Directory.GetFiles("Assets\\Noter");
				foreach(string filename in filepaths) {
					File.Delete(filename);
				}
			}
			catch (Exception e)
	        {
	            Debug.Log("The file could not be read:");
	            Debug.Log(e.Message);
	        }

			if (isLevel1)
			{
				Application.LoadLevel(1);
			}
			else
			{
				Application.LoadLevel(3);
			}
		}
	}
}
