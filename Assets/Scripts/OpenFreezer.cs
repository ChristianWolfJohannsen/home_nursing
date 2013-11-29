using UnityEngine;
using System.Collections;

public class OpenFreezer : MonoBehaviour {
	
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
		animation["OpenFreezer"].speed = 1.0f;
		animation.Play("OpenFreezer");
	}
	
	void PlayBackwards()
	{
		animation["OpenFreezer"].speed = -1.0f;
		animation["OpenFreezer"].time = animation["OpenFreezer"].length;
		animation.Play("OpenFreezer");
	}
}
