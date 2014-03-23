using UnityEngine;
using System.Collections;

public class ToiletWaterScript : MonoBehaviour {

	public bool isRunning = false;
	private ParticleSystem toiletWater;
	public GameObject toiletWaterObj;
	
	// Use this for initialization
	void Start () {
		
	}
	
	void OnMouseDown()
	{
		if (!isRunning) {
			Debug.Log ("ToiletWater animation starting");
			toiletWaterObj.particleSystem.Play();
			isRunning = true;
		}
		else if (isRunning) {
			Debug.Log("ToiletWater animation stopping");
			toiletWaterObj.particleSystem.Stop();
			isRunning = false;
		}
}
}
