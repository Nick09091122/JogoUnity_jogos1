using UnityEngine;

public class MainCharacterController : MonoBehaviour
{
    public float moveSpeed = 25f;     // Horizontal movement speed
    public float jumpForce = 15f;    // Jump strength
    public Transform groundCheck;    // Empty GameObject at feet
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    
    private Rigidbody2D rb;
    public bool isGrounded;

    public Transform visual;
    private Animator anim;  

    public float airStopTime = 3f;
    public KeyCode stopKey;
    
    private bool isStopping = false;
    private float stopTimer = 0f;
    private float originalGravity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = visual.GetComponent<Animator>();
        originalGravity = rb.gravityScale;
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        float moveInput = Input.GetAxisRaw("Horizontal");
        
        // CORREÇÃO: Use velocity em vez de linearVelocity
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // CORREÇÃO: Use velocity em vez de linearVelocity
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // CORREÇÕES NAS ANIMAÇÕES: Use velocity em vez de linearVelocity
        anim.SetBool("IsRunning", Mathf.Abs(rb.linearVelocity.x) > 0.1f && isGrounded);
        
        if (moveInput > 0.01f)
            visual.localScale = new Vector3(4, 4, 4);
        else if (moveInput < -0.01f)
            visual.localScale = new Vector3(-4, 4, 4);

        // CORREÇÃO: Lógica de animação de queda corrigida
        if (!isGrounded)
        {
            anim.SetBool("IsFalling", rb.linearVelocity.y < -0.1f);
        }
        else
        {
            anim.SetBool("IsFalling", false);
        }

        // Ativa parada no ar quando pressionar a tecla e estiver no ar
        // CORREÇÃO: Use isGrounded (já calculado) em vez de chamar IsGrounded()
        if (Input.GetKeyDown(stopKey) && !isGrounded && !isStopping)
        {
            StartAirStop();
        }
        
        // Controla o temporizador
        if (isStopping)
        {
            stopTimer -= Time.deltaTime;
            
            // CORREÇÃO: Use velocity em vez de linearVelocity
            rb.linearVelocity = Vector2.zero;
            
            if (stopTimer <= 0)
            {
                EndAirStop();
            }
        }
    }
    
    void StartAirStop()
    {
        isStopping = true;
        stopTimer = airStopTime;
        rb.gravityScale = 0;
        
        Debug.Log("Parou no ar por " + airStopTime + " segundos!");
    }
    
    void EndAirStop()
    {
        isStopping = false;
        rb.gravityScale = originalGravity;
    }
    
    // Este método pode ser removido pois já temos isGrounded
    // bool IsGrounded()
    // {
    //     return Physics2D.Raycast(transform.position, Vector2.down, 0.6f);
    // }
}