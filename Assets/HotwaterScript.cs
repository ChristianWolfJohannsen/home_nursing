using UnityEngine;
using System.Collections;

public class HotwaterScript : MonoBehaviour {

	public bool isRunning = false;
	private ParticleSystem Sinkwater;
	public GameObject sinkWaterObj;

	// Use this for initialization
	void Start () {

	}

	void OnMouseDown()
	{
		if (!isRunning) {
			Debug.Log ("Sinkwater animation starting");
			sinkWaterObj.particleSystem.Play();
			isRunning = true;
				}
		else if (isRunning) {
			Debug.Log("Sinkwater animation stopping");
			sinkWaterObj.particleSystem.Stop();
			isRunning = false;
				}


	}
}
