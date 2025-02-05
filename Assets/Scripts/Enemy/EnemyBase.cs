using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] protected Health health;
    [SerializeField] protected AttackEnemy attack;
    [SerializeField] protected MotionEnemy motionEnemy;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected float speedAgent;
    [SerializeField] protected float attackStartRange;

    protected Transform player;
    protected readonly float smoothnessOfRotation = 0.015f; // отвечает за плавность поворота мобов в стороны игрока; значения от 0f до 1f

    private void Update()
    {
        EnemyBehaviorCycle();
    }

    protected virtual void EnemyBehaviorCycle()
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

    private void OnTriggerEnter(Collider other)
    {
        DetectPlayer(other);
        /*
        if (other.CompareTag("Player"))
        {
            player = other.transform;
        }
        */
    }

    private void OnTriggerExit(Collider other)
    {
        LosePlayer(other);
        /*
        if (other.CompareTag("Player"))
        {
            agent.speed = 0;
            motionEnemy.StopMovingAnimations();
            player = null;
        }
        */
    }

    protected Quaternion RotateInDirectionOfPlayer(float smoothness)
    {
        Vector3 directionToPlayer = player.position - transform.position;
        Quaternion rotationToPlayer = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(directionToPlayer), smoothness);

        return rotationToPlayer;
    }

    protected void ChasePlayer()
    {
        agent.speed = speedAgent;
        motionEnemy.StartMovingAnimations();
        agent.SetDestination(player.position);
    }

    protected void AttackPlayer()
    {
        agent.speed = 0;
        motionEnemy.StopMovingAnimations();
        attack.TryAttack();
    }

    protected void DetectPlayer(Collider detectedObject)
    {
        if (detectedObject.CompareTag("Player"))
        {
            player = detectedObject.transform;
        }
    }

    protected void LosePlayer(Collider detectedObject)
    {
        if (detectedObject.CompareTag("Player"))
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
