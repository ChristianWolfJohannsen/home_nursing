using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class MenuObject : MonoBehaviour {

	public bool loadNotes = false;
	public int level;
	
	
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
		case 0: Application.Quit(); break;
		case 1: Application.LoadLevel(1); DeleteFiles(); break;
		case 2: Application.LoadLevel(3); DeleteFiles(); break;
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
