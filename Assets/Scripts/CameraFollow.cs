using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target to follow")]
    public Transform target;
    public float smoothSpeed = 0.5f;
    public Vector3 offset = new Vector3(0, 2, -10);

    void LateUpdate()
    {
        if (target == null) {
        debug.LogWarning("CameraFollow: No target assigned to follow."); 
        return; }

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
        transform.LookAt(target);
    }
}