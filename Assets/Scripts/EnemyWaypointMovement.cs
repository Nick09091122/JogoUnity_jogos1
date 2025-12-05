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
    public float kockbackForce = 7f;
    
    private Rigidbody2D rb;
    private Vector2 movementDirection;
    private int currentWaypointIndex = 0;
    private float lastAttackTime = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (waypoints == null || waypoints.Count == 0)
        {
            Debug.LogError("Waypoints not set for EnemyWaypointMovement");
            enabled = false; // Disable the script if no waypoints are set
            return;
        }
        SetTargetWaypoint(currentWaypointIndex);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveTowardsWaypoint();
        CheckIfReachedWaypoint();
    }

    void MoveTowardsWaypoint()
    {
        if (waypoints.Count == 0) return;

        Vector2 targetPosition = waypoints[currentWaypointIndex].position;
        movementDirection = (targetPosition - (Vector2)transform.position).normalized;
        rb.linearVelocity = movementDirection * moveSpeed;
    }

    void SetTargetWaypoint(int index)
    {
        if (waypoints.Count == 0) return;

        currentWaypointIndex = index;
        Vector2 targetPosition = waypoints[currentWaypointIndex].position;
        movementDirection = (targetPosition - (Vector2)transform.position).normalized;
    }

    void CheckIfReachedWaypoint()
    {
        if (waypoints.Count == 0) return;

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
            }
            else
            {
                enabled = true; // Stop moving if not looping
                rb.linearVelocity = Vector2.zero;
                return;
            }
        }
        SetTargetWaypoint(currentWaypointIndex);
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
            playerHealth = player.GetComponent<currentHealth>();
            if (playerHealth != null)
            {
                Vector2 knockbackDirection = (player.transform.position - transform.position).normalized;

                playerHealth.TakeDamage(damage, knockbackDirection, kockbackForce);
                lastAttackTime = Time.time;
                }
            }
        }
    }