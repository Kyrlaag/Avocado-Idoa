using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    private float inputX;
    private Rigidbody2D rb;
    private RaycastHit2D hit;
    private bool isFaceRight = true;
    private float coyoteTime = 0.1f;
    private float coyoteTimeCounter;
    private float jumpBuffer = 0.1f;
    private float jumpBufferCounter;
    [SerializeField] private float rayOffset = 0.1f;
    [SerializeField] private float rayLenght = 0.1f;
    private Vector2[] groundRayVectors = new Vector2[3];
    private Vector2[] wallRayVectors = new Vector2[2];
    private bool isTouchRight;
    private bool isTouchLeft;
    [SerializeField]private bool canWallJump;

    private bool isWallSliding;
    private float wallSlidingSpeed = 2f;
    [SerializeField] private Transform wallCheck;

    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.2f;
    private Vector2 wallJumpingPower = new Vector2(10f, 30f);

    [SerializeField] private Animator playerAnimator;
    private float normalGravity;

    [SerializeField] private GameObject ladderPrefab;
    [SerializeField] private bool canLadderUse;
    private Vector2 ladderRayPoint;
    private List<Vector2> ladderPositions;

    private float inputY;
    private bool isLadder;
    private bool isClimbing;
    [SerializeField] private float climbingSpeed;
    private int ladderCount;

    private Vector2 checkPoint;



    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        normalGravity = rb.gravityScale;
        checkPoint = transform.position;
    }


    void Update()
    {
        //input for movement
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");


        //face direction
        if (!isWallJumping)
        {
            if ((inputX < 0 && isFaceRight) || (inputX > 0 && isFaceRight == false))
                Flip();
        }
            


        //jump
        Jump();

        //ground check rays
        float temp = 0.5f - rayOffset; 
        Debug.DrawRay(new Vector2(transform.position.x, transform.position.y - 0.4f), Vector2.down * rayLenght, Color.blue);
        Debug.DrawRay(new Vector2(transform.position.x - temp, transform.position.y - 0.4f), Vector2.down * rayLenght, Color.blue);
        Debug.DrawRay(new Vector2(transform.position.x + temp, transform.position.y - 0.4f), Vector2.down * rayLenght, Color.blue);



        //Wall jump
        if (canWallJump)
        {
            WallSlide();
            WallJump();
        }


        //Ladder
        if (canLadderUse)
        {
            CreateLadder();
        }

        if (isLadder && inputY != 0)
            isClimbing = true;




    }
    


    void FixedUpdate()
    {
        if (!isWallJumping)
        {
            rb.velocity = new Vector2(inputX * moveSpeed, rb.velocity.y);
            if (inputX != 0 && GroundCheck() == true)
                playerAnimator.SetBool("isWalking", true);
            else
                playerAnimator.SetBool("isWalking", false);

        }

        if (isClimbing)
        {
            rb.gravityScale = 0f;
            rb.velocity = new Vector2(rb.velocity.x, inputY * climbingSpeed);
        }
        else if (IsWalled() == false) 
            rb.gravityScale = normalGravity;

    }

    private void CreateLadder()
    {
        if (canLadderUse == true && Input.GetKeyDown(KeyCode.E))
        {
            hit = Physics2D.Raycast(transform.position, Vector2.down, 1f, LayerMask.GetMask("Ground"));
            if (hit.collider != null && hit.collider.tag == "LadderArea")
            {
                ladderPositions = new List<Vector2>();
                ladderRayPoint = new Vector2(hit.point.x, hit.point.y + 0.01f);
                for (int i = 1; i <= 15; i++)
                {
                    Debug.DrawRay(ladderRayPoint, Vector2.up * 1f, Color.blue);
                    hit = Physics2D.Raycast(ladderRayPoint, Vector2.up, 1f, LayerMask.GetMask("Ground"));
                    if (hit.collider != null)
                        break;

                    ladderPositions.Add(new Vector2(ladderRayPoint.x, ladderRayPoint.y));
                    ladderRayPoint = new Vector2(ladderRayPoint.x, ladderRayPoint.y + 1f);
                }

                foreach (var position in ladderPositions)
                {

                    Instantiate(ladderPrefab, position, Quaternion.identity);


                }
            }
        }
    }
    private void Jump()
    {
        //Timers
        if (GroundCheck())
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;

        if (Input.GetButtonDown("Jump"))
            jumpBufferCounter = jumpBuffer;
        else
            jumpBufferCounter -= Time.deltaTime;

        //Jump starter
        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
        {
            FindObjectOfType<AudioManager>().Play("Jump");
            rb.velocity = Vector2.up * jumpForce;
            jumpBufferCounter = 0f;
        }

        //Jump ender
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            coyoteTimeCounter = 0f;
        }
    }

    private bool GroundCheck()
    {
        float offset = 0.5f - rayOffset;
        groundRayVectors[0] = new Vector2(transform.position.x, transform.position.y - 0.4f);
        groundRayVectors[1] = new Vector2(transform.position.x - offset, transform.position.y - 0.4f);
        groundRayVectors[2] = new Vector2(transform.position.x + offset, transform.position.y - 0.4f);

        foreach (Vector2 rayVec in groundRayVectors)
        {
            hit = Physics2D.Raycast(rayVec, Vector2.down, rayLenght, LayerMask.GetMask("Ground"));

            if (hit.collider != null)
                return true;
        }

        return false;
    }

    

    private void WallSlide()
    {
        if (IsWalled() && !GroundCheck() && inputX != 0)
        {
            playerAnimator.SetBool("isWallSliding", true);
            isWallSliding = true;
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f;
        }
        else if (IsWalled() && !GroundCheck())
        {
            playerAnimator.SetBool("isWallSliding", true);
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
            rb.gravityScale = normalGravity;
        }
        else
        {
            playerAnimator.SetBool("isWallSliding", false);
            isWallSliding = false;
            rb.gravityScale = normalGravity;

        }
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, LayerMask.GetMask("Ground"));
    }

    private void WallJump()
    {


        //Wall Jump
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f)
        {

            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            if (transform.localScale.x != wallJumpingDirection)
            {
                Flip();
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
        
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }
    private void Flip()
    {
        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        isFaceRight = !isFaceRight;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isLadder = true;
            ladderCount++;
        }
        if (collision.CompareTag("Tutorial"))
        {
            collision.GetComponent<CanvasGroup>().alpha = 1f;
        }
        if (collision.CompareTag("DeadZone"))
        {
            FindObjectOfType<GameManager>().ReloadLevel();
        }
        if (collision.CompareTag("CheckPoint"))
        {
            checkPoint = collision.transform.position;
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            ladderCount--;
            if (ladderCount == 0)
            {
                isLadder = false;
                isClimbing = false;
            }
            
        }
        if (collision.CompareTag("Tutorial"))
        {
            collision.GetComponent<CanvasGroup>().alpha = 0f;
        }
        if (collision.CompareTag("PowerUp"))
        {
            Destroy(collision.gameObject, 0.5f);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            FindObjectOfType<GameManager>().ReloadLevel();
        }
    }

    public void ReturnToCheckPoint()
    {
        rb.velocity = Vector2.zero;
        transform.position = checkPoint;
        rb.gravityScale = normalGravity;
    }
}
