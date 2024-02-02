using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthSystem : MonoBehaviour
{
    private int currentHealth;
    public int maxHealth;

    public delegate void EnemyDamagedHandler();
    public event EnemyDamagedHandler OnEnemyDamaged;


    EnemyUICanvasController healthBar;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;

        healthBar = GetComponentInChildren<EnemyUICanvasController>();
        healthBar.SetMaxHealth(maxHealth);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        healthBar.SetHealth(currentHealth);

        // Notify any listeners (like the EnemyAI) that this enemy has taken damage
        OnEnemyDamaged?.Invoke();

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }

        // If I want a better enemy death system:
        // if (currentHealth <= 0)
        // {
            // GetComponent<EnemyAI>()?.HandleDeath();
            // Destroy(gameObject);
        // }

    }
}
