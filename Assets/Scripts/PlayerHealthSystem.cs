using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthSystem : MonoBehaviour
{

    public int maxHealth;
    public int currentHealth;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int amountOfDamage)
    {
        currentHealth -= amountOfDamage;

        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
