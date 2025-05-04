using System.Collections;
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

    [SerializeField] private float destructionTimer = 5.5f;

    protected Transform player;
    protected float playerCCRadius = 0f;
    protected bool isAlive = true;

    protected EnemyType rewardResolveType;

    #region UNITY METHODS
    protected void Awake()
    {
        isAlive = true;
        playerCCRadius = 0;
    }

    private void Update()
    {
        if (isAlive)
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

    #region USER METHODS

    protected virtual void EnemyBehaviourCycle()
    {
        if (player != null && !combatController.IsRecoveringAfterHit())
        {
            if (playerCCRadius == 0f)
            {
                if (player.gameObject.TryGetComponent<CharacterController>(out CharacterController player_cc))
                    playerCCRadius = player_cc.radius;
                else
                    Debug.LogError("Cannot extract player's CC!");
            }

            transform.rotation = RotateInDirectionOfPlayer();
            float distance = Vector3.Distance(transform.position, player.position) - playerCCRadius; // Radius to attack capsule surface
            //Debug.Log($"distance: {distance}, attack start distance: {attackStartRange}");
            if (distance <= attackStartRange)
            {
                StopEnemyMovement();
                if (combatController.IsAttackAllowed())
                {
                    float angle = Vector3.Angle(transform.forward, (player.position - transform.position).normalized);
                    if (angle < maxPlayerDetectViewAngle)
                    {
                        agent.speed = 0f;
                        agent.velocity = Vector3.zero;
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

    protected void StopEnemyMovement()
    {
        agent.speed = 0;
        agent.angularSpeed = 0;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        motionEnemy.StopMovingAnimations();
    }

    protected void AllowEnemyMovement()
    {
        agent.speed = speedAgent;
        agent.angularSpeed = angularSpeedAgent;
        agent.isStopped = false;
        //motionEnemy.StartMovingAnimations();
    }

    protected void ChasePlayer()
    {
        if (player != null)
        {
            Vector3 playerCapsuleSurfacePos = GetPointOnSurface(player, gameObject.transform, playerCCRadius);
            agent.SetDestination(player.position + playerCapsuleSurfacePos);  // CHANGED !!!!!!!!!!!!!!!!!!!!!! (����: player.position)
            AllowEnemyMovement();
            motionEnemy.StartMovingAnimations();
        }
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
            StopEnemyMovement();
            player = null;
            playerCCRadius = 0f;
        }
    }

    protected void Block()
    {
        if (combatController.IsBlockAllowed())
        {
            combatController.Block();
        }
    }

    private Vector3 GetPointOnSurface(Transform start, Transform target, float distanceFromCenter)
    {
        return (target.position - start.position).normalized * distanceFromCenter;
    }
    #endregion

    #region EVENT CALLBACKS
    public void OnAttackStarted()
    {
        StopEnemyMovement();
    }

    public void OnAttackEnded()
    {
        /*
        if (player != null)
            ChasePlayer();
        */
        Debug.Log($"Enemy ({gameObject.name}) attack finished!");
    }

    public void OnBlockStarted()
    {
        health.ActivateDamageReductionByBlock();
    }

    public void OnBlockEnded()
    {
        health.DeactivateDamageReductionByBlock();
    }

    public void OnGetHit()
    {
        StopEnemyMovement();
        agent.ResetPath();
    }

    public void EnemyDied()
    {
        isAlive = false;
        player = null;
        StopEnemyMovement();
        
        if (TryGetComponent<CapsuleCollider>(out CapsuleCollider capsule))
        {
            capsule.enabled = false;
        }

        if (TryGetComponent<BoxCollider>(out BoxCollider box))
        {
            box.enabled = false;
        }

        StartCoroutine(DestroyAfterDeathCoroutine(gameObject));

        GameplayManager gm = FindAnyObjectByType<GameplayManager>();
        if (gm)
        {
            gm.OnEnemyKilled(rewardResolveType);
        }
        else
        {
            Debug.LogWarning("Place GameplayManager script on scene to collect score reward for enemy "
                + rewardResolveType.ToString());
        }
        //Destroy(gameObject);
    }

    private IEnumerator DestroyAfterDeathCoroutine(GameObject objectToDestroy)
    {
        yield return new WaitForSeconds(destructionTimer);
        Destroy(objectToDestroy);
    }
    #endregion

    //method for group fights 
    public void SetPlayer(Transform externalLinkToPlayer)
    {
        player = externalLinkToPlayer;
    }
}
