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

    public Animator animator;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, viewRange);
        
    }

    private void OnDrawGizmosSelected()
    {
        if (agent != null)
        {
            Gizmos.DrawSphere(agent.destination, gotToDestinationSize);
        }

        Gizmos.color = Color.red;
        Vector3 pos = wanderPosition;

        if (wanderPosition == Vector3.zero)
        {
            pos = transform.position;
        }

        Gizmos.DrawWireSphere(pos, wanderRange);
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        wanderPosition = transform.position;

        animator.SetBool("Moving", true);
    }

    public float wanderRange = 5f;
    public float gotToDestinationSize = 3f;

    Vector3 wanderPosition = Vector3.zero;

    

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
                    if (RandomPointOnNavMesh(wanderPosition, wanderRange, out point))
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
                animator.SetBool("Player In View", true);
                return true;
            }
        }
        animator.SetBool("Player In View", false);
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
