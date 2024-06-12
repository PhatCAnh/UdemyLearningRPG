using UnityEngine;

public class Player : MonoBehaviour
{
    private Animator _animator;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float moveSpeed, jumpForce;

    private float xInput;
    private int facingDir = 1;
    private bool facingRight = true;

    [Header("Collison Info")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    private bool isGrounded;

    [Header("Dash Ability Info")]
    [SerializeField] private float dashDuration; 
    private float dashTime;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashCooldown;
    private float dashCooldownTimer;

    [Header("Attack Info")]
    [SerializeField] private float comboTime;
    private float comboTimeWindow;
    private bool isAttacking;
    private int comboCounter;

    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
    }


    void Update()
    {

        Movement();

        CheckInput();

        CollisionCheck();

        dashTime -= Time.deltaTime;       
        dashCooldownTimer -= Time.deltaTime;
        comboTimeWindow -= Time.deltaTime;


        FlipController();

        AnimationController();
    }

    public void AttackOver()
    {
        isAttacking = false;

        comboCounter++;

        if(comboCounter > 2)
        {
            comboCounter = 0;
        }
    }


    private void CollisionCheck()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
    }

    private void CheckInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            StartAttack();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            DashAbility();
        }

    }

    private void StartAttack()
    {
        if (!isGrounded) return;

        if (comboTimeWindow < 0)
        {
            comboCounter = 0;
        }

        isAttacking = true;

        comboTimeWindow = comboTime;
    }

    private void DashAbility()
    {
        if(dashCooldownTimer < 0 && !isAttacking)
        {
            dashCooldownTimer = dashCooldown;
            dashTime = dashDuration;
        } 
    }

    private void Movement()
    {
        if (isAttacking)
        {
            rb.velocity = Vector2.zero;
        }else if (dashTime > 0)
        {            
            rb.velocity = new Vector2(dashSpeed * facingDir, 0);
        }
        else
        {
            rb.velocity = new Vector2(moveSpeed * xInput, rb.velocity.y);
        }
    }

    private void Jump()
    {
        if (isGrounded)
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    private void Flip()
    {
        facingDir *= -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    private void FlipController()
    {
        if (rb.velocity.x > 0 && !facingRight)
            Flip();
        else if (rb.velocity.x < 0 && facingRight)
            Flip();
    }

    private void AnimationController()
    {
        bool isMoving = rb.velocity.x != 0;

        _animator.SetFloat("yVelocity", rb.velocity.y);

        _animator.SetBool("isMoving", isMoving);
        _animator.SetBool("isGrounded", isGrounded);
        _animator.SetBool("isDashing", dashTime > 0);
        _animator.SetBool("isAttacking", isAttacking);
        _animator.SetInteger("comboCounter", comboCounter);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - groundCheckDistance));
    }

}
