using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    Rigidbody2D rb;
    bool isGrounded;
    float moveX;
    bool isFalling;

    public float moveSpeed = 8f;
    public float jumpForce = 10f;

    public Transform groundCheck;
    public LayerMask groundLayer;
    public float checkRadius = 0.1f;
    public Vector2 groundCheckSize = new Vector2(0.8f, 0.1f);

    public Transform spawnPoint;
    public float respawnDelay = 1f;

    public CameraFollow cameraFollow;


    public float acceleration = 60f; // how quickly the player speeds up or slows down

    Animator anim;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        moveX = 0f;
        if (Keyboard.current.aKey.isPressed)
        {
            moveX = -1f;
        }
        else if (Keyboard.current.dKey.isPressed)
        {
            moveX = 1f;
        }

        // Flip the sprite based on movement direction
        if (moveX > 0)
        {
            transform.localScale = new Vector3(1, 1, 1); // facing right
        }
        else if (moveX < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1); // facing left
        }


        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        if (Keyboard.current.spaceKey.wasReleasedThisFrame && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }

        isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);
        isFalling = rb.linearVelocity.y < 0f;



        // Update animations
        anim.SetFloat("Speed", Mathf.Abs(moveX));
        anim.SetBool("isGrounded", isGrounded);

        if (!isGrounded)
        {
            anim.SetBool("isJumping", rb.linearVelocity.y > 0.01f);
            anim.SetBool("isFalling", rb.linearVelocity.y < -0.01f);
        }
        else
        {
            anim.SetBool("isJumping", false);
            anim.SetBool("isFalling", false);
        }

        if (transform.position.y < -10f) // adjust based on your level
        {
            StartCoroutine(RespawnCoroutine());
        }

    }
    void FixedUpdate()
    {
        // Target speed
        float targetSpeed = moveX * moveSpeed;

        // Smoothly move towards the target speed
        float smoothedX = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, acceleration * Time.fixedDeltaTime);

        rb.linearVelocity = new Vector2(smoothedX, rb.linearVelocity.y);

        if (transform.position.y < -15f) // adjust -10f depending on your level
        {
            Respawn();
        }

    }

    void Respawn()
    {
        // Reset position
        transform.position = spawnPoint.position;

        // Reset velocity
        rb.linearVelocity = Vector2.zero;
    }

    IEnumerator RespawnCoroutine()
    {
        // Trigger hit animation while player is visible
        anim.SetBool("isDead", true);

        // Wait for the animation to play (keep sprite visible)
        yield return new WaitForSeconds(0.5f); // adjust to match animation length

        // Stop movement and disable visuals/collider
        rb.linearVelocity = Vector2.zero;
        cameraFollow.enabled = false;

        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        // Wait for respawn delay (optional pause)
        yield return new WaitForSeconds(respawnDelay);

        // Move player to spawn and reset Rigidbody
        rb.position = spawnPoint.position;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.Sleep();
        rb.WakeUp();

        // Re-enable visuals, collider, and camera
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<Collider2D>().enabled = true;
        cameraFollow.enabled = true;

        // Reset animation
        anim.SetBool("isDead", false);
    }






}
