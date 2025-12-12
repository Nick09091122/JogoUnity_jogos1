using UnityEngine;

public class MainCharacterController : MonoBehaviour
{
    public float moveSpeed = 25f;     // Horizontal movement speed
    public float jumpForce = 15f;    // Jump strength
    public Transform groundCheck;    // Empty GameObject at feet
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    
    private Rigidbody2D rb, rc;
    private bool isGrounded;

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
        originalGravity = rc.gravityScale;

    }

    void Update()
    {

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        float moveInput = Input.GetAxisRaw("Horizontal");
        float Jump = Input.GetAxisRaw("Spacebar");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

         anim.SetBool("IsRunning", Mathf.Abs(rb.linearVelocity.x) > 0.1f && isGrounded);
        if (moveInput > 0.01f)
            visual.localScale = new Vector3(4, 4, 4);
        else if (moveInput < -0.01f)
            visual.localScale = new Vector3(-4, 4, 4);

            anim.SetBool("IsFalling", Mathf.Abs(rb.linearVelocity.y) < 0f && isGrounded == false);
                if (rb.linearVelocity.y < 0.1f)
                    anim.SetBool("IsFalling", false);
                else if (rb.linearVelocity.y > 0.1f)
                    anim.SetBool("IsFalling", true);

                     // Ativa parada no ar quando pressionar a tecla e estiver no ar
        if (Input.GetKeyDown(stopKey) && !IsGrounded() && !isStopping)
        {
            StartAirStop();
        }
        
        // Controla o temporizador
        if (isStopping)
        {
            stopTimer -= Time.deltaTime;
            
            // Congela o movimento
            rc.linearVelocity = Vector2.zero;
            
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
        rc.gravityScale = 0;
        Debug.Log("Parou no ar por " + airStopTime + " segundos!");
    }
    
    void EndAirStop()
    {
        isStopping = false;
        rc.gravityScale = originalGravity;
    }
    
    bool IsGrounded()
    {
        // Implemente sua verificação de chão aqui
        return Physics2D.Raycast(transform.position, Vector2.down, 0.6f);
    }
    }