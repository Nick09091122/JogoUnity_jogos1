using UnityEngine;

public class InimigoSimples : MonoBehaviour
{

    [Header("Waypoints")]
    public List<Transform> waypoints; // List of waypoints for the enemy to follow
    [Header("Movement Settings")]
    public float moveSpeed = 5f; // Speed of the enemy
    public float waypointReachedDistance = 0.2f; // Distance to consider waypoint reached
    public bool loop = true; // Should the enemy loop through waypoints
    private int currentWaypointIndex = 0; // Current waypoint index
    private Vector2 movementDirection;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = getcomponent<Rigidbody2D>();

        if (waypoints == null || waypoints.Count == 0)
        {
            debug.logerror("Waypoints not set for InimigoSimples");
            enabled = false; // Disable the script if no waypoints are set
            return;
        }
        Settargetwaypoint(currentwaypointindex);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Movetowardswaypoint();
        CheckIfReachedWaypoint();
    }

    void Movetowardswaypoint()
    {
        if(waypoints.count == 0) return;

        vector2 targetPosition = waypoints[currentWaypointIndex].position;
        movementDirection = (targetPosition -(vector2)transform.position).normalized;
        rb.velocity = movementDirection * moveSpeed;
    }
    void Settargettwaypoint()
    {
      if (waypoints.count == 0) return;

      currentwaypointindex = index;
      Vector2 targetPosition = waypoints[currentwaypointindex].position;
      movementDirection = (targetPosition - (Vector2)transform.position).normalized;
    }
    void CheckIfReachedWaypoint()
    {
        if (waypoints.Count == 0) return;

        float distanceToWaypoint = Vector2.Distance(transform.position, waypoints[currentWaypointIndex].position);

        if (distanceToWaypoint <= waypointreacheddistance)
        {
           gototextwaypoint();
        }
    }
    void Gototextwaypoint()
    {
        currentwaypointindex++;

        if (currentwaypointindex >= waypoints.count)
        {
            if (loop)
            {
                currentwaypointindex = 0;
            } else
            {
                enabled = false; // Stop moving if not looping
                rb.linearVelocity = Vector2.zero;
                return;
            }
        }
        Settargetwaypoint(currentwaypointindex);
    }
}
