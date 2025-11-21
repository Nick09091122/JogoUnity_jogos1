using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    [Header("Parallax Settings")]
    public float parallaxFactor = 0.5f; // Factor by which the background moves relative to the camera
    public bool infiniteScroll = true; // Should the background scroll infinitely
    
    private Transform cameraTransform;
    private Vector3 lastCameraPosition;
    private float textureUnitSize;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && sr.sprite != null)
        {
            textureUnitSize = sr.bounds.size.x;
        }
        else
        {
            Debug.LogError("SpriteRenderer not found or no sprite assigned!");
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        transform.position += new Vector3(deltaMovement.x * parallaxFactor, 0, 0);

        lastCameraPosition = cameraTransform.position;

        if (infiniteScroll)
        {
            float deltaX = cameraTransform.position.x - transform.position.x;
            if (Mathf.Abs(deltaX) >= textureUnitSize)
            {
                float offsetPosition = deltaX > 0 ? textureUnitSize : -textureUnitSize;
                transform.position = new Vector3(transform.position.x + offsetPosition, transform.position.y, transform.position.z);
            }
        }
    }
}