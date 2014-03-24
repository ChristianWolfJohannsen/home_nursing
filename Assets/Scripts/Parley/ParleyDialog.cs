using UnityEngine;
using System.Collections;

public class ParleyDialog : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void DialogStarted()
	{
		GameObject g = GameObject.Find("MAX");
		MAX d = g.GetComponent<MAX>();

		d.Nod();
	}
}
