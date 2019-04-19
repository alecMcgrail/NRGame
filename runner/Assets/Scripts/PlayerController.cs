using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Ninja Runner Player Controller script

public class PlayerController : MonoBehaviour {
    public GameObject       player;

    public static int       maxHealth = 3;
    public static int       currHealth;

    public float            baseSpeed;
    public float            adjustedSpeed = 0.0f;
    private float           gameSpeedMultiplier = 1;
    private bool            isFrozen = false;
    public bool             hitWall = false;
    public bool             hitObstacle = false;
    public bool             hitGoal = false;

    public float            jumpForce;
    public int              extraJumpsValue;
    private int             extraJumps;

    //Jump speed variables, depends on how long Jump was pressed
    public float            fallMultiplier;
    public float            lowJumpMultiplier;

    private bool            isGrounded;
    public LayerMask        whatIsGround;
    public LayerMask        whatIsWall;
    public LayerMask        whatIsObstacle;

    private Collider2D      col;
    private Rigidbody2D     rb;
    private Animator        anim;

    private Color                       defCol;
    public static PlayerController      instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Player Controller: more than one Player Controller in the Scene.");
        }
        else
        {
            instance = this;
        }
    }

    //Initialize
    void Start () {
        currHealth = maxHealth;
        extraJumps = extraJumpsValue;

        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();

        defCol = GetComponent<SpriteRenderer>().color;
    }

    private void FixedUpdate()
    {
        //Is the Player touching the ground?
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.IsTouchingLayers(col, whatIsGround);
        hitWall = Physics2D.IsTouchingLayers(col, whatIsWall);

        if(isGrounded && !wasGrounded)
        {
            anim.SetTrigger("Landed");
        }

        if (!isFrozen)
        {
            if (rb.velocity.x != adjustedSpeed)
            {
                rb.velocity = new Vector2(adjustedSpeed, rb.velocity.y);
            }
        }
  
    }

    void Update () {

        if (currHealth > maxHealth)
        {
            currHealth = maxHealth;
        }

        if (!isFrozen)
        {

            if (isGrounded)
            {
                //Reset jumps
                extraJumps = extraJumpsValue;
            }

            //Update speed
            adjustedSpeed = baseSpeed * gameSpeedMultiplier;

            //Jumping stuff
            if (Input.GetButtonDown("Jump")
                && (isGrounded || extraJumps > 0))
            {
                rb.velocity = new Vector2(rb.velocity.x, Vector2.up.y * jumpForce);
                if (!isGrounded)
                {
                    extraJumps -= 1;
                }
            }
            else if (rb.velocity.y < 0)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
            else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
            {
                rb.velocity += Vector2.up * Physics2D.gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
        }

        UpdateAnimVariables();
    }

    private void UpdateAnimVariables()
    {
        anim.SetBool("Is Grounded", isGrounded);
        anim.SetFloat("Y Velocity", rb.velocity.y);

    }

    public void Respawn(Vector2 platPos)
    {
        rb.velocity = Vector2.zero;
        transform.position = new Vector2(platPos.x - 1, platPos.y + 1);
    }

    public void UpdateMultiplier(float inMult)
    {
        gameSpeedMultiplier = inMult;
    }

    public void ToggleFreeze()
    {
        isFrozen = !isFrozen;
    }
    public void ToggleFreeze(bool inB)
    {
        isFrozen = inB;
    }
    public float YVelocity()
    {
        return rb.velocity.y;
    }

    public void TakeDamage(int amt)
    {
        currHealth -= amt;
        if (currHealth < 0)
        {
            currHealth = 0;
        }
    }
    public static int CurrentHealth()
    {
        return currHealth;
    }
    public static int MaximumHealth()
    {
        return maxHealth;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            collision.collider.enabled = false;
            collision.gameObject.SetActive(false);
            hitObstacle = true;
        }
        if (collision.gameObject.CompareTag("Finish"))
        {
            hitGoal = true;
        }
    }
}
