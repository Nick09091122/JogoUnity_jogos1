using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.3f;
    public Vector3 offset = new Vector3(0, 0, -10);
    
    [Header("Ground-based Camera Adjustment")]
    public bool enableGroundAdjustment = true;
    public float groundedOffsetY = 0f;        // Offset quando no chão
    public float airborneOffsetY = -2f;       // Offset quando no ar (rebaixado)
    public float adjustmentSmoothTime = 0.5f; // Suavidade do ajuste
    
    private Vector3 velocity = Vector3.zero;
    private float currentOffsetY;
    private float offsetYVelocity = 0f;
    private MainCharacterController characterController;

    void Start()
    {
        // Tenta encontrar o MainCharacterController no alvo
        if (target != null)
        {
            characterController = target.GetComponent<MainCharacterController>();
            
            if (characterController == null)
            {
                // Tenta encontrar em filhos
                characterController = target.GetComponentInChildren<MainCharacterController>();
                
                if (characterController == null)
                {
                    Debug.LogWarning("MainCharacterController não encontrado no target. Ajuste de câmera por ground não funcionará.");
                }
            }
        }
        
        // Inicializa com offset normal
        currentOffsetY = offset.y;
    }

    void FixedUpdate()
    {
        if (target == null) return;
        
        // Atualiza o offset Y baseado no estado ground
        if (enableGroundAdjustment && characterController != null)
        {
            UpdateCameraOffset();
        }
        
        Vector3 targetPosition = CalculateTargetPosition();
        targetPosition.z = transform.position.z; // Mantém Z constante
        
        transform.position = Vector3.SmoothDamp(
            transform.position, 
            targetPosition, 
            ref velocity, 
            smoothTime
        );
    }
    
    void UpdateCameraOffset()
    {
        // Determina o offset Y desejado baseado no isGrounded
        float desiredOffsetY = characterController.isGrounded ? groundedOffsetY : airborneOffsetY;
        
        // Suaviza a transição entre offsets
        currentOffsetY = Mathf.SmoothDamp(
            currentOffsetY, 
            desiredOffsetY, 
            ref offsetYVelocity, 
            adjustmentSmoothTime
        );
        
        // Atualiza o offset com o Y ajustado
        offset = new Vector3(offset.x, currentOffsetY, offset.z);
    }
    
    Vector3 CalculateTargetPosition()
    {
        // Posição base do target
        Vector3 targetPosition = target.position;
        
        // Se não estiver usando ajuste por ground, usa offset fixo
        if (!enableGroundAdjustment || characterController == null)
        {
            targetPosition += offset;
        }
        else
        {
            // Usa offset com Y ajustado dinamicamente
            targetPosition += new Vector3(offset.x, currentOffsetY, offset.z);
        }
        
        return targetPosition;
    }
    
    // Método para forçar um offset específico (útil para cutscenes)
    public void SetCameraOffset(float newOffsetY, float transitionTime = 0.5f)
    {
        enableGroundAdjustment = false;
        StartCoroutine(TransitionToOffset(newOffsetY, transitionTime));
    }
    
    // Método para reativar o ajuste automático por ground
    public void EnableGroundAdjustment()
    {
        enableGroundAdjustment = true;
    }
    
    // Coroutine para transição suave (opcional)
    System.Collections.IEnumerator TransitionToOffset(float targetOffsetY, float duration)
    {
        float startOffsetY = currentOffsetY;
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            currentOffsetY = Mathf.Lerp(startOffsetY, targetOffsetY, elapsedTime / duration);
            offset = new Vector3(offset.x, currentOffsetY, offset.z);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        currentOffsetY = targetOffsetY;
        offset = new Vector3(offset.x, currentOffsetY, offset.z);
    }
    
    // Debug no Editor
    void OnDrawGizmosSelected()
    {
        if (target != null && enableGroundAdjustment)
        {
            // Desenha a área de visão quando no chão
            Gizmos.color = Color.green;
            Vector3 groundedView = target.position + new Vector3(0, groundedOffsetY, -5);
            Gizmos.DrawWireCube(groundedView, new Vector3(10, 6, 0));
            
            // Desenha a área de visão quando no ar
            Gizmos.color = Color.red;
            Vector3 airborneView = target.position + new Vector3(0, airborneOffsetY, -5);
            Gizmos.DrawWireCube(airborneView, new Vector3(10, 6, 0));
            
            // Linha conectando as duas áreas
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(groundedView, airborneView);
        }
    }
    
    // Métodos públicos para obter informações atuais
    public bool IsCameraAdjustedForAirborne()
    {
        return enableGroundAdjustment && characterController != null && !characterController.isGrounded;
    }
    
    public float GetCurrentOffsetY()
    {
        return currentOffsetY;
    }
}