using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float acceleration = 0.5f;
    public float airAcceleration = 1.5f;
    public float maxSpeed = 11;
    public float groundFriction = 1.5f;
    public float airFriction = 2.5f;
    public float gravity = 0.1f;
    public float jumpForce = 25;
    public float terminalVelocity = -15;
    public float dashSpeed = 30;
    public float dashDuration = 0.1f;
    public float wallFriction = -10;
    public float wallJumpCooldown = 0.2f;
    public float jumpForgiveness = 0.1f;

    private float currentSpeedHorizontal = 0;
    private float currentAcceleration = 0;
    private float distToGround;
    private float distToGroundDiag;
    private float currentSpeedVertical = 0;
    private float currentJumpForgiveness = 0;
    private float dashDir = 0;

    private bool dashing = false;
    private bool hasDashed = false;
    private bool walljumping = false;
    private bool failedJump = false;
    //holy variables batman!

    Rigidbody2D RB;
    Collider2D col;
    PlayerHealth health;

    // Start is called before the first frame update
    void Start()
    {
        RB = gameObject.GetComponent<Rigidbody2D>();
        col = gameObject.GetComponent<Collider2D>();
        health = gameObject.GetComponent<PlayerHealth>();
        
        distToGround = col.bounds.extents.y + 0.02f;
        distToGroundDiag = distToGround + 0.05f;
    }
    //initialize some stuff

    // Update is called once per frame
    void Update()
    {
        if (!dashing) //dashing takes away control, so most of the controls are in a conditional avoiding dashing
        {
            bool grounded = isGrounded();
            var wallStatus = isWallSliding(); //collision :flushed:
            float movementDir = Input.GetAxis("Horizontal");
            if (movementDir != 0)
            {
                if (grounded) currentAcceleration = movementDir * acceleration;
                else if (!walljumping) currentAcceleration = movementDir * airAcceleration; //walljumping temporary takes away your ability to move in the air to get you away from the wall
                else currentAcceleration = 0;
                if (Math.Abs(currentSpeedHorizontal + currentAcceleration) < maxSpeed) currentSpeedHorizontal += currentAcceleration;   //cap speed magnitude
            }
            else if (currentSpeedHorizontal != 0)
            {
                if (grounded)
                {
                    if (Math.Abs(currentSpeedHorizontal) < groundFriction) currentSpeedHorizontal = 0;
                    else if (currentSpeedHorizontal > 0) currentSpeedHorizontal -= groundFriction;
                    else currentSpeedHorizontal += groundFriction;
                }

                else{
                    if (Math.Abs(currentSpeedHorizontal) < airFriction) currentSpeedHorizontal = 0;
                    else if (currentSpeedHorizontal > 0) currentSpeedHorizontal -= airFriction;
                    else currentSpeedHorizontal += airFriction;
                }
            }
            //friction baby!
            //i think unity just does this but i wrote what i knew oops

            if ((Input.GetKeyDown("x")) && !hasDashed)
            {
                dashing = true;
                hasDashed = true;
                if (Input.GetAxis("Horizontal") < 0) dashDir = -1;
                else dashDir = 1;
                StartCoroutine(Dash());
            }
            //if we're dashing, say so and get the direction

            if (grounded)
            {
                //if ((hasDashed == true) && Input.GetAxis("Fire1") == 0) hasDashed = false; //dashes only reset when you let go of the dash button to avoid just holding it and vooping
                hasDashed = false;
                currentSpeedVertical = 0;   //stading so no vertical movementz
                if (failedJump && currentJumpForgiveness > 0) {
                    failedJump = false;
                    transform.position = transform.position + new Vector3(0, 0.2f, 0); //character is a little stuck in the ground? force them out
                    currentSpeedVertical = jumpForce;   //boing
                }   //celeste style missed jump correction
                currentJumpForgiveness = 0;
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    transform.position = transform.position + new Vector3(0, 0.2f, 0); //character is a little stuck in the ground? force them out
                    currentSpeedVertical = jumpForce;   //boing
                }
            }

            else if (wallStatus.Item1) {    //if next to a wall is true
                if (Input.GetKey(KeyCode.Z))
                {
                    walljumping = true;
                    currentSpeedVertical = jumpForce;
                    if (wallStatus.Item2 == 'L') currentSpeedHorizontal = maxSpeed; //speed away from wall
                    else currentSpeedHorizontal = -maxSpeed;
                    StartCoroutine(WallJumpCooldown());
                }
                else currentSpeedVertical = wallFriction;  //slide at constant rate down
            }

            else
            {
                currentJumpForgiveness -= Time.deltaTime;
                if (ceilingAbove()) currentSpeedVertical = -5;  //bonk off of ceilings, ends jump early
                if (currentSpeedVertical > terminalVelocity) currentSpeedVertical -= gravity;
                if (Input.GetKeyDown(KeyCode.Z)) {
                    currentJumpForgiveness = jumpForgiveness;
                    failedJump = true;
                }
            }
        }
        else
        {
            currentSpeedHorizontal = dashSpeed * dashDir;
            currentSpeedVertical = 0;
        }
        
    }

    void FixedUpdate()
    {
        RB.MovePosition(transform.position + new Vector3(currentSpeedHorizontal, currentSpeedVertical, 0) * Time.deltaTime);    //move in fixed update to avoid jittering
    }

    bool isGrounded()
    {
        RaycastHit2D hit1 = Physics2D.Raycast(transform.position, Vector2.down, distToGround);
        RaycastHit2D hit2 = Physics2D.Raycast(transform.position + Vector3.right * 0.45f, Vector2.down, distToGround);
        RaycastHit2D hit3 = Physics2D.Raycast(transform.position + Vector3.left * 0.45f, Vector2.down, distToGround);
        return (hit1.collider != null) || (hit2.collider != null) || (hit3.collider != null);
    }

    Tuple<bool, char> isWallSliding() {
        RaycastHit2D hitL = Physics2D.Raycast(transform.position, Vector2.left, distToGround);
        RaycastHit2D hitR = Physics2D.Raycast(transform.position, Vector3.right, distToGround);
        if (hitL.collider != null && Input.GetAxis("Horizontal") < 0) return Tuple.Create(true, 'L');
        else if (hitR.collider != null && Input.GetAxis("Horizontal") > 0) return Tuple.Create(true, 'R');
        else return Tuple.Create(false, '-');
    }

    bool ceilingAbove()
    {
        RaycastHit2D hit1 = Physics2D.Raycast(transform.position, Vector2.up, distToGround);
        RaycastHit2D hit2 = Physics2D.Raycast(transform.position + Vector3.right * 0.45f, Vector2.up, distToGround);
        RaycastHit2D hit3 = Physics2D.Raycast(transform.position + Vector3.left * 0.45f, Vector2.up, distToGround);
        return (hit1.collider != null) || (hit2.collider != null) || (hit3.collider != null);
    }
    //boo boring raycasts boo booo

    IEnumerator Dash() {
        yield return new WaitForSeconds(dashDuration);
        dashing = false;
        currentSpeedHorizontal = 0;
    }

    IEnumerator WallJumpCooldown()
    {
        yield return new WaitForSeconds(wallJumpCooldown);
        walljumping = false;
    }
    //coroutines to make waiting for cooldowns easier
}
