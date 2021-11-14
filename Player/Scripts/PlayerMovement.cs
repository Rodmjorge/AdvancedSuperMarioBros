using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    private Rigidbody2D rigidBody;
    private SpriteRenderer sprite;
    private Animator animation;
    public LayerMask layerMask;

    public float moveSpeed = 4f;
    public float jumpForce = 6f;

    private bool isMoving = default;
    private bool isMovingRight = default;
    private bool pressedMovingKey = default;

    private bool jumping = default;
    private bool canJump = default;
    public float jumpTime = 0.5f;
    private float jumpTimer = 0;

    void Start()
    {
        boxCollider = GetComponents<BoxCollider2D>()[0];
        rigidBody = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animation = GetComponent<Animator>();
    }


    void FixedUpdate()
    {
        if (!LevelSettings.playerDied) {
            Moving();
            Jumping();
        }
        else {
            rigidBody.velocity = Vector2.zero;
        }
    }

    public void Moving() {
        float f = 1f;
        f = (Input.GetKey(KeyCode.UpArrow)) ? 1.5f : 1f;

        if (Input.GetKey(KeyCode.D)) {

            bool b = !CollisionCheck.isTouchingRightWall(boxCollider, layerMask);
            rigidBody.velocity = b ? new Vector2(moveSpeed, rigidBody.velocity.y) : new Vector2(0f, rigidBody.velocity.y);

            isMoving = true;
            isMovingRight = true;

            if (isOnGround() && b) { animation.Play("Walking_Small"); }

            sprite.flipX = false;
            pressedMovingKey = true;
        }
        else if (Input.GetKey(KeyCode.A)) {

            bool b = !CollisionCheck.isTouchingLeftWall(boxCollider, layerMask);
            rigidBody.velocity = b ? new Vector2(-moveSpeed, rigidBody.velocity.y) : new Vector2(0f, rigidBody.velocity.y);

            isMoving = true;
            isMovingRight = false;

            if (isOnGround() && b) { animation.Play("Walking_Small"); }

            sprite.flipX = true;
            pressedMovingKey = true;
        }

        else { isMoving = false; }

        if (!isMoving && rigidBody.velocity.x != 0 && pressedMovingKey) {

            if (isMovingRight) {
                if (rigidBody.velocity.x > 0) {
                    rigidBody.velocity = new Vector2(rigidBody.velocity.x - Time.deltaTime * (moveSpeed * 5), rigidBody.velocity.y);
                }

                else {
                    rigidBody.velocity = new Vector2(0f, rigidBody.velocity.y);
                    pressedMovingKey = false;

                    if (!isJumping()) { animation.Play("Stopped_Small"); }
                }
            }

            if (!isMovingRight) {
                if (rigidBody.velocity.x < 0) {
                    rigidBody.velocity = new Vector2(rigidBody.velocity.x + Time.deltaTime * (moveSpeed * 5), rigidBody.velocity.y);
                }

                else {
                    rigidBody.velocity = new Vector2(0f, rigidBody.velocity.y);
                    pressedMovingKey = false;

                    if (!isJumping()) { animation.Play("Stopped_Small"); }
                }
            }
        }

        if (rigidBody.velocity.x == 0 && rigidBody.velocity.y == 0 && !isJumping() && !isWalking()) {
            animation.Play("Stopped_Small");
        }
    }

    public Vector2 MovementVelocity(float speed) {

        //smoothly start walking
        return new Vector2(speed, rigidBody.velocity.y); 
    }

    public void Jumping() {

        if (Input.GetKey(KeyCode.Space) && jumpTimer > 0 && canJump) {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpForce);

            jumping = true;
            jumpTimer -= Time.deltaTime;
            animation.Play("Jumping_Small");
        }
        else {
            canJump = isOnGround();

            jumping = false;
            if (!isOnGround()) { animation.Play("Jumping_Small"); }
        }

        if (isOnGround()) {
            jumpTimer = jumpTime;
            if (!isWalking() && !pressedMovingKey) { animation.Play("Stopped_Small"); }
        }

        jumpTimer = CollisionCheck.isTouchingCeiling(boxCollider, layerMask) ? 0f : jumpTimer;
    }

    public bool isWalking() {
        return isMoving;
    }

    public bool isWalkingRight() {
        return isMovingRight;
    }

    public bool isJumping() {
        return jumping;
    }

    public bool isOnGround() {
        return CollisionCheck.isOnGround(boxCollider, layerMask);
    }
    
    public void ResetJump() {
        canJump = true;
        jumpTimer = jumpTime - jumpTime / 5f;
    }

    public void ResetVelocityY()
    {
        jumpTimer = 0f;
    }
}
