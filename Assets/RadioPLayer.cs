using UnityEngine;
using System.Collections;

public class RadioPLayer : MonoBehaviour {

	private bool isRunning;

	// Use this for initialization
	void Start () 
	{
		audio.Play ();
		isRunning = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseDown()
	{
		if (isRunning && Input.GetMouseButtonDown (0)) 
		{
			audio.Pause ();
			isRunning = false;
		} 
		else if (!isRunning && Input.GetMouseButtonDown (0))
		{
			audio.Play();
			isRunning = true;
		}
	}
}
