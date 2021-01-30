using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AIState { tracking, wander, foundPlayer}

[RequireComponent(typeof(NavMeshAgent))]
public class AI : MonoBehaviour
{
    public AIState currentState = AIState.foundPlayer;

    NavMeshAgent agent;

    public GameObject player;

    public float viewRange;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, viewRange);

        if (agent != null)
        {
            Gizmos.DrawSphere(agent.destination, gotToDestinationSize);
        }
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

    }

    public float wanderRange = 5f;
    public float gotToDestinationSize = 3f;

    // Update is called once per frame
    void Update()
    {

        if (CanSeePlayer())
        {
            currentState = AIState.foundPlayer;
        }
        else
        {
            currentState = AIState.wander;
        }

        switch (currentState)
        {
            case AIState.wander:
                if (Vector3.Distance(agent.destination, transform.position) < gotToDestinationSize)
                {
                    Debug.Log("Seaching for point");
                    Vector3 point;
                    if (RandomPointOnNavMesh(transform.position, wanderRange, out point))
                    {
                        //Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
                        agent.SetDestination(point);
                    }
                }
                break;

            default:
                agent.SetDestination(player.transform.position);
                break;
        }

        
    }

    bool CanSeePlayer()
    {
        //Raycast from here to the player
        RaycastHit hit;

        if (Physics.Raycast(transform.position,player.transform.position - transform.position, out hit,viewRange))
        {
            //If what we hit was the player
            if (hit.transform.tag == "Player")
            {
                return true;
            }
        }
        return false;
    }

    bool RandomPointOnNavMesh(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }
}
