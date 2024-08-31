using UnityEngine;

public class raycast : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public GameObject muzzleFlashPrefab;
    public GameObject bulletPrefab;
    public Transform firePoint;

    private Camera fpsCam;
    private AudioSource gunShotSound;

    void Start()
    {
        fpsCam = Camera.main;
        gunShotSound = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Play gunfire sound
        if (gunShotSound != null)
        {
            gunShotSound.Play();
        }

        // Muzzle flash instantiate karein
        if (muzzleFlashPrefab != null)
        {
            GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, firePoint.position, firePoint.rotation);
            Destroy(muzzleFlash, 0.1f);
        }

        // Bullet instantiate karein
        if (bulletPrefab != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                bulletRb.velocity = fpsCam.transform.forward * range;
                Destroy(bullet, 2f);
            }
        }

        // Notify zombies about the sound
        ZombieAI[] zombies = FindObjectsOfType<ZombieAI>();
        foreach (ZombieAI zombie in zombies)
        {
            zombie.OnSoundHeard(firePoint.position); // Notify each zombie about the sound's position
        }

        // Raycast shooting direction ko camera ke nazar se align karein
        RaycastHit hit;
        Ray ray = fpsCam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, range))
        {
            Debug.Log("Hit: " + hit.transform.name);

            // Check if the hit object has a ZombieAI component (to apply damage)
            ZombieAI zombie = hit.transform.GetComponent<ZombieAI>();
            if (zombie != null)
            {
                zombie.TakeDamage((int)damage); // Apply damage to the zombie
            }
            else
            {
                // If no ZombieAI component, check for a PlayerHealth component
                PlayerHealth playerHealth = hit.transform.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage((int)damage); // Apply damage to the player
                }
                else
                {
                    // If no PlayerHealth component, check for a generic Target component
                    Target target = hit.transform.GetComponent<Target>();
                    if (target != null)
                    {
                        target.TakeDamage(damage);
                    }
                    else
                    {
                        Debug.LogWarning("No Target, PlayerHealth, or ZombieAI component found on " + hit.transform.name);
                    }
                }
            }
        }
    }
}
