using UnityEngine;
using UnityEngine.AI;

public class Remey : MonoBehaviour
{
    public Transform player;           // Player ka Transform
    public float detectionRange = 20f; // Kitni doori tak player ko detect karna hai
    public float attackRange = 5f;     // Jab player is range mein ho to attack kare
    public float hidingRange = 10f;    // Agar player is range mein ho to AI hide karega
    public float moveSpeed = 5f;       // AI ki speed
    public float fireRate = 1f;        // Kitni der se fire hoga
    public GameObject bulletPrefab;    // Bullet prefab jo fire kiya jayega
    public Transform firePoint;        // Fire point jahan se bullet fire hoga
    public Transform coverSpot;        // Cover spot jahan AI chupayega
    public LayerMask coverLayerMask;   // Layer mask for cover objects

    private NavMeshAgent navMeshAgent;
    private Animator animator;         // Animator component for handling animations
    private float nextTimeToFire = 0f;
    private bool isTakingCover = false; // AI is taking cover
    private bool isHit = false;        // AI has been hit

    void Start()
    {
        // NavMeshAgent component le kar AI ko movement ke liye ready karen
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = moveSpeed; // Speed set karen

        // Animator component initialize karen
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isHit)
        {
            TakeCover(); // Agar hit hua hai to cover lein
        }
        else if (IsPlayerInRange())
        {
            FacePlayer(); // AI ka face player ki taraf karein

            if (IsPlayerInHidingRange() || isTakingCover)
            {
                // Agar player hiding range mein hai ya AI already cover le raha hai
                if (!isTakingCover)
                {
                    TakeCover();
                }

                if (Time.time >= nextTimeToFire && isTakingCover)
                {
                    nextTimeToFire = Time.time + 1f / fireRate;
                    Fire();
                }
            }
            else if (IsPlayerInAttackRange())
            {
                // Jab player attack range mein ho to fire kare aur movement ko stop kar de
                navMeshAgent.isStopped = true;
                animator.SetBool("isHiding", false); // Hiding animation ko disable karen
                animator.SetBool("isWalking", true); // Walking fire animation set karen

                if (Time.time >= nextTimeToFire)
                {
                    nextTimeToFire = Time.time + 1f / fireRate;
                    Fire();
                }
            }
            else
            {
                // Jab player hiding ya attack range mein nahi hai to AI player ko chase karega
                navMeshAgent.isStopped = false;
                animator.SetBool("isHiding", false); // Hiding animation ko disable karen
                animator.SetBool("isWalking", true); // Walking animation set karen
                ChasePlayer();
            }
        }
        else
        {
            // Agar player detect na ho to AI stop ho jaye
            navMeshAgent.ResetPath();
            navMeshAgent.isStopped = false;
            animator.SetBool("isWalking", false); // Walking animation ko disable karen
            animator.SetBool("isHiding", false); // Hiding animation ko disable karen
        }
    }

    private bool IsPlayerInRange()
    {
        // Player aur AI ke darmiyan distance check karein
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        return distanceToPlayer <= detectionRange;
    }

    private bool IsPlayerInAttackRange()
    {
        // Player aur AI ke darmiyan attack range check karein
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        return distanceToPlayer <= attackRange;
    }

    private bool IsPlayerInHidingRange()
    {
        // Player aur AI ke darmiyan hiding range check karein
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        return distanceToPlayer <= hidingRange && distanceToPlayer > attackRange;
    }

    private void ChasePlayer()
    {
        // Player ki taraf move karein
        navMeshAgent.SetDestination(player.position);
    }

    private void FacePlayer()
    {
        // AI ka chehra player ki taraf karna
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private void Fire()
    {
        Debug.Log("Firing at player!");

        // Player ke chest par target karne ke liye direction find karein
        Vector3 direction = (player.position + Vector3.up * 1.5f - firePoint.position).normalized;

        RaycastHit hit;
        if (Physics.Raycast(firePoint.position, direction, out hit))
        {
            // Agar raycast hit kare, to bullet ko hit point par fire karein
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(direction));
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

            if (bulletRb != null)
            {
                bulletRb.velocity = direction * 100f; // Bullet ki speed set karen
            }
        }
    }

    private void TakeCover()
    {
        if (!isTakingCover)
        {
            // Cover position find karna
            Vector3 coverDirection = (coverSpot.position - transform.position).normalized;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(coverSpot.position, out hit, 1.0f, NavMesh.AllAreas))
            {
                navMeshAgent.SetDestination(hit.position); // Move to cover spot
                isTakingCover = true;
                navMeshAgent.isStopped = false;
                animator.SetBool("isHiding", true); // Hiding animation set karen
                animator.SetBool("isWalking", false); // Walking animation ko disable karen
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            isHit = true; // Agar AI ko bullet lage to cover le
            Destroy(other.gameObject); // Bullet ko destroy karen
        }
    }

    void OnDrawGizmosSelected()
    {
        // Gizmos se detection, attack, aur hiding ranges draw karein
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, hidingRange);

        // Raycast ko visualize karne ke liye
        if (player != null && firePoint != null)
        {
            Vector3 direction = (player.position + Vector3.up * 1.5f - firePoint.position).normalized;
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(firePoint.position, direction * detectionRange);
        }
    }
}
