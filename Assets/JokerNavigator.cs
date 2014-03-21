using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JokerNavigator : MonoBehaviour {
	
	public float patrolWaitTime = 1f;
	public Transform[] patrolWayPoints;
	
	private NavMeshAgent nav;
	private float patrolTimer;
	private int wayPointIndex;
	
	// Use this for initialization
	void Start () 
	{
		nav = GetComponent<NavMeshAgent> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		RandomPatrolling ();
	}
	
	void Patrolling()
	{
		if (nav.remainingDistance < nav.stoppingDistance) {
			patrolTimer += Time.deltaTime;
			
			if (patrolTimer >= patrolWaitTime) {
				if (wayPointIndex == patrolWayPoints.Length - 1)
					
					wayPointIndex = 0;
				else
					
					wayPointIndex++;
				
				patrolTimer = 0;
				
			}
		} else
			
			patrolTimer = 0;
		
		nav.destination = patrolWayPoints [wayPointIndex].position;
	}
	
	void RandomPatrolling()
	{
		if (nav.remainingDistance < nav.stoppingDistance) 
		{
			patrolTimer += Time.deltaTime;
			
			int randomNumber = Random.Range(0, patrolWayPoints.Length);
			
			nav.destination = patrolWayPoints[randomNumber].position;
		}
	}
	
	void OnTriggerEnter(Collider other)
	{
		Debug.Log ("Something has entered");
		if (other.gameObject.tag == "Player")
		{
			//Debug.Log("Player has entered");
			
			nav.speed = 0;
			
			//Debug.Log("Joker should be still");
		}
	}
	
	void OnTriggerExit(Collider player)
	{
		if (player.gameObject.tag == "Player")
		{
			//Debug.Log("Player has left Joker all alone... :(");
			
			nav.speed = 3.5f;
			
			//Debug.Log("Joker should be still");
		}
	}
}