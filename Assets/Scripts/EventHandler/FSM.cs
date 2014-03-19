using UnityEngine;
using System.Collections;

public class FSM : MonoBehaviour {

	protected virtual void Initialize() { }
	protected virtual void FSMUpdate() { }
	// Use this for initialization
	void Start () 
	{
		Initialize ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		FSMUpdate ();	
	}
}
