using UnityEngine;
using System.Collections;

public class StoveFireOnMouseDown : MonoBehaviour {

	public bool isRunning = false;
	private ParticleSystem FireStove;
	public GameObject FireStoveObj;
	
	// Use this for initialization
	void Start () {
		
	}
	
	void OnMouseDown()
	{
		if (!isRunning) {
			Debug.Log ("Firestove animation starting");
			FireStoveObj.particleSystem.Play();
			isRunning = true;
		}
		else if (isRunning) {
			Debug.Log("Firestove animation stopping");
			FireStoveObj.particleSystem.Stop();
			isRunning = false;
		}
		
		
	}
}
