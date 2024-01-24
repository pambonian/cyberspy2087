using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthSystem : MonoBehaviour
{

    public int maxHealth;
    private int currentHealth;

    UICanvasController healthBar;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;

        healthBar = FindObjectOfType<UICanvasController>();
        healthBar.SetMaxHealth(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int amountOfDamage)
    {
        currentHealth -= amountOfDamage;

        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);

            AudioManager.instance.StopBackgroundMusic();

            FindObjectOfType<GameManager>().PlayerRespawn();
        }
    }

    public void HealPlayer(int healAmount)
    {
        // If player collides with first aid kit
        currentHealth += healAmount;

        
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth; 
            
        }

        healthBar.SetHealth(currentHealth);
    }
}
