using UnityEngine;

public class WalkAimBehaviour : MonoBehaviour
{
    private Animator animator;
    public float speedThreshold = 0.1f; // Threshold to determine if player is moving
    public string speedParam = "Speed";
    public string walkAimParam = "Walk Aim";
    public string aimParam = "Aim";
    public string strafingParam = "Strafing"; // Assuming you have a strafing parameter

    void Start()
    {
        // Get the Animator component attached to the player
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Calculate the player's speed
        float currentSpeed = CalculateSpeed();

        // Set the speed parameter in the Animator
        animator.SetFloat(speedParam, currentSpeed);

        // Check if the player is aiming
        bool isAiming = animator.GetBool(aimParam);

        // Determine if the player should be walking while aiming
        bool shouldWalkAim = currentSpeed > speedThreshold && isAiming;

        // Set the Walk Aim parameter based on the conditions
        animator.SetBool(walkAimParam, shouldWalkAim);

        // Set the Strafing parameter based on aiming
        animator.SetBool(strafingParam, isAiming && !shouldWalkAim);

        // Adjust layer weights based on conditions
        if (shouldWalkAim)
        {
            animator.SetLayerWeight(2, 1.0f); // Ensure Walk Aim layer is active
        }
        else
        {
            animator.SetLayerWeight(2, 0.0f); // Deactivate Walk Aim layer when not needed
        }
    }

    private float CalculateSpeed()
    {
        // Calculate the player's movement speed based on input (example)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        return Mathf.Sqrt(horizontal * horizontal + vertical * vertical);
    }
}
