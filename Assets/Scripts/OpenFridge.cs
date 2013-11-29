using UnityEngine;
using System.Collections;

public class OpenFridge : MonoBehaviour {
	
	public bool isOpen = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnMouseDown()
	{
		if(isOpen == false)
		{
			PlayForwards();
			isOpen = true;
		}
		else
		{
			PlayBackwards();
			isOpen = false;
		}
	}
	
	void PlayForwards()
	{
		animation["OpenFridge"].speed = 1.0f;
		animation.Play("OpenFridge");
	}
	
	void PlayBackwards()
	{
		animation["OpenFridge"].speed = -1.0f;
		animation["OpenFridge"].time = animation["OpenFridge"].length;
		animation.Play("OpenFridge");
	}
}
