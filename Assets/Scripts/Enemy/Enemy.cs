using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

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
    private void Start()
    {
        speedAgent = agent.speed;
    }
    private void Update()
    {
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance < attackStartRange)
            {
                transform.LookAt(player.position);
                agent.speed = 0;
                motionEnemy.StopMovingAnimations();
                attack.TryAttack();
                
            }
            if (distance > attackStartRange)
            {
                agent.speed = speedAgent;
                motionEnemy.StartMovingAnimations();
                agent.SetDestination(player.position);
            }
        }
    }
    public void EnemyDied()
    {
        Destroy(gameObject);
    }

}
