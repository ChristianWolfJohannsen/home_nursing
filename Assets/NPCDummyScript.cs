using UnityEngine;
using System.Collections;

public class NPCDummyScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other)
	{
		Debug.Log ("Something has entered");
		if (other.gameObject.tag == "Player")
		{
			Debug.Log ("Player has entered");
			GameObject.Find("JokerNavigator").GetComponent<JokerNavigator>().enabled = false;
			Debug.Log("JokerNavigator should be disabled");
		}
	}
}
