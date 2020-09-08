using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Ninja Runner Player Controller script

public class PlayerController : MonoBehaviour {
    public GameObject player;

    private static int maxHealth = 3;
    private static int currHealth;

    private static int hookCount = 0;
    private static int logCount = 0;

    public static float invulnTime = 1.5f; //time, in seconds
    public static float invulnValue;

    public float baseSpeed;
    public float adjustedSpeed = 0.0f;
    private float gameSpeedMultiplier = 1;
    private bool isFrozen = false;
    public bool isHooking = false;
    public bool hitWall = false;
    public bool hitObstacle = false;
    public bool hitGoal = false;

    public float jumpForce;
    public int extraJumpsValue;
    private int extraJumps;

    //Jump speed variables, depends on how long Jump was pressed
    public float fallMultiplier;
    public float lowJumpMultiplier;

    private bool isGrounded;
    private bool wasGrounded;
    private float fallCount = 0;

    public LayerMask whatIsGround;
    public LayerMask whatIsWall;
    public LayerMask whatIsObstacle;

    private Collider2D col;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sRen;

    private Color defCol;
    public static PlayerController instance;

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
    void Start() {
        currHealth = maxHealth;
        extraJumps = extraJumpsValue;

        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        sRen = GetComponent<SpriteRenderer>();

        defCol = GetComponent<SpriteRenderer>().color;
    }

    private void FixedUpdate()
    {
        //Is the Player touching the ground?
        wasGrounded = isGrounded;
        isGrounded = Physics2D.IsTouchingLayers(col, whatIsGround);
        hitWall = Physics2D.IsTouchingLayers(col, whatIsWall);

        if (!isFrozen && !isHooking || currHealth > 0)
        {
            if (rb.velocity.x != adjustedSpeed)
            {
                rb.velocity = new Vector2(adjustedSpeed, rb.velocity.y);
            }
        }
        UpdateAnimVariables();

    }

    void Update() {

        if (currHealth > maxHealth)
        {
            currHealth = maxHealth;
        }

        if (invulnValue > 0)
        {
            sRen.enabled = !sRen.enabled;
            invulnValue -= Time.deltaTime;
        }
        else
        {
            //hitObstacle = false;
            sRen.enabled = true;
            invulnValue = 0;
        }

        if (!isFrozen && !isHooking)
        {
            GetComponent<SpriteRenderer>().color = defCol;
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
    }

    private void UpdateAnimVariables()
    {
        if (isGrounded)
        {
            if (hitObstacle)
            {
                anim.SetTrigger("Rolling");
                return;
            }
            if (currHealth <= 0)
            {
                anim.SetTrigger("Dead");
                return;
            }

            if (!wasGrounded)
            {
                anim.SetTrigger("Landing");
                if (fallCount >= 17)
                {
                    anim.SetTrigger("Rolling");
                }
                fallCount = 0;
                return;
            }
            if (Input.GetButtonDown("Jump") && !isFrozen)
            {
                anim.SetTrigger("TakeOff");
                return;
            }
        }
        else
        {
            if(currHealth <= 0)
            {
                anim.SetTrigger("FallingDeath");
                return;
            }
            if (rb.velocity.y > 3)
            {
                anim.SetTrigger("JumpUp");
                return;
            }
            else if (rb.velocity.y < -2)
            {
                fallCount += 1;
                anim.SetTrigger("Falling");
                return;
            }
            else
            {
                anim.SetTrigger("HangTime");
                return;
            }
        }

        //not doing anything else, just running
        if (isGrounded && currHealth > 0)
        {
            if (!isFrozen && Input.GetAxisRaw("Vertical") < -0.2f)
            {
                anim.SetTrigger("Slide");
                return;
            }
            else
            {
                anim.SetTrigger("Running");
            }
        }
    }

    public void Respawn(Vector2 platPos)
    {
        rb.velocity = Vector2.zero;
        transform.position = new Vector2(platPos.x - 1, platPos.y + 1);
    }

    public IEnumerator HookToCoroutine(Vector3 target)
    {
        float ogScale = rb.gravityScale;

        isHooking = true;
        col.enabled = false;
        rb.velocity = Vector3.zero;
        rb.gravityScale = 0;

        Debug.DrawLine(transform.position, target, Color.yellow);
        Debug.Log(Vector3.Distance(transform.position, target));
        //Debug.Break();
      
        while (transform.position.y < target.y - 1f)
        {
            transform.position = Vector3.Lerp(transform.position, target, 7f * Time.deltaTime);
            yield return null;
        }

        isHooking = false;
        col.enabled = true;
        rb.gravityScale = ogScale;
    }

    public void UpdateMultiplier(float inMult)
    {
        gameSpeedMultiplier = inMult;
    }

    public void ToggleFreeze(bool inB)
    {
        isFrozen = inB;
        rb.velocity = Vector2.Lerp(rb.velocity, new Vector3(0, rb.velocity.y, 0), 0.5f);
    }
    public float YVelocity()
    {
        return rb.velocity.y;
    }
    public Vector3 PlayerPosition()
    {
        return rb.transform.position;
    }

    public void TakeDamage(int amt)
    {
        currHealth -= amt;
        if (currHealth <= 0)
        {
            currHealth = 0;
            return;
        }
        invulnValue = invulnTime;
    }
    public static int GetCurrentHealth()
    {
        return currHealth;
    }
    public static int GetMaximumHealth()
    {
        return maxHealth;
    }
    public static int GetHookCount()
    {
        return hookCount;
    }
    public static void SetHookCount(int n)
    {
        hookCount = n;
    }
    public static int GetLogCount()
    {
        return logCount;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            collision.collider.enabled = false;
            if (invulnValue <= 0)
            {
                collision.gameObject.SetActive(false);
                hitObstacle = true;
            }
        }
        if (collision.gameObject.CompareTag("Finish"))
        {
            hitGoal = true;
        }
    }
}
