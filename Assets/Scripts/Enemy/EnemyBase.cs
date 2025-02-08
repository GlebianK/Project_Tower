using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] protected Health health;
    [SerializeField] protected AttackEnemy attack;
    [SerializeField] protected MotionEnemy motionEnemy;
    [SerializeField] protected float attackStartRange;
    [SerializeField] protected float maxPlayerDetectViewAngle = 15.0f;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected float speedAgent;
    [SerializeField] protected float angularSpeedAgent;

    protected Transform player;
    protected readonly float smoothnessOfRotation = 0.015f; // отвечает за плавность поворота мобов в сторону игрока; значения от 0f до 1f

    #region UNITY METHODS
    private void Update()
    {
        EnemyBehaviorCycle();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        DetectPlayer(other);
    }

    private void OnTriggerExit(Collider other)
    {
        LosePlayer(other);
    }
    #endregion

    #region USER PROTECTED METHODS

    protected virtual void EnemyBehaviorCycle()
    {
        if (player != null)
        {
            transform.rotation = RotateInDirectionOfPlayer(smoothnessOfRotation);
            float distance = Vector3.Distance(transform.position, player.position);

            if (distance <= attackStartRange)
            {
                agent.speed = 0f;
                if (attack.CanAttack())
                {
                    float angle = Vector3.Angle(transform.forward, (player.position - transform.position).normalized);
                    if (angle < maxPlayerDetectViewAngle)
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

    protected Quaternion RotateInDirectionOfPlayer(float smoothness)
    {
        Vector3 directionToPlayer = player.position - transform.position;
        //Quaternion rotationToPlayer = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(directionToPlayer), smoothness);
        Quaternion rotationToPlayer = Quaternion.RotateTowards(transform.rotation, 
            Quaternion.LookRotation(directionToPlayer), angularSpeedAgent * Time.deltaTime);

        return rotationToPlayer;
    }

    protected void ChasePlayer()
    {
        agent.speed = speedAgent;
        agent.angularSpeed = angularSpeedAgent;
        motionEnemy.StartMovingAnimations();
        agent.SetDestination(player.position);
    }

    protected void AttackPlayer()
    {
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
    #endregion

    #region EVENT CALLBACKS
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
    #endregion
}
