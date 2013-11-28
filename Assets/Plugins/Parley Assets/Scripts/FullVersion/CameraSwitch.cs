using UnityEngine;
using System.Collections;

public class CameraSwitch : MonoBehaviour {
	private Quaternion startRotation;
	private Vector3 startPosition;
	private float startDepth;
	private float startFieldOfView;
	private float startNear;
	private float startFar;
	
	private bool done=false;

	// Use this for initialization
	void Start () {
		startRotation=transform.rotation;
		startPosition=transform.position;
		startDepth=camera.depth;
		startFieldOfView=camera.fieldOfView;
		startNear=camera.near;
		startFar=camera.far;
	}
	
	// Update is called once per frame
	void Update () {
		if (!done){
			done=true;
			
			transform.rotation=Quaternion.Lerp(transform.rotation,startRotation,Time.deltaTime/10f);
			done=done&transform.rotation.Equals(startRotation);
			
			transform.position=Vector3.Lerp(transform.position,startPosition,Time.deltaTime/10f);
			done=done&transform.position.Equals(startPosition);
			
			camera.depth=Mathf.Lerp(camera.depth,startDepth,Time.deltaTime/10f);
			done=done&camera.depth.Equals(startDepth);
			
			camera.fieldOfView=Mathf.Lerp(camera.fieldOfView,startFieldOfView,Time.deltaTime/10f);
			done=done&camera.fieldOfView.Equals(startFieldOfView);
			
			camera.near=Mathf.Lerp(camera.near,startNear,Time.deltaTime/10f);
			done=done&camera.near.Equals(startNear);
			
			camera.far=Mathf.Lerp(camera.far,startFar,Time.deltaTime/10f);
			done=done&camera.far.Equals(startFar);
		}
	}
	
	public void SwitchCamera(){
		Camera c=Camera.main;
		
		transform.rotation=c.transform.rotation;
		transform.position=c.transform.position;
		camera.depth=c.depth;
		camera.fieldOfView=c.fieldOfView;
		camera.near=c.near;
		camera.far=c.far;
		
		done=false;

		c.gameObject.SetActive(false);		
		gameObject.SetActive(true);
	}
}
