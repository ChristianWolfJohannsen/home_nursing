using UnityEngine;
using System.Collections;

public class QuestObjectSpin : MonoBehaviour {
	public Vector3 speed=new Vector3(0,45,0);
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(speed*Time.deltaTime);
	}
}
