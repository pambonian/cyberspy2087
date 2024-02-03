using UnityEngine;

public class PortalDoor : MonoBehaviour
{
    // Reference to the door GameObject's Animator
    public Animator doorAnimator;

    // Optional: Tag to identify the player
    public string playerTag = "Player";

    private void Start()
    {
        // Initialize your doorAnimator here if you haven't assigned it in the inspector
        doorAnimator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering collider is the player
        if (other.CompareTag(playerTag))
        {
            // Play the door open animation
            doorAnimator.SetBool("Opening", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the exiting collider is the player
        if (other.CompareTag(playerTag))
        {
            // Play the door close animation
            doorAnimator.SetBool("Opening", false);
        }
    }
}
