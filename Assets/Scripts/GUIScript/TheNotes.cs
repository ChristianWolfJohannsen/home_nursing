using UnityEngine;
using System.Collections;
using System.IO;

public class TheNotes : MonoBehaviour {

	private string textAreaString = "";
	private bool openInventoryWindow;//   Controls OnGUI and opens/closes the inventory window
	private Rect inventoryWindow;//    The dimensions of the inventory window
	private int countTo = 0;
	private string prefix ="";
	private string name = "note";
	private string suffix = ".txt";

	void OnGUI ()
	{
		// Inventory Window
	    if (openInventoryWindow)//   If the "open inventory window" toggle is true
	    {                                          
	       			// Make a background box
			GUI.BeginGroup (new Rect (Screen.width - 500, Screen.height -250, 800, 600));
			GUI.Box (new Rect (20, 0, 400, 200), "Noter");
			if(GUI.Button(new Rect(335,175,75,20), "Gem Note"))
			{
				if(Directory.Exists("Assets\\Noter")) 
				{
//					DirectoryInfo dir = new DirectoryInfo("Assets\\Noter");
					int count = Directory.GetFiles("Assets\\Noter", "*.txt").Length;
					Debug.Log(count.ToString());
					
					countTo = count +1;
					Debug.Log(countTo.ToString());
					prefix = (string)countTo.ToString();
					
					string fileName = "";
					fileName += prefix;
					fileName += name;
					fileName += suffix;
					
					Debug.Log(fileName);
					
					File.WriteAllText("Assets\\Noter\\"+fileName, textAreaString);
					
				
					openInventoryWindow = false;
				}
				else
				{
					Directory.CreateDirectory("Assets\\Noter");
					
//					DirectoryInfo dir = new DirectoryInfo("Assets\\Noter");
					int count = Directory.GetFiles("Assets\\Noter", "*.txt").Length;
					
					countTo = count +1;
					prefix = (string)countTo.ToString();
					
					string fileName = "";
					fileName += prefix;
					fileName += name;
					fileName += suffix;
					
					File.WriteAllText("Assets\\Noter\\"+fileName, textAreaString);
					openInventoryWindow = false;
				}
				
				
			}
			textAreaString = GUI.TextArea (new Rect (25, 25, 390, 145), textAreaString);
			GUI.EndGroup ();
	    }
		
	}
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyUp (KeyCode.N)) {//   If the "i" key is pressed...
			openInventoryWindow = !openInventoryWindow; //   ... toggle the inventory window.
 
		}
	}
	
	void CloseNotes()
	{
		openInventoryWindow = false;
	}
}
