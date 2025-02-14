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
    [Tooltip("The amount of time the enemy waits around at each wandering position, before moving to the next one.")]
    [SerializeField] float delayBetweenMoves = 5f;
    [EndIf]
    [Tooltip("The amount of time the enemy will chase the player after the player has left the detection range. \nReturns to wandering after this time.")]
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

    Enemy enemy;
    NavMeshAgent agent;
    Animator anim;
    Transform target;

    void Awake()
    {
        enemy = GetComponent<Enemy>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
    }
    
    void OnEnable()
    {
        Health.OnDeath += OnPlayerDeath;
        target = FindFirstObjectByType<Player>().transform;
    }

    void OnDisable() => Health.OnDeath -= OnPlayerDeath;

    void OnPlayerDeath()
    {
        agent.ResetPath();
        
        StopAllCoroutines();
        wanderCoroutine = StartCoroutine(Wander());
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
    
    bool playerIsDead => FindAnyObjectByType<Health>().CurrentHealth <= 0;

    public void UpdateAI()
    {
        if (agent) agent.isStopped = GameManager.Instance.CurrentState is PauseState;
        
        if (enemy.IsDead)
        {
            StopAllCoroutines();
            wanderCoroutine = null;
            return;
        }
        
        if (!enabled) return;
        if (playerIsDead) return;
        
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
                    if (NearTarget && HasLineOfSight) Engage(attributes);
                    else if (HasLineOfSight)
                    {
                        Chase();
                        
                        if (chaseTimer > 0) Engage(attributes);
                        else wanderCoroutine ??= StartCoroutine(Wander());
                    }
                    else wanderCoroutine ??= StartCoroutine(Wander());
                }

                // Keep chasing after the player has left the detection range
                else if (chaseTimer > 0) Engage(attributes);
                else wanderCoroutine ??= StartCoroutine(Wander());

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

    bool isAttacking;

    void Engage((float speed, float rotationSpeed, float acceleration, float stoppingDistance) attributes)
    {
        var path = new NavMeshPath();

        if (agent.CalculatePath(target.position, path) && path.status == NavMeshPathStatus.PathComplete) { agent.SetPath(path); }
        else if (agent.pathStatus == NavMeshPathStatus.PathPartial)
        {
            Debug.LogWarning("Path is partial! \nPlayer is either behind a closed door or out of bounds.");
            agent.ResetPath();
            wanderCoroutine = StartCoroutine(Wander());
        }

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
        if (NearTarget) Attack();
        else
        {
            attackCoroutine = null; // Can't attack while chasing.
            Chase();
        }
    }
    
    Coroutine attackCoroutine;

    [SerializeField] ParticleSystem slashVFX;
    
    bool enteredComabt => attackCoroutine != null;
    
    void Attack()
    {
        if (isAttacking) return;

        isAttacking = true;

        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        attackCoroutine ??= StartCoroutine(Cooldown());

        if (enteredComabt)
        {
            MenuView currentDialogue = MenuViewManager.GetCurrentView();
            if (currentDialogue) MenuViewManager.HideView(currentDialogue);
        }
    }

    void Update()
    {
        // animation was making the enemy wonky and sometimes would moonwalk.
        // super ugly fix but it works.
        
        if (enemy.isBoss) return;
        
        // keep all transform values at zero
        Transform child = transform.GetChild(0);
        child.localPosition = Vector3.zero;
        child.localRotation = Quaternion.identity;
        child.localScale = Vector3.one;
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(attackCooldown);

        // double check if the player is still within attack range
        if (!NearTarget)
        {
            isAttacking = false;
            yield break;
        }

        anim.SetTrigger("slash");

        yield return new WaitForSeconds(0.4f);
        
        var slash = Instantiate(slashVFX, transform.position + new Vector3(0, 0.75f, 0), Quaternion.identity);
        slash.transform.forward = target.position - transform.position;
        slash.transform.rotation = Quaternion.Euler(-90, slash.transform.rotation.eulerAngles.y, -90);
        
        slash.Play();

        yield return new WaitForSeconds(0.25f);
        
        target.TryGetComponent(out Player player);
        player.TakeDamage(damage);

        isAttacking = false;
        attackCoroutine = null;
    }

    public bool NearTarget
    {
        get
        {
            if (!agent || !target || !gameObject.activeSelf || !enabled) return false;
            return agent.remainingDistance <= agent.stoppingDistance && Vector3.Distance(target.position, transform.position) <= attackRange;
        }
    }

    public bool HasLineOfSight
    {
        get
        {
            if (Physics.Raycast(transform.position + new Vector3(0, 0.75f, 0), target.position - transform.position + new Vector3(0, 0.75f, 0), out RaycastHit hit, Mathf.Infinity))
            {
                // if the ray hits a player and does not hit a wall or a closed door, return true
                if (hit.transform.CompareTag("Player") && !hit.transform.name.ToLower().Contains("wall") && !(hit.transform.TryGetComponent(out Door door) && !door.IsOpen)) return true;
            }

            return false;
        }
    }

    void Chase()
    {
        chaseTimer += Time.deltaTime;
        //Debug.Log($"Chasing player! | {chaseTimer.RoundToInt()}s elapsed.");
        
        if (chaseTimer >= chaseDuration && !NearTarget)
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
            float randDist = Random.Range(attackRange, detectionRange);
            Vector3 randomPosition = RandomNavSphere(transform.position, randDist, -1);
            agent.stoppingDistance = 0;
            agent.SetDestination(randomPosition);
            yield return new WaitForSeconds(delayBetweenMoves);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = NearTarget ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
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
