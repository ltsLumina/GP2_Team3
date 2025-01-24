#region
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using VInspector;
using Random = UnityEngine.Random;
#endregion

public class EnemyAI : MonoBehaviour
{
    public enum Brain // Agent Type in Unity's NavMeshAgent
    {
        [InspectorName("Default")]
        Humanoid, // Will wander aimlessly until the player is within detection range
        MiniBoss,     // Will not wander. A mini boss is a tougher default enemy. It has the same properties, but has a boss healthbar.
        Boss        // Will not wander. It cannot move but can attack the player from a distance using MMO-style abilities (AoEs)
    }

    [Header("Agent Type")]
    [SerializeField] Brain brain = Brain.Humanoid;

    [Header("AI Attributes")]
    [SerializeField] int detectionRange = 10;
    [HideIf(nameof(brain), Brain.Boss)]
    [SerializeField] float delayBetweenMoves = 5f;
    [EndIf]
    [SerializeField] float chaseDuration = 5;
    [Space(5)]
    [SerializeField] float speed = 3.5f;
    [SerializeField] float rotationSpeed = 180f;
    [SerializeField] float acceleration = 8;
    [SerializeField] float attackRange = 2;
    [Space(5)]
    [SerializeField] int damage = 10;
    [SerializeField] float attackCooldown = 2.5f;

    readonly static int Offset = Animator.StringToHash("Offset");
    readonly static int Multiplier = Animator.StringToHash("Multiplier");
    
    NavMeshAgent agent;
    Animator anim;
    Transform target;

    #region API
    public void SetTarget(Transform newTarget) => target = newTarget;
    #endregion

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
    }

    // target is by default the player, but it could be changed if we want
    void Start()
    {
        target = FindFirstObjectByType<Player>().transform;

        agent.autoRepath = false;
        agent.ResetPath();
        
         // set the animator's speed and offset based on the enemy's speed
         anim.SetFloat(Offset, Random.Range(0f, 1f));
    }

    void OnValidate()
    {
        Debug.Assert(attackRange < detectionRange, "Attack range must be less than detection range!");
        if (attackRange > detectionRange) attackRange = detectionRange - 1;
    }

    float distance;
    Coroutine wanderCoroutine;
    float chaseTimer; // how long the enemy has been chasing the player
    
    void Update()
    {
        switch (brain)
        {
            case Brain.Humanoid:
                agent.agentTypeID = 0;
                break;
            
            case Brain.MiniBoss:
                agent.agentTypeID = -1923039037; // unity.
                break;
            
            case Brain.Boss:
                agent.agentTypeID = -902729914; // unity.
                break;
        }
        
        anim.SetFloat(Multiplier, agent.velocity.magnitude / agent.speed);

        // Set the agent type based on the brain
        distance = Vector3.Distance(target.position, transform.position);

        (float speed, float rotationSpeed, float acceleration, float stoppingDistance)
            attributes = (speed, rotationSpeed, acceleration, attackRange);

        switch (brain)
        {
            case Brain.Humanoid:
                // If the player is within the enemy's detection range, the enemy will move towards the player
                if (distance <= detectionRange)
                {
                    if (wanderCoroutine != null)
                    {
                        StopCoroutine(wanderCoroutine);
                        wanderCoroutine = null;
                    }

                    Engage(attributes);
                }
                // Keep chasing after the player has left the detection range
                else if (chaseTimer > 0) Engage(attributes);
                // If the player is outside the enemy's detection range and not chasing, the enemy will wander aimlessly
                else
                {
                    // If the player is outside the enemy's detection range and not chasing, the enemy will wander aimlessly
                    wanderCoroutine ??= StartCoroutine(Wander());
                }
                break;

            case Brain.MiniBoss:
                if (distance <= detectionRange)
                {
                    Engage(attributes);
                }
                // Keep chasing after the player has left the detection range. Wont stop until the player is dead or leaves the load zone.
                else if (chaseTimer > 0) Engage(attributes);
                break;

            case Brain.Boss:
                if (distance <= detectionRange)
                {
                    Engage(attributes);
                }
                break;

            default: // Should never happen because we're using a serialized enum, but Unity likes to throw exceptions for no apparent reason
                throw new ArgumentOutOfRangeException();
            
        }
    }

    void Engage((float speed, float rotationSpeed, float acceleration, float stoppingDistance) attributes)
    {
        var path = new NavMeshPath();
        agent.CalculatePath(target.position, path);
        agent.path = path;
        
        agent.speed = attributes.speed;
        agent.angularSpeed = attributes.rotationSpeed;
        agent.acceleration = attributes.acceleration;
        agent.stoppingDistance = attributes.stoppingDistance;

        bool success = agent.SetDestination(target.position);

        if (!success)
        {
            Debug.LogError("Failed to set destination! \nPlayer is probably out of bounds.");
            wanderCoroutine = StartCoroutine(Wander());
        }

        // check if player is within attack range
        if (nearTarget)
        {
            Attack();
        }
        else
        {
            attackCoroutine = null; // Can't attack while chasing.
            Chase();
        }
    }
    
    Coroutine attackCoroutine;

    void Attack()
    {
        //anim.SetTrigger("Attack");
        attackCoroutine ??= StartCoroutine(Cooldown());
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        
        target.TryGetComponent(out Player player);
        player.TakeDamage(damage);
        
        attackCoroutine = null;
    }

    bool nearTarget => agent.remainingDistance <= agent.stoppingDistance;
    
    void Chase()
    {
        chaseTimer += Time.deltaTime;
        //Debug.Log($"Chasing player! | {chaseTimer.RoundToInt()}s elapsed.");
        
        if (chaseTimer >= chaseDuration && !nearTarget)
        {
            chaseTimer = 0;
            agent.ResetPath();
            wanderCoroutine = StartCoroutine(Wander());
        }
    }

    IEnumerator Wander()
    {
        while (true)
        {
            Vector3 randomPosition = RandomNavSphere(transform.position, distance, -1);
            agent.stoppingDistance = 0;
            agent.SetDestination(randomPosition);
            yield return new WaitForSeconds(delayBetweenMoves);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    #region Utility
    /// <param name="origin"> The origin position </param>
    /// <param name="distance"> The distance from the origin </param>
    /// <param name="layerMask"> The layer mask to use </param>
    /// <returns> A random position within the NavMesh </returns>
    public static Vector3 RandomNavSphere(Vector3 origin, float distance, int layerMask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;

        randomDirection += origin;

        NavMesh.SamplePosition(randomDirection, out NavMeshHit navHit, distance, layerMask);

        return navHit.position;
    }
    #endregion
}
