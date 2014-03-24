using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MoviePlayer : MonoBehaviour 
{
	public MovieTexture movTexture;
	private bool isRunning;

	void Start () 
	{
		isRunning = false;
		renderer.material.mainTexture = movTexture;
	}


	void OnMouseDown () 
	{
		if (!isRunning && Input.GetMouseButtonDown (0)) 
		{
			movTexture.Play ();
			isRunning = true;
			audio.Play ();
		}
	 	else if (isRunning && Input.GetMouseButtonDown(0)) 
		{
			movTexture.Stop();
			isRunning = false;
			audio.Stop();
		}
	}
}