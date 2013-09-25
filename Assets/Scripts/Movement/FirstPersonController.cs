using UnityEngine;
using System.Collections;

[RequireComponent (typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour {
	
	public float movementspeed = 4.0f;
	public float mousesense = 2.0f;
	public float updownRange = 60.0f;
	
	float verticalRotation = 0;
	
	CharacterController cc;
	// Use this for initialization
	void Start () 
	{
		cc = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () 
	{	
		//Rotataion
		if(Input.GetMouseButton(1))
		{
			Screen.lockCursor = true; // makes cursor invisible
			float rotLeftRight = Input.GetAxis("Mouse X") * mousesense;
			transform.Rotate(0,rotLeftRight,0);
		
			verticalRotation -= Input.GetAxis("Mouse Y") * mousesense;
			verticalRotation = Mathf.Clamp(verticalRotation, -updownRange, updownRange);
			Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0 , 0);
		}
		if(!Input.GetMouseButton(1) && Screen.lockCursor == true)
		{
			Screen.lockCursor = false; // makes cursor visible
		}
		
		//Movement
		
		float forwardSpeed = Input.GetAxis("Vertical") * movementspeed;
		float sideSpeed = Input.GetAxis("Horizontal") * movementspeed;
		
		Vector3 speed = new Vector3(sideSpeed,0,forwardSpeed);
		
		speed = transform.rotation * speed;
		
		cc.Move(speed * Time.deltaTime);
	}
}
