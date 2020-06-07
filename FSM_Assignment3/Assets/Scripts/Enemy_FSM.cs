using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class Enemy_FSM : MonoBehaviour
{

    public enum ENEMY_STATE
    {
        EnemyPATROL,
        EnemyCHASE,
        EnemyKilling,
        PlayerDead,
        GameOver
    };

    [SerializeField] private ENEMY_STATE currentState;

    public ENEMY_STATE CurrentState
    {
        get { return currentState; }
        set
        {
            currentState = value;
            StopAllCoroutines();
            switch (currentState)
            {
                case ENEMY_STATE.EnemyPATROL:
                    StartCoroutine(EnemyMoving());
                    break;
                case ENEMY_STATE.EnemyCHASE:
                    StartCoroutine(ChasePlayer());
                    break;
                case ENEMY_STATE.EnemyKilling:
                    StartCoroutine(EnemyKilling());
                    break;
                case ENEMY_STATE.PlayerDead:
                    StartCoroutine(playerDead());
                    break;
                case ENEMY_STATE.GameOver:
                    StartCoroutine(GameOver());
                    break;
            }

        }
    }

    private CheckMyVision checkMyVision;
    private NavMeshAgent agent = null;
    private Transform patrolDestination = null;
    private Health playerHealth = null;
    private Enemy_Health enemyHealth = null;
    public float maxDamage = 10f;

    private void Awake()
    {
        checkMyVision = GetComponent<CheckMyVision>();
        agent = GetComponent<NavMeshAgent>();
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        enemyHealth = GameObject.FindGameObjectWithTag("enemy").GetComponent<Enemy_Health>();

    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] destinations = GameObject.FindGameObjectsWithTag("Dest");

        patrolDestination = destinations[Random.Range(0, destinations.Length)].GetComponent<Transform>();
        CurrentState = ENEMY_STATE.EnemyPATROL;
    }

    public IEnumerator EnemyMoving()
    {
        while (currentState == ENEMY_STATE.EnemyPATROL)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
            float dist = Vector3.Distance(agent.transform.position,
                GameObject.FindGameObjectWithTag("Player").transform.position);
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
                CurrentState = ENEMY_STATE.EnemyCHASE;
                yield break;
            }

            if (dist < 2.0f)
            {
                enemyHealth.EnemyHealthPoints -= maxDamage * Time.deltaTime;
                agent.SetDestination(transform.position);
            }

            if (enemies.Length < 1)
            {

            }

            yield return null;
        }

    }

    public IEnumerator ChasePlayer()
    {
        while (currentState == ENEMY_STATE.EnemyCHASE)
        {
            checkMyVision.sensitity = CheckMyVision.enmSensitivity.LOW;
            agent.isStopped = false;
            agent.SetDestination(checkMyVision.lastknownSighting);
            while (agent.pathPending)
            {
                yield return null;
            }

            if (!checkMyVision.targetInSight)
            {
                agent.isStopped = true;
                CurrentState = ENEMY_STATE.EnemyPATROL;
                yield break;
            }

            CurrentState = ENEMY_STATE.EnemyKilling;
        }

        yield break;
    }

    public IEnumerator EnemyKilling()
    {
        while (currentState == ENEMY_STATE.EnemyKilling)
        {
            checkMyVision.sensitity = CheckMyVision.enmSensitivity.LOW;
            agent.isStopped = false;
            agent.SetDestination(checkMyVision.lastknownSighting);
            while (agent.pathPending)
            {
                yield return null;
            }

            if (!checkMyVision.targetInSight)
            {
                agent.isStopped = true;
                CurrentState = ENEMY_STATE.EnemyPATROL;
                yield break;
            }

            playerHealth.HealthPoints -= maxDamage * Time.deltaTime;
            if (playerHealth.HealthPoints <= 0)
            {
                CurrentState = ENEMY_STATE.PlayerDead;
                yield break;
            }
            yield return null;
        }

        yield break;
    }

    [SerializeField] private float life = 3;
    public IEnumerator playerDead()
    {
        if (playerHealth.HealthPoints <= 0)
        {
            life--;
            if (life >= 1)
            {
                playerHealth.HealthPoints = 100f;
            }
            else
            {
                CurrentState = ENEMY_STATE.GameOver;
                Destroy(gameObject);
            }
            if (!checkMyVision.targetInSight)
            {
                agent.isStopped = true;
                CurrentState = ENEMY_STATE.EnemyPATROL;
                yield break;
            }
            else
            {
                agent.isStopped = true;
                CurrentState = ENEMY_STATE.EnemyKilling;
                yield break;
            }
        }
    }

    public IEnumerator GameOver()
    {
        yield break;
    }
}
