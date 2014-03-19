using UnityEngine;
using System.Collections;

public class GUITest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		GUILayout.BeginArea(new Rect(200, 200, 300, 400));
		GUI.Label(new Rect (10, 10, 120, 25), "d00md00md00m");
		if(GUI.Button(new Rect(10, 40, 120, 40), "Physics" ))
		{
			Debug.Log("Physics Pressed");
		}
		GUI.Button(new Rect(10, 85, 120, 40), "Navigation" );
		GUI.Button(new Rect(10, 130, 120, 40), "Collision" );
	}
}
