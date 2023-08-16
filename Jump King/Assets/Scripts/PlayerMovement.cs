using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider2d;
    [SerializeField] public float speed;
    [SerializeField] private LayerMask platformLayerMask; 
    private Animator anim;
    public bool canJump = true;
    public float jumpValue = 0.0f;
    public float horizontalInput;
    public bool canMove = true;
    public PhysicsMaterial2D bounceMat, normalMat;
    Vector3 lastVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider2d = transform.GetComponent<BoxCollider2D>();
    }
    void Start()
    {
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        lastVelocity = rb.velocity;
        
        //flip character
        if (horizontalInput > 0.01f)
        {
            transform.localScale = Vector3.one;
        } else if (horizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        if (Input.GetKey("space") && isGrounded()  && canJump)
        {

            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            canMove = false;
            Jump();
            
        }

        if(jumpValue == 0.0f && isGrounded())
        {
            rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);
        }

        if (Input.GetKeyUp("space"))
        {
            if (isGrounded())
            {
                rb.velocity = new Vector2(horizontalInput * speed, jumpValue);
                jumpValue = 0.0f;
            }
        }

        if (isGrounded())
        {
            canJump = true;
        } 
        
        else if (!isGrounded()) {
            canJump = false;
            jumpValue = 0;
            canMove = true;
        }

        if (jumpValue >= 11f && isGrounded())
        {
            float tempx = horizontalInput * speed;
            float tempy = jumpValue;
            rb.velocity = new Vector2(tempx, tempy);
        }



        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", isGrounded());
    }

    private void Jump()
    {
        jumpValue += 0.05f;
    }


    private bool isGrounded()
    {
        float extraHeightText = 0.1f;
        RaycastHit2D rayCastHit = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size, 0f,  Vector2.down, extraHeightText, platformLayerMask);
        Color rayColor;
        if(rayCastHit.collider != null)
        {
            rayColor = Color.green;
           // anim.SetTrigger("idle");
        } else
        {
           // anim.SetTrigger("jump");
            rayColor = Color.red;
        }
        Debug.DrawRay(boxCollider2d.bounds.center + new Vector3(boxCollider2d.bounds.extents.x, 0), Vector2.down * (boxCollider2d.bounds.extents.y + extraHeightText), rayColor);
        Debug.DrawRay(boxCollider2d.bounds.center - new Vector3(boxCollider2d.bounds.extents.x, 0), Vector2.down * (boxCollider2d.bounds.extents.y + extraHeightText), rayColor);
        Debug.DrawRay(boxCollider2d.bounds.center - new Vector3(boxCollider2d.bounds.extents.x, boxCollider2d.bounds.extents.y), Vector2.right * (boxCollider2d.bounds.extents.x), rayColor);
        return rayCastHit.collider != null;
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if(coll.collider.tag == "wall" && !canJump)
        {
            var playerSpeed = lastVelocity.magnitude;
            var direction = Vector3.Reflect(lastVelocity.normalized, coll.contacts[0].normal);

            rb.velocity = direction * Mathf.Max(playerSpeed, 0f);
        }
        
    }
}

