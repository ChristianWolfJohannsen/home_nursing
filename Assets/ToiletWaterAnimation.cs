using UnityEngine;
using System.Collections;

public class ToiletWaterAnimation : MonoBehaviour {

	public bool isRunning = false;
	private ParticleSystem ToiletWater;
	public GameObject ToiletWaterObj;
	
	// Use this for initialization
	void Start () {
		
	}
	
	void OnMouseDown()
	{
		if (!isRunning) {
			Debug.Log ("ToiletWater animation starting");
			ToiletWaterObj.particleSystem.Play();
			isRunning = true;
		}
		else if (isRunning) {
			Debug.Log("ToiletWater animation stopping");
			ToiletWaterObj.particleSystem.Stop();
			isRunning = false;
		}
}
}