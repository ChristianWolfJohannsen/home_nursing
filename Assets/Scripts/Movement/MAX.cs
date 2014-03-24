using UnityEngine;
using System.Collections;

public class MAX : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/// <summary>
	/// Make the character nod.
	/// </summary>
	public void Nod()
	{
		Debug.Log("MAX.NOD");

		var a = GetComponent<Animator>();
		a.SetBool("ActionNod", true);
	}
}
