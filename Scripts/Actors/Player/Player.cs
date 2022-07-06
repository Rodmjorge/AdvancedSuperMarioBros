using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Actor
{
    //Vector2[i][0] -> size; Vector2[i][1] -> offset
    public static readonly Vector2[][] boxcolliderSizes = new Vector2[][] {
        new Vector2[] { new Vector2(0.8f, 0.8f), new Vector2(0f, -.1f) }, //small size
        new Vector2[] { new Vector2(0.8f, 0.5f), new Vector2(0f, -.25f) }, //crouched small size
        new Vector2[] { new Vector2(0.85f, 1.7f), new Vector2(0f, -.15f) }, //big size
        new Vector2[] { new Vector2(0.85f, 0.9f), new Vector2(0f, -.05f) } //crouched big size
    };
    public static readonly float[] shiftYWhenHolding = new float[] {
        0.15f, //small size
        0f //crouched small size
    };

    public override void DataLoaded(string s, string beforeEqual) { return; }

    public LayerMask layerMask { get { return LayerMaskInterface.grounded; } }

    internal bool isWalking { get { return Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D); } }
    internal bool isCrouching { get { return Input.GetKey(KeyCode.S); } }


    private float moveSpeed = 4f;

    private readonly float jumpTime = 0.4f;
    private float jumpTimer;
    private bool isJumping;

    private bool isHoldingSomething;
    private Actor whatIsHolding;

    public override void Tick()
    {
        anim.speed = IsFalling() ? 0 : 1;

        Walking();
        Jumping();
        Crouching();
    }

    public override void Framed()
    {
        anim.SetBool("holding", isHoldingSomething);

        if (isHoldingSomething) {
            if (!Input.GetKey(GetHoldingKeyCode())) {
                whatIsHolding.Thrown(this);
                NotHoldingAnything();

                return;
            }
            else if (whatIsHolding.pauseActor || !whatIsHolding.isBeingHeld || whatIsHolding == null) {
                NotHoldingAnything();
                return;
            }
            
            Vector3 v = new Vector3(spriteR.flipX ? -0.55f : 0.55f, shiftYWhenHolding[GetIndexOfBCS()]);
            whatIsHolding.transform.position = this.transform.position + v;
        }
    }

    public override void PausedTick()
    {
        if (Input.GetKey(KeyCode.R)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            LevelManager.StatusOfScene(false, true);
        }
    }

    public void Walking()
    {
        anim.SetBool("walking", IsMoving() && !IsJumping());

        if (IsWalking()) {
            bool isLeft = Input.GetKey(KeyCode.A);
            spriteR.flipX = isLeft;

            if (!isCrouching || (isCrouching && isJumping)) rigidBody.velocity = RigidVector(isLeft ? -moveSpeed : moveSpeed, null);
            else rigidBody.velocity = RigidVector(0, null);
        }
        else
            rigidBody.velocity = RigidVector(0, null);
    }

    public void Jumping()
    {
        anim.SetBool("jumping", IsJumping());

        if (Input.GetKey(KeyCode.Space) && jumpTimer > 0f) {
            if (!isJumping) AudioManager.PlayAudio(GetJumpSound());

            jumpTimer -= (!CeilingCollider()) ? Time.fixedDeltaTime : jumpTimer;

            rigidBody.velocity = RigidVector(null, GetJumpForce());
            isJumping = true;
        }
        else {
            bool grounded = IsOnGround();

            jumpTimer = grounded ? jumpTime + (IsWalking() ? jumpTime / 4f : 0f) : 0f;
            isJumping = grounded ? false : isJumping;
        }
    }

    public void Crouching()
    {
        anim.SetBool("crouching", IsCrouching());

        int index = GetIndexOfBCS();
        bcs.SetBoxCollider(boxcolliderSizes[index][0], boxcolliderSizes[index][1]);
    }

    public override void Collided(GameObject GO, Actor actor) { StayingCollided(GO, actor); }
    public override void StayingCollided(GameObject GO, Actor actor)
    {
        if (actor.IsHoldable() && Input.GetKey(GetHoldingKeyCode())) {

            actor.StartedHolding(this);
            actor.ChangeHoldingStatus(true);

            isHoldingSomething = true;

            whatIsHolding = actor;
        }

        base.StayingCollided(GO, actor);
    }


    internal void AddJump(float time = 0.4f, float multiplied = 1.5f)
    {
        rigidBody.velocity = RigidVector(null, GetJumpForce() * multiplied);
        jumpTimer = time;
    }
    internal void HitPlayer(GameObject GO, Actor actor)
    {
        anim.SetBool("death", true);
        LevelManager.StatusOfScene(true);
    }

    private bool IsWalking() { return isWalking; }
    private bool IsMoving() { return rigidBody.velocity.x < -0.25f || rigidBody.velocity.x > 0.25f; }
    public bool IsJumping() { return isJumping; }
    public bool IsCrouching() { return isCrouching; }
    public bool CeilingCollider()
    {
        bool b = ColliderCheck.CollidedWithWall(ColliderCheck.WallDirection.Ceiling, boxCollider, layerMask);
        if (b) AudioManager.PlayAudio("hit_block");

        return b;
    }

    public void NotHoldingAnything()
    {
        isHoldingSomething = false;
        whatIsHolding = null;
    }

    public bool IsOnGround() { return ColliderCheck.CollidedWithWall(ColliderCheck.WallDirection.Ground, boxCollider, layerMask); }
    private bool IsFalling() { return !IsOnGround(); }
    private int GetIndexOfBCS() { return (IsCrouching() && !IsJumping()) ? 1 : 0; }

    private float GetJumpForce() { return 9f; }
    private string GetJumpSound() { return "small_mario_jump"; }


    public static KeyCode GetHoldingKeyCode() { return KeyCode.UpArrow; }
    public static bool IsPlayer(GameObject GO) { return GO.GetComponent<Player>() != null; }

    public static bool IsPlayer(GameObject GO, out Player player) {
        player = GO.GetComponent<Player>();
        return player != null; 
    }

}
