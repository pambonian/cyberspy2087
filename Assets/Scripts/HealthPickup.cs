using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int amountOfHealing = 5;
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealthSystem>().HealPlayer(amountOfHealing);
            AudioManager.instance.PlayerSFX(1);
            Destroy(gameObject);
        }
    }

    
}
