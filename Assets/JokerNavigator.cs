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
		NextRandomTarget ();
	}

	void NextRandomTarget()
	{
		if (nav.remainingDistance < nav.stoppingDistance) 
		{
				if(wayPointIndex == patrolWayPoints.Length - 1)
					wayPointIndex = 0;
				else
					wayPointIndex++;

					nav.destination = patrolWayPoints[wayPointIndex].position;
		}
	}
}
