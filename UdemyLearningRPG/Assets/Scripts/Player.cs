using UnityEngine;

public class Player : MonoBehaviour
{
    private Animator _animator;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float moveSpeed, jumpForce;

    private float xInput;
    private bool facingRight = true;

    [Header("Collison Info")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    private bool isGrounded;


    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
    }


    void Update()
    {

        Movement();

        CheckInput();

        CollisionCheck();

        FlipController();

        AnimationController();
    }

    private void CollisionCheck()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
    }

    private void CheckInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    private void Movement()
    {
        rb.velocity = new Vector2(moveSpeed * xInput, rb.velocity.y);
    }

    private void Jump()
    {
        if(isGrounded)
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    private void Flip()
    {
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
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - groundCheckDistance));
    }

}
