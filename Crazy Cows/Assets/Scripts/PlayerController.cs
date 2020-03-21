using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // controls
    public KeyCode left;
    public KeyCode right;
    public KeyCode jump;
    public KeyCode fire;
    public KeyCode attack;

    // range attack variables
    public GameObject bullet;
    public Transform shootPoint;
    private float timeBtwShoot;
    public float startTimeBtwShoot;
    public float laserKnockback;

    private bool canJumpAfterHit = false;
    private bool stunned;

    // movement variables
    public float speed;
    public float wallSlideSpeed;
    public float jumpForce;
    public float wallJumpHorizontalForce;
    public float wallJumpVerticalForce;
    public float spriteScale;

    private Rigidbody2D rb;
    
    public float pointRadius;
    public LayerMask platform;
    public LayerMask player;

    private bool onPlatform;
    private bool onLeftWall;
    private bool onRightWall;
    private bool onPlayer;

    public Transform TopLeftPoint;
    public Transform TopRightPoint;
    public Transform BottomLeftPoint;
    public Transform BottomRightPoint;
    public Transform RightGroundPoint;
    public Transform LeftGroundPoint;

    private float timeBtwAttack;
    public float startTimeBtwAttack;

    public Transform attackPoint;
    public float attackRange;
    public LayerMask otherPlayer;
    public GameObject enemy;
    public int damage;
    public Transform enemyBloodSplatterPoint;
    public float meleeAttackKnockback;

    public GameObject bloodEffect;
    public Transform bloodEffectPoint;
    public GameObject deathEffect;

    public float flashTime;
    private bool flash;
    private float savedTime;

    public GameObject scoreScreen;

    private Animator anim;

    void Start()
    {
        if(GameObject.Find("ScoreScreen") != null)
        {
            scoreScreen = GameObject.Find("ScoreScreen");
        }
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), enemy.GetComponent<Collider2D>());
    }

    void Update()
    {
        if (flash && (int)GetComponent<SpriteRenderer>().color.b == 1)
            GetComponent<SpriteRenderer>().color = new Color(1, 0.3f, 0.3f);
        else if (flash && Time.time - savedTime > flashTime)
        {
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
            flash = false;
        }

        // check if player is on ground, on other player, or on wall
        onPlatform = Physics2D.OverlapCircle(LeftGroundPoint.position, pointRadius, platform) || Physics2D.OverlapCircle(RightGroundPoint.position, pointRadius, platform);
        onPlayer = Physics2D.OverlapCircle(LeftGroundPoint.position, pointRadius, player) || Physics2D.OverlapCircle(RightGroundPoint.position, pointRadius, player);
        onLeftWall = (Physics2D.OverlapCircle(TopLeftPoint.position, pointRadius, platform) || Physics2D.OverlapCircle(BottomLeftPoint.position, pointRadius, platform)) && !onPlatform && !onPlayer;
        onRightWall = (Physics2D.OverlapCircle(TopRightPoint.position, pointRadius, platform) || Physics2D.OverlapCircle(BottomRightPoint.position, pointRadius, platform)) && !onPlatform && !onPlayer;
        if(onPlatform && stunned)
        {
            stunned = false;
        }

        // directional movement
        if (Input.GetKey(left))
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
        }
        else if (Input.GetKey(right))
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        
        // wall sliding
        if(onLeftWall || onRightWall)
        {
            rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
        }

        // jump movement
        if (Input.GetKeyDown(jump))
        {
            if (onPlatform || onPlayer)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }

            if (canJumpAfterHit)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                canJumpAfterHit = false;
            }

            if (transform.localScale.x < 0) // if facing right
            {
                if (onRightWall)
                {
                    if (!Input.GetKey(right))
                    {
                        rb.velocity = new Vector2(-wallJumpHorizontalForce, wallJumpVerticalForce);
                    } else if(Input.GetKey(right))
                    {
                        rb.velocity = new Vector2(-wallJumpHorizontalForce-50, wallJumpVerticalForce);
                    }
                }
            } else if(transform.localScale.x > 0) // if facing left
            {
                if (onRightWall)
                {
                    if(!Input.GetKey(left))
                    {
                        rb.velocity = new Vector2(wallJumpHorizontalForce, wallJumpVerticalForce);
                    } else if (Input.GetKey(left))
                    {
                        rb.velocity = new Vector2(wallJumpHorizontalForce+50, wallJumpVerticalForce);
                    }
                }
            } 
        }

        // shooting
        if (timeBtwShoot <= 0 && !stunned) // if can shoot
        {
            if (Input.GetKeyDown(fire))
            {
                if (!onLeftWall && !onRightWall)
                {
                    timeBtwShoot = startTimeBtwShoot; // reset delay between shots
                    GameObject bulletClone = Instantiate(bullet, shootPoint.position, shootPoint.rotation);
                    if (transform.localScale.x > 0) // if facing left
                    {
                        bulletClone.transform.localScale = new Vector3(-bulletClone.transform.localScale.x, bulletClone.transform.localScale.y);
                    }
                    else if (transform.localScale.x < 0) // if facing right
                    {
                        bulletClone.transform.localScale = bulletClone.transform.localScale;
                    }
                    anim.SetTrigger("Shoot");
                }
            }
        } else
        {
            if(timeBtwShoot > 0)
            {
                timeBtwShoot -= Time.deltaTime; // decrease delay
            }
        }

        // melee
        if (timeBtwAttack <= 0 && !stunned)
        {
            if (!onLeftWall && !onRightWall)
            {
                if (Input.GetKeyDown(attack))
                {
                    timeBtwAttack = startTimeBtwAttack;
                    bool playerHit = Physics2D.OverlapCircle(attackPoint.position, attackRange, otherPlayer);
                    if (playerHit)
                    {
                        if (enemy.tag == "Player1")
                        {
                            GameObject.Find("Player1").GetComponent<PlayerController>().changeJumpAfterHit();
                            GameObject.Find("Player1").GetComponent<PlayerController>().setFlashTrue();
                            MakeBlood(enemyBloodSplatterPoint);
                            FindObjectOfType<GameManager>().DamageP1(damage);
                        }
                        else if (enemy.tag == "Player2")
                        {
                            GameObject.Find("Player2").GetComponent<PlayerController>().changeJumpAfterHit();
                            GameObject.Find("Player2").GetComponent<PlayerController>().setFlashTrue();
                            MakeBlood(enemyBloodSplatterPoint);
                            FindObjectOfType<GameManager>().DamageP2(damage);
                        }
                        if (enemy.transform.position.x < gameObject.transform.position.x)
                        {
                            enemy.GetComponent<Rigidbody2D>().velocity = new Vector2(-meleeAttackKnockback-50, meleeAttackKnockback+5);

                        }
                        else if (enemy.transform.position.x > gameObject.transform.position.y)
                        {
                            enemy.GetComponent<Rigidbody2D>().velocity = new Vector2(meleeAttackKnockback+50, meleeAttackKnockback+5);
                        }
                    }
                    anim.SetTrigger("Attack");
                }
            }
        }
        else
        {
            timeBtwAttack -= Time.deltaTime;
        }

        // change facing direction based on movement
        if (rb.velocity.x < 0)
        {
            transform.localScale = new Vector3(spriteScale, spriteScale, 1);
        } else if(rb.velocity.x > 0)
        {
            transform.localScale = new Vector3(-spriteScale, spriteScale, 1);
        }

        anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        anim.SetBool("onGround", onPlatform || onPlayer);
        anim.SetBool("onWall", onLeftWall || onRightWall);
    }

    public void MakeBlood(Transform point)
    {
        Instantiate(bloodEffect, point.position, point.rotation);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Laser")
        {
            flash = true;
            savedTime = Time.time;
            stunned = true;
            canJumpAfterHit = true;
            Instantiate(bloodEffect, bloodEffectPoint.position, bloodEffectPoint.rotation);
            if(enemy.transform.position.x < gameObject.transform.position.x)
            {
                rb.velocity = new Vector2(laserKnockback+10, laserKnockback+10);
            } else if(enemy.transform.position.x > gameObject.transform.position.x)
            {
                rb.velocity = new Vector2(-laserKnockback-10, laserKnockback+10);
            }
        }
        if(collision.gameObject.tag == "Boundary" && !scoreScreen.activeSelf)
        {
            Instantiate(deathEffect, transform.position, transform.rotation);
            if (gameObject.tag == "Player1")
            {
                FindObjectOfType<GameManager>().DamageP1(FindObjectOfType<GameManager>().P1Health);
            } else if(gameObject.tag == "Player2")
            {
                FindObjectOfType<GameManager>().DamageP2(FindObjectOfType<GameManager>().P2Health);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
    public void changeJumpAfterHit()
    {
        canJumpAfterHit = true;
    }

    public void setFlashTrue()
    {
        flash = true;
        savedTime = Time.time;
    }
}