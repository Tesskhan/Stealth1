using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNav : MonoBehaviour
{
    public List<Waypoint> waypoints = new List<Waypoint>();
    public float speed = 3.5f;
    public float angularSpeed = 120f;
    public float waitTimeAtWaypoint = 2f;

    private NavMeshAgent agent;
    public int currentWaypointIndex = 0;
    private eEnemyState currentState = eEnemyState.none;
    private float waitTimer = 0f;

    private enum eEnemyState
    {
        none = 0,
        waiting = 1,
        rotating = 2,
        moving = 3
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        agent.angularSpeed = angularSpeed;

        if (waypoints.Count > 0)
        {
            currentState = eEnemyState.rotating;
            //agent.SetDestination(waypoints[currentWaypointIndex].transform.position);
        }
    }

    void Update()
    {
        switch (currentState)
        {
            case eEnemyState.moving:
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    currentState = eEnemyState.waiting;
                    waitTimer = waitTimeAtWaypoint;
                }
                break;

            case eEnemyState.waiting:
                waitTimer -= Time.deltaTime;
                if (waitTimer <= 0f)
                {
                    currentState = eEnemyState.rotating;
                }
                break;

            case eEnemyState.rotating:
                RotateTowardsNextWaypoint();
                break;
        }
    }

    private void RotateTowardsNextWaypoint()
    {
        Vector3 directionToNextWaypoint = waypoints[currentWaypointIndex].transform.position - transform.position;
        directionToNextWaypoint.y = 0; // Mantener la rotación en el plano horizontal
        Quaternion targetRotation = Quaternion.LookRotation(directionToNextWaypoint);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, angularSpeed * Time.deltaTime);

        // Si la rotación está completa, cambiar al estado de movimiento
        if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
        {
            agent.SetDestination(waypoints[currentWaypointIndex].transform.position);
            currentState = eEnemyState.moving;
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
        }
    }
}