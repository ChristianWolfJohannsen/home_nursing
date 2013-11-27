using UnityEngine;
using System.Collections;

public class LoadEndMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter () {
		Debug.Log("game has ended");
		Application.LoadLevel(2);
	}	
}
