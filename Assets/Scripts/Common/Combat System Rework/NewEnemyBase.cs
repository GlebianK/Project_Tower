using UnityEngine;
using UnityEngine.AI;

public class NewEnemyBase : MonoBehaviour
{
    [SerializeField] protected Health health;
    [SerializeField] protected CombatControllerEnemy combatController;
    [SerializeField] protected MotionEnemy motionEnemy;
    [SerializeField] protected float attackStartRange;
    [SerializeField] protected float maxPlayerDetectViewAngle = 15.0f;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected float speedAgent;
    [SerializeField] protected float angularSpeedAgent;

    protected Transform player;

    #region UNITY METHODS
    private void Update()
    {
        EnemyBehaviourCycle();
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

    protected virtual void EnemyBehaviourCycle()
    {
        if (player != null)
        {
            transform.rotation = RotateInDirectionOfPlayer();
            float distance = Vector3.Distance(transform.position, player.position);

            if (distance <= attackStartRange)
            {
                //agent.speed = 0f;
                if (combatController.IsAttackAllowed())
                {
                    float angle = Vector3.Angle(transform.forward, (player.position - transform.position).normalized);
                    if (angle < maxPlayerDetectViewAngle)
                    {
                        agent.speed = 0f;
                        AttackPlayer();
                    }
                }
            }
            else if (combatController.IsAttackAllowed())
            {
                ChasePlayer();
            }
        }
    }

    protected Quaternion RotateInDirectionOfPlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
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
        combatController.Attack();
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
            agent.angularSpeed = 0;
            motionEnemy.StopMovingAnimations();
            player = null;
        }
    }

    protected void Block()
    {
        if (combatController.IsBlockAllowed())
        {
            combatController.Block();
        }
    }
    #endregion

    #region EVENT CALLBACKS
    public void OnAttackStarted()
    {
        agent.speed = 0;
        agent.angularSpeed = 0;
        motionEnemy.StopMovingAnimations();
    }

    public void OnAttackEnded()
    {
        if (player != null)
            ChasePlayer();
    }

    public void OnBlockStarted()
    {
        health.ActivateDamageReduction();
    }

    public void OnBlockEnded()
    {
        health.DeactivateDamageReduction();
    }

    public void EnemyDied()
    {
        Destroy(gameObject);
    }
    #endregion
}
