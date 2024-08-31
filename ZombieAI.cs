using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 15f;
    public float attackRange = 2f;
    public float stopDistance = 1.5f;
    public float soundDetectionRange = 20f;
    public float runSpeed = 5f;
    public int maxHealth = 30; // Maximum health of the zombie

    private int currentHealth;
    private NavMeshAgent agent;
    private Animator animator;
    private bool isAttacking = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth; // Initialize health
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            agent.SetDestination(player.position);

            if (distanceToPlayer <= attackRange)
            {
                agent.isStopped = true;
                RotateTowards(player.position); // Hamesha player ki taraf dekhain

                if (!isAttacking)
                {
                    isAttacking = true;
                    AttackPlayer(); // Player health ko reduce karo
                }
            }
            else if (distanceToPlayer <= stopDistance)
            {
                agent.isStopped = true;
                RotateTowards(player.position); // Hamesha player ki taraf dekhain
                isAttacking = false; // Reset attack state when out of attack range
            }
            else
            {
                agent.isStopped = false;
                if (animator != null)
                {
                    animator.SetBool("isWalking", true);
                }
                isAttacking = false; // Reset attack state when out of attack range
            }
        }
        else
        {
            agent.isStopped = true;
            if (animator != null)
            {
                animator.SetBool("isWalking", false);
                isAttacking = false;
            }
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("Zombie took damage! Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void AttackPlayer()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(10); // Reduce player health by 10 (or any other value)
        }

        // Zombie should attack continuously until out of range
        Invoke(nameof(ResetAttack), 1.5f); // Delay between attacks, adjust as needed
    }

    void ResetAttack()
    {
        isAttacking = false; // Allow the zombie to attack again after cooldown
    }

    void RotateTowards(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void Die()
    {
        agent.isStopped = true;
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
        Destroy(gameObject, 2f);
    }

    public void OnSoundHeard(Vector3 soundPosition)
    {
        float distanceToSound = Vector3.Distance(transform.position, soundPosition);

        if (distanceToSound <= soundDetectionRange)
        {
            Debug.Log("Zombie heard the sound!");
            agent.speed = runSpeed;
            agent.SetDestination(soundPosition);
            agent.isStopped = false;

            if (animator != null)
            {
                animator.SetBool("isWalking", true);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, soundDetectionRange);
    }
}
