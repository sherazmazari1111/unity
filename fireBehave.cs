using UnityEngine;
using System.Collections;

public class fireBehave : MonoBehaviour
{
    public Animator animator;
    public float speed = 0.0f;
    private bool isAiming;
    private bool isFiring;

    void Update()
    {
        // Speed calculation from input
        Vector3 inputVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        speed = inputVector.magnitude;
        animator.SetFloat("Speed", speed);
        Debug.Log("Speed: " + speed); // Debugging speed value

        // Aim check
        isAiming = Input.GetButton("Fire2"); // Right Mouse Button for aiming
        animator.SetBool("Walk Aim", isAiming);
        Debug.Log("Is Aiming: " + isAiming); // Debugging aim status

        // Walk Aim logic
        bool isWalkingAndAiming = speed > 0.1f && isAiming;
        animator.SetBool("Walk Aim", isWalkingAndAiming);
        Debug.Log("Is Walking and Aiming: " + isWalkingAndAiming); // Debugging walk aim status

        // Fire logic
        if (Input.GetButton("Fire1")) // Left Mouse Button for continuous firing
        {
            animator.SetBool("fire", true); // Set fire to true when the fire button is pressed
            Debug.Log("Fire button pressed: " + true); // Debugging fire pressed
            isFiring = true; // Mark that firing has started

            // Start the firing coroutine
            StartCoroutine(FireCoroutine());
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            animator.SetBool("fire", false);
            Debug.Log("Fire button released: " + false); // Debugging fire released
            isFiring = false; // Mark that firing has stopped
        }
    }

    // Coroutine for continuous firing
    private IEnumerator FireCoroutine()
    {
        while (isFiring)
        {
            // Execute your shooting logic here (e.g., shooting a bullet)
            Debug.Log("Shooting...");

            // Wait for a short interval before the next shot
            yield return new WaitForSeconds(0.1f); // Adjust this interval as needed
        }
    }
}
