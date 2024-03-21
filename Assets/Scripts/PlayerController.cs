using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // movement variables
    [SerializeField] Rigidbody2D rbody;
    private float horizInput;
    private float moveSpeed = 450.0f;  // 4.5 * 100 since we're using Rigidbody physics to move (for Newtons)

    // jump variables
    private float jumpHeight = 3.0f;
    private float jumpTime = 0.75f;
    private float initialJumpVelocity;

    private bool isGrounded = false;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private LayerMask groundLayerMask;
    private float groundCheckRadius = 0.3f;

    private int jumpMax = 2;
    private int jumpsAvailable = 0;

    [SerializeField] Animator anim;

    private bool facingRight = true;

    // Start is called before the first frame update
    void Start()
    {
        // given a desired jumpHeight and jumpTime, calculate gravity (same formulas as 3D)
        float timeToApex = jumpTime / 2.0f;
        float gravity = (-2 * jumpHeight) / Mathf.Pow(timeToApex, 2);
        Debug.Log("gravity: " + gravity);

        // calculate jump velocity (upward motion)
        initialJumpVelocity = Mathf.Sqrt(jumpHeight * -2 * gravity);

        rbody.gravityScale = gravity / Physics2D.gravity.y;
    }

    // Update is called once per frame
    void Update()
    {
        // read (and store) horizontal input
        horizInput = Input.GetAxis("Horizontal");
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayerMask) && rbody.velocity.y < 0.01f;
        anim.SetBool("isGrounded", isGrounded);

        bool isRunning = horizInput > 0.01 || horizInput < -0.01;
        anim.SetBool("isRunning", isRunning);

        if (isGrounded)
        {
            jumpsAvailable = jumpMax;
        }
        if (Input.GetButtonDown("Jump") && jumpsAvailable > 0)
        {
            Jump();
        }

        // if we're facing the opposite direction of movement, Flip the Player!
        if ((!facingRight && horizInput > 0.01f) || (facingRight && horizInput < -0.01f))
        {
            Flip();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(groundCheckPoint.position, groundCheckRadius);
    }

    private void Jump()
    {
        rbody.velocity = new Vector2(rbody.velocity.x, initialJumpVelocity);
        jumpsAvailable -= 1;
        anim.SetTrigger("jump");
    }

    void Flip()
    {
        facingRight = !facingRight;
        // rotate 180 deg on the Y axis (yaw)
        transform.Rotate(Vector3.up * 180);
    }

    private void FixedUpdate()
    {
        // We're moving via Rigidbody physics (setting velocity directly) so we need to use FixedUpdate
        float xVel = horizInput * moveSpeed * Time.deltaTime;   // determine x velocity
        rbody.velocity = new Vector2(xVel, rbody.velocity.y);   // set new x velocity, maintain existing y velocity
    }
}
