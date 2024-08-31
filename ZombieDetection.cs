using UnityEngine.AI;
using UnityEngine;

public class ZombieDetection : MonoBehaviour
{
    public Transform player;
    public float chaseSpeed = 5f;
    public float patrolSpeed = 2f;
    private NavMeshAgent agent;
    private ZombiePatrol patrolScript;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        patrolScript = GetComponent<ZombiePatrol>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            patrolScript.enabled = false; // Patrol stop kar do
            agent.speed = chaseSpeed;
            agent.SetDestination(player.position); // Player ko target karo
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            patrolScript.enabled = true; // Patrol resume karo
            agent.speed = patrolSpeed;
            agent.SetDestination(patrolScript.waypoints[patrolScript.currentWaypointIndex].position);
        }
    }

    void Update()
    {
        if (!patrolScript.enabled && agent.remainingDistance < 0.5f && !agent.pathPending)
        {
            agent.SetDestination(player.position); // Agar player move kare, zombie follow kare
        }
    }
}
