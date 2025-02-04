using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private AttackEnemy attack;
    [SerializeField] private MotionEnemy motionEnemy;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float attackStartRange;

    private float speedAgent;
    private float angularSpeedAgent;
    private Transform player;
    private readonly float smoothnessOfRotation = 0.015f; // отвечает за плавность поворота мобов в стороны игрока; значения от 0f до 1f

    private void Start()
    {
        speedAgent = agent.speed;
        angularSpeedAgent = agent.angularSpeed;
    }

    private void Update()
    {
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            if (distance <= attackStartRange)
            {
                transform.rotation = RotateInDirectionOfPlayer(smoothnessOfRotation);
                agent.speed = 0;
                if (attack.CanAttack())
                {
                    float angle = Vector3.Angle(transform.forward, (player.position - transform.position).normalized);
                    if (angle < 15.0f)
                    {
                        AttackPlayer();
                    }
                }
            }
            else if (attack.CanAttack())
            {
                ChasePlayer();
            }
        }
    }

    private Quaternion RotateInDirectionOfPlayer(float smoothness)
    {
        Vector3 directionToPlayer = player.position - transform.position;
        //Quaternion rotationToPlayer = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(directionToPlayer), smoothness);
        Quaternion rotationToPlayer = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(directionToPlayer), angularSpeedAgent * Time.deltaTime);
        return rotationToPlayer;
    }

    private void ChasePlayer()
    {
        agent.speed = speedAgent;
        agent.angularSpeed = angularSpeedAgent;
        motionEnemy.StartMovingAnimations();
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        attack.TryAttack();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            agent.speed = 0;
            motionEnemy.StopMovingAnimations();
            player = null;
        }
    }

    public void AttackStarted()
    {
        agent.speed = 0;
        agent.angularSpeed = 0;
        motionEnemy.StopMovingAnimations();
    }

    public void AttackEnded()
    {
        ChasePlayer();
    }

    public void EnemyDied()
    {
        Destroy(gameObject);
    }

}
