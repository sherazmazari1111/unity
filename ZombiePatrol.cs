using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombiePatrol : MonoBehaviour
{
    public Transform[] waypoints; // Assign waypoints here
    public int currentWaypointIndex = 0;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(waypoints[currentWaypointIndex].position);
    }

    void Update()
    {
        if (agent.remainingDistance < 0.5f && !agent.pathPending)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }
}
