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
		movTexture.loop = true;
		audio.loop = true;
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
			movTexture.Pause();
			isRunning = false;
			audio.Pause();
		}
	}
}