using UnityEngine;
using System.Collections.Generic;

public class EnemyWaypointMovement : MonoBehaviour
{
    [Header("Waypoints")]
    public List<Transform> waypoints; // List of waypoints for the enemy to follow
    
    [Header("Movement Settings")]
    public float moveSpeed = 5f; // Speed of the enemy
    public float waypointReachedDistance = 0.1f; // Distance to consider waypoint reached
    public bool loop = true; // Should the enemy loop through waypoints

    [Header("Combat Settings")]
    public float damage = 1f;
    public float attackRate = 1f;
    public float knockbackForce = 7f;
    
    [Header("Visual Settings")]
    public bool flipSprite = true; // Deve inverter o sprite?
    public Transform spriteTransform; // Referência ao transform do sprite (opcional)
    
    private Rigidbody2D rb;
    private Vector2 movementDirection;
    private int currentWaypointIndex = 0;
    private float lastAttackTime = 0f;
    private bool isMoving = true;
    private bool facingRight = true; // Controla direção atual do sprite

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Se spriteTransform não foi atribuído, usa o transform principal
        if (spriteTransform == null)
        {
            spriteTransform = transform;
            Debug.Log("SpriteTransform não atribuído, usando transform principal.");
        }

        if (waypoints == null || waypoints.Count == 0)
        {
            Debug.LogError("Waypoints not set for EnemyWaypointMovement");
            enabled = false;
            return;
        }
        SetTargetWaypoint(currentWaypointIndex);
        
        // Define direção inicial baseada no primeiro waypoint
        UpdateFacingDirection();
    }

    void Update()
    {
        CheckIfReachedWaypoint();
        
        // Atualiza a direção do sprite durante o movimento
        if (isMoving && flipSprite)
        {
            UpdateFacingDirection();
        }
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            MoveTowardsWaypoint();
        }
    }

    void MoveTowardsWaypoint()
    {
        if (waypoints.Count == 0 || currentWaypointIndex >= waypoints.Count) 
        {
            isMoving = false;
            return;
        }

        Vector2 targetPosition = waypoints[currentWaypointIndex].position;
        movementDirection = (targetPosition - (Vector2)transform.position).normalized;
        
        // Move apenas no eixo X, mantém velocidade Y atual (para física)
        rb.linearVelocity = new Vector2(movementDirection.x * moveSpeed, rb.linearVelocity.y);
    }
    
    // Função para inverter o sprite baseado na direção do movimento
    void UpdateFacingDirection()
    {
        if (!isMoving || waypoints.Count == 0 || currentWaypointIndex >= waypoints.Count)
            return;

        // Determina a direção do movimento
        Vector2 targetPosition = waypoints[currentWaypointIndex].position;
        float horizontalDirection = targetPosition.x - transform.position.x;
        
        // Se está se movendo para a direita e não está virado para a direita
        if (horizontalDirection > 0.1f && !facingRight)
        {
            FlipSprite(true);
        }
        // Se está se movendo para a esquerda e está virado para a direita
        else if (horizontalDirection < -0.1f && facingRight)
        {
            FlipSprite(false);
        }
    }
    
    // Função para inverter o sprite
    void FlipSprite(bool faceRight)
    {
        if (!flipSprite) return;
        
        facingRight = faceRight;
        
        // Método 1: Inverte a escala no eixo X (mais comum)
        Vector3 currentScale = spriteTransform.localScale;
        currentScale.x = facingRight ? Mathf.Abs(currentScale.x) : -Mathf.Abs(currentScale.x);
        spriteTransform.localScale = currentScale;
        
        // Método alternativo: Rotação no eixo Y (descomente se preferir)
        // spriteTransform.rotation = Quaternion.Euler(0, facingRight ? 0 : 180, 0);
    }
    
    public void Jump(float JumpForce)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, JumpForce);
    }

    void SetTargetWaypoint(int index)
    {
        if (waypoints.Count == 0 || index >= waypoints.Count || index < 0)
        {
            Debug.LogWarning("Waypoint index out of range");
            isMoving = false;
            return;
        }

        currentWaypointIndex = index;
        isMoving = true;
        
        // Atualiza direção ao definir novo waypoint
        if (flipSprite)
        {
            UpdateFacingDirection();
        }
    }

    void CheckIfReachedWaypoint()
    {
        if (!isMoving || waypoints.Count == 0 || currentWaypointIndex >= waypoints.Count) 
            return;

        float distanceToWaypoint = Vector2.Distance(transform.position, waypoints[currentWaypointIndex].position);

        if (distanceToWaypoint <= waypointReachedDistance)
        {
            GotoNextWaypoint();
        }
    }

    void GotoNextWaypoint()
    {
        currentWaypointIndex++;

        if (currentWaypointIndex >= waypoints.Count)
        {
            if (loop)
            {
                currentWaypointIndex = 0;
                SetTargetWaypoint(currentWaypointIndex);
            }
            else
            {
                isMoving = false;
                rb.linearVelocity = Vector2.zero;
                Debug.Log("Reached final waypoint, stopping movement.");
            }
        }
        else
        {
            SetTargetWaypoint(currentWaypointIndex);
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            TryAttackPlayer(collision.gameObject);
        }
    }
    
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            TryAttackPlayer(collision.gameObject);
        }
    }
    
    void TryAttackPlayer(GameObject player)
    {
        if (Time.time >= lastAttackTime + attackRate)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                Vector2 knockbackDirection = (player.transform.position - transform.position).normalized;
                playerHealth.TakeDamage(damage, knockbackDirection, knockbackForce);
                lastAttackTime = Time.time;
            }
        }
    }

    // Método para visualizar waypoints e direção
    void OnDrawGizmosSelected()
    {
        if (waypoints != null && waypoints.Count > 0)
        {
            Gizmos.color = Color.red;
            
            for (int i = 0; i < waypoints.Count; i++)
            {
                if (waypoints[i] != null)
                {
                    Gizmos.DrawSphere(waypoints[i].position, 0.2f);
                    
                    if (i < waypoints.Count - 1 && waypoints[i + 1] != null)
                    {
                        Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                    }
                    else if (loop && waypoints[0] != null)
                    {
                        Gizmos.DrawLine(waypoints[i].position, waypoints[0].position);
                    }
                    
                    #if UNITY_EDITOR
                    UnityEditor.Handles.Label(waypoints[i].position + Vector3.up * 0.3f, "WP " + i);
                    #endif
                }
            }
            
            // Mostrar direção atual
            if (Application.isPlaying && isMoving && currentWaypointIndex < waypoints.Count)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, waypoints[currentWaypointIndex].position);
                
                // Mostrar direção do sprite
                Gizmos.color = facingRight ? Color.green : Color.blue;
                Vector3 directionIndicator = transform.position + (facingRight ? Vector3.right : Vector3.left) * 0.5f;
                Gizmos.DrawLine(transform.position, directionIndicator);
            }
        }
    }
    
    // Método público para forçar inverter direção (útil para habilidades especiais)
    public void ForceFlip(bool faceRight)
    {
        FlipSprite(faceRight);
    }
    
    // Método para obter direção atual
    public bool IsFacingRight()
    {
        return facingRight;
    }
    
    // Método para debugging
    public void DebugWaypointInfo()
    {
        Debug.Log($"Current WP: {currentWaypointIndex}/{waypoints.Count - 1}, " +
                 $"IsMoving: {isMoving}, FacingRight: {facingRight}, " +
                 $"Velocity X: {rb.linearVelocity.x:F2}");
    }
}