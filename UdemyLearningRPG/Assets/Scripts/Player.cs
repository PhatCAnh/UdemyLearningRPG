using UnityEngine;

public class Player : Entity
{

    private float xInput;   

    [Header("Move Info")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;

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

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

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

}
