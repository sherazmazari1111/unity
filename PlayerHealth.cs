using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public Slider healthBar;

    void Start()
    {
        currentHealth = maxHealth; // Initialize the player's health
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth; // Update the health bar to reflect the current health
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Clamp health to ensure it stays within bounds

        healthBar.value = currentHealth; // Update the health bar UI

        if (currentHealth <= 0)
        {
            Die(); // Call Die method if health is 0
        }
    }

    void Die()
    {
        // Handle player death (e.g., triggering death animation, restarting level, etc.)
        Debug.Log("Player has died!");
    }
}
