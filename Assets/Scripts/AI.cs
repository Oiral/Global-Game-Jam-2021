using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AIState { tracking, wander, foundPlayer, surveying}

[RequireComponent(typeof(NavMeshAgent))]
public class AI : MonoBehaviour
{
    public AIState currentState = AIState.foundPlayer;

    NavMeshAgent agent;
    float startingAgentSpeed;
    float startingAgentAcceleration;

    public GameObject player;

    public float viewRange;

    public Animator animator;

    bool trackingPlayer;

    public LayerMask rayMask;

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

        startingAgentSpeed = agent.speed;
        startingAgentAcceleration = agent.acceleration;

        wanderPosition = transform.position;

        animator.SetBool("Moving", true);
    }

    public float wanderRange = 5f;
    public float gotToDestinationSize = 3f;

    Vector3 wanderPosition = Vector3.zero;

    float surveyTimer;
    float surveyCoolDown;

    float chargeTimer;
    Vector3 lastSeenPlayerPos;

    // Update is called once per frame
    void Update()
    {
        if (surveyCoolDown >= 0)
        {
            surveyCoolDown -= Time.deltaTime;
        }

        /*
        if (CanSeePlayer() || trackingPlayer)
        {
            currentState = AIState.foundPlayer;
        }
        else
        {
            if (Vector3.Distance(transform.position, player.transform.position) > 20 || surveyCoolDown >= 0)
            {
                currentState = AIState.wander;
            }
            else
            {
                currentState = AIState.surveying;
            }
            
        }*/

        switch (currentState)
        {
            case AIState.wander:

                
                if (Vector3.Distance(agent.destination, transform.position) < gotToDestinationSize)
                {
                    if (trackingPlayer)
                    {
                        animator.SetBool("Player In View", false);
                        trackingPlayer = false;
                        agent.speed = startingAgentSpeed;
                        agent.acceleration = startingAgentAcceleration;
                        currentState = AIState.surveying;
                    }
                    else
                    {
                        Debug.Log("Seaching for point");
                        Vector3 point;
                        if (RandomPointOnNavMesh(wanderPosition, wanderRange, out point))
                        {
                            //Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
                            animator.SetBool("Moving", true);
                            agent.SetDestination(point);
                        }
                    }
                }

                if (Vector3.Distance(transform.position, player.transform.position) < 20 && surveyCoolDown <= 0 && trackingPlayer == false)
                {
                    currentState = AIState.surveying;
                    break;
                }
                break;

            case AIState.surveying:
                surveyTimer += Time.deltaTime;

                agent.destination = transform.position;
                animator.SetBool("Moving", false);

                if (CanSeePlayer())
                {
                    surveyTimer = 0;
                    currentState = AIState.foundPlayer;
                    break;
                }

                if (surveyTimer > 5)
                {
                    surveyCoolDown = Random.Range(3f,5f);
                    surveyTimer = 0;
                    currentState = AIState.wander;
                }
                break;

            case AIState.foundPlayer:
                //we want to let the minotaur roar
                animator.SetBool("Moving", false);
                animator.SetBool("Player In View", true);

                agent.SetDestination(transform.position + (player.transform.position - transform.position).normalized * 0.1f);

                if (trackingPlayer == false)
                {
                    if (CanSeePlayer())
                    {
                        lastSeenPlayerPos = player.transform.position;
                    }

                    

                    chargeTimer += Time.deltaTime;
                    if (chargeTimer < 2)
                    {
                        //If we are not ready to charge
                        
                        break;
                    }
                }

                //We can only get here when it is time to charge

                chargeTimer = 0;
                //We want to set the destination
                //We want to charge past the last seen player pos
                RaycastHit hit;

                Vector3 dir = lastSeenPlayerPos - transform.position;
                dir.y = 0;

                Debug.DrawRay(transform.position, (dir) * 40f, Color.red,1f);

                if (Physics.Raycast(transform.position, dir, out hit, 20f,rayMask))
                {
                    Debug.DrawRay(transform.position, hit.point - transform.position, Color.red, 1f);

                    agent.SetDestination(hit.point);
                }
                else
                {
                    //If we don't hit any walls, Dont go more than 20 units away
                    agent.SetDestination(transform.position + dir.normalized * 20f);
                    trackingPlayer = true;
                }

                surveyCoolDown = 2;
                animator.SetBool("Moving", true);
                trackingPlayer = true;

                agent.speed = startingAgentSpeed * 1.2f;
                agent.acceleration = startingAgentAcceleration * 1.2f;

                currentState = AIState.wander;
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
