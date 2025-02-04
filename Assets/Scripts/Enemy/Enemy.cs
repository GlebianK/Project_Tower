using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private AttackEnemy attack;
    [SerializeField] private MotionEnemy motionEnemy;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float speedAgent;
    [SerializeField] private float attackStartRange;

    private Transform player;
    private readonly float smoothnessOfRotation = 0.015f; // отвечает за плавность поворота мобов в стороны игрока; значения от 0f до 1f

    private void Update()
    {
        if (player != null)
        {
            transform.rotation = RotateInDirectionOfPlayer(smoothnessOfRotation);
            float distance = Vector3.Distance(transform.position, player.position);

            if (distance <= attackStartRange)
            {
                AttackPlayer();
            }
            else
            {
                ChasePlayer();
            }
        }
    }

    private Quaternion RotateInDirectionOfPlayer(float smoothness)
    {
        Vector3 directionToPlayer = player.position - transform.position;
        Quaternion rotationToPlayer = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(directionToPlayer), smoothness);

        return rotationToPlayer;
    }

    private void ChasePlayer()
    {
        agent.speed = speedAgent;
        motionEnemy.StartMovingAnimations();
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        agent.speed = 0;
        motionEnemy.StopMovingAnimations();
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

    public void EnemyDied()
    {
        Destroy(gameObject);
    }

}
