using UnityEngine;

public class HealthBarController : MonoBehaviour
{
    public float maxHealth = 10f;
    public float currentHealth;
    private Rigidbody2D rb;
    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(float damage, Vector2 knockbackDirection, float knockbackForce = 10f)
    {
        currentHealth -= damage;
        Debug.log("Player took " + damage + " damage. Current health: " + currentHealth);

        rb.addForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

        if (currentHealth <= 0)
        {
            Die();
        }   
    }    
    void Die()
    {
        Debug.log("Player has died.");
        gameObject.SetActive(false);
    }
}
