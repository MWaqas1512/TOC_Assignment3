using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
using UnityEngine.UI;
public class Enemy_FSM : MonoBehaviour
{
    public enum ENEMY_STATE { PATROL, CHASE, ATTACK };
    [SerializeField]
    private ENEMY_STATE currentState;

    public ENEMY_STATE CurrentState
    {
        get
        {
            return currentState;

        }
        set
        {

            currentState = value;
            StopAllCoroutines();
            switch (currentState)
            {
                case ENEMY_STATE.PATROL:
                    StartCoroutine(EnemyPatrol());
                    break;
                case ENEMY_STATE.CHASE:
                    StartCoroutine(ChasePlayer());
                    break;
            }

        }
    }

    private CheckMyVision checkMyVision;
    private NavMeshAgent agent = null;
    public Transform playerTransform = null;
    private Transform patrolDestination = null;
    private Health playerHealth = null;
    public float maxDamage = 10f;
    private void Awake()
    {
        checkMyVision = GetComponent<CheckMyVision>();
        agent = GetComponent<NavMeshAgent>();
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        playerTransform = playerHealth.GetComponent<Transform>();
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] destinations = GameObject.FindGameObjectsWithTag("Dest");
        patrolDestination = destinations[Random.Range(0, destinations.Length)].GetComponent<Transform>();
        CurrentState = ENEMY_STATE.PATROL;
    }

    public IEnumerator EnemyPatrol()
    {
        while (currentState == ENEMY_STATE.PATROL)
        {
            checkMyVision.sensitity = CheckMyVision.enmSensitivity.HIGH;
            agent.isStopped = false;
            agent.SetDestination(patrolDestination.position);
            while (agent.pathPending)
            {
                yield return null;
            }

            if (checkMyVision.targetInSight)
            {
                agent.isStopped = true;
                CurrentState = ENEMY_STATE.CHASE;
                yield break;
            }
            yield return null;
        }

    }
    public IEnumerator ChasePlayer()
    {
        while (currentState == ENEMY_STATE.CHASE)
        {
            checkMyVision.sensitity = CheckMyVision.enmSensitivity.LOW;
            agent.isStopped = false;
            agent.SetDestination(checkMyVision.lastknownSighting);
            while (agent.pathPending)
            {
                yield return null;
            }
           
                playerHealth.HealthPoints -= maxDamage * Time.deltaTime;
                yield return null;
        }
        yield break;
    }
}
