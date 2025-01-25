using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private Attack attack;
    [SerializeField] private MotionEnemy motionEnemy;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float attackStartRange;
    [Header("PlayerTransform")]
    [SerializeField] private Transform player;
    private float speedAgent;
    private float accelerationAgent;
    private float angularSpeedAgent;
    private void Start()
    {
        speedAgent = agent.speed;
        accelerationAgent = agent.acceleration;
        angularSpeedAgent = agent.angularSpeed;
    }
    private void Update()
    {
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            if (distance < attackStartRange)
            {
                agent.SetDestination(transform.position);
                motionEnemy.StopMovingAnimations();
                attack.TryAttack();
            }
            if (distance > attackStartRange)
            {
                motionEnemy.StartMovingAnimations();
                attack.StopAttackAnimations();
                agent.SetDestination(player.position);
                agent.speed = speedAgent;
                agent.acceleration = accelerationAgent;
                agent.angularSpeed = angularSpeedAgent;
            }
        }
    }
    public void EnemyDied()
    {
        Destroy(gameObject);
    }

}
