using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class MenuObject : MonoBehaviour {

	public bool loadNotes = false;
	public int level;
	private bool guiOn = false;
	
	
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
		switch (level) 
		{
		case 0:	guiOn = true; break;
		case 1: Application.LoadLevel(1); DeleteFiles(); break;
		case 2: Application.LoadLevel(3); DeleteFiles(); break;
		}
	}

	void OnGUI () 
	{
		if(guiOn)
		{
			GUI.ModalWindow (1, new Rect (10,10,100,90), DoMyWindow, "Er du sikker?");
		}
	}

	void DoMyWindow(int windowID)
	{
		if (GUI.Button (new Rect (10,30,80,20), "Ja")) 
		{
			Application.Quit();
		}
		
		if (GUI.Button (new Rect (10,60,80,20), "Nej")) 
		{
			guiOn = false;
		}
	}

	private void DeleteFiles()
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
	}
}
