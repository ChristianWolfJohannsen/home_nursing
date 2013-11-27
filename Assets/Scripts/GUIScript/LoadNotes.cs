using UnityEngine;
using System.Collections;
using System.IO;

public class LoadNotes : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI () {
		Debug.Log("ps");
			GUI.BeginGroup (new Rect (Screen.width/2+60, Screen.height/6-50, 800, 600));
			GUI.Box (new Rect (20, 0, 320, 300), "Noter oprettet i spillet");
			GUI.TextArea (new Rect (25, 25, 310, 250), "");
			GUI.EndGroup ();
	}
}
