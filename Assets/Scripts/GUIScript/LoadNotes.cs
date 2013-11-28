using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class LoadNotes : MonoBehaviour {
	
	private string txt = "";
	// Use this for initialization
	void Start () {
		Load();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI () {
		Debug.Log("ps");
			GUI.BeginGroup (new Rect (Screen.width/2+60, Screen.height/6-50, 800, 600));
			GUI.Box (new Rect (20, 0, 320, 300), "Noter oprettet i spillet");
			GUI.TextArea (new Rect (25, 25, 310, 250), txt);
			GUI.EndGroup ();
	}
	
	private void Load() {
		try
        {
			int count = 1;
			string[] filepaths = Directory.GetFiles("Assets\\Noter", "*.txt");
			
			foreach(string filename in filepaths) {
				using (StreamReader sr = new StreamReader(filename))
            {
                string line = sr.ReadToEnd();
				txt += "note nr ";
				txt += count.ToString();
				txt += "\n";
				txt += line;
				txt += "\n" + "\n";
				count++;
                Debug.Log(line);
            }
			}
//            using (StreamReader sr = new StreamReader("Assets\\Noter\\1note.txt"))
//            {
//                string line = sr.ReadToEnd();
//				txt = line;
//                Debug.Log(line);
//            }
        }
        catch (Exception e)
        {
            Debug.Log("The file could not be read:");
            Debug.Log(e.Message);
        }
	}
}
