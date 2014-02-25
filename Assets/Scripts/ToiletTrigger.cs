using UnityEngine;
using System.Collections;

public class ToiletTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "Player")
        {
            GameObject o = coll.transform.FindChild("Cylinder").gameObject;
            o.renderer.enabled = true;
        }
    }

    void OnTriggerExit(Collider coll)
    {
        if (coll.tag == "Player")
        {
            GameObject o = coll.transform.FindChild("Cylinder").gameObject;
            o.renderer.enabled = false;
        }
    }
}
