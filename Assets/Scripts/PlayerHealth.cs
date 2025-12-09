using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 10f;
    public float currentHealth;
    private Rigidbody2D rb;
    public HealthBar healthBar;
    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(float damage, Vector2 knockbackDirection, float knockbackForce = 10f)
    {
        currentHealth -= damage;
        Debug.Log("Player took " + damage + " damage. Current health: " + currentHealth);

        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

        if (currentHealth <= 0)
        {
            Die();
        }   
    }    
    void Die()
    {
        Debug.Log("Player has died.");
        gameObject.SetActive(false);
    }
}
