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
        new Vector2[] { new Vector2(0.85f, 0.9f), new Vector2(0f, -.55f) } //crouched big size
    };
    public static readonly float[] shiftYWhenHolding = new float[] {
        0.15f, //small size
        0f, //crouched small size
        0.3f, //big size
        -0.2f, //crouched big size
    };

    /* 0 - small mario
     * 1 - mushroom
     * 2 - fire flower
     */
    public int powerup = 0;
    protected bool isBig { get { return powerup > 0; } }


    public override void DataLoaded(string s, string beforeEqual) { return; }

    public LayerMask layerMask { get { return LayerMaskInterface.grounded; } }

    internal bool isWalking { get { return Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D); } }
    internal bool isCrouching { get { return Input.GetKey(KeyCode.S); } }


    private float moveSpeed = 4f;

    private readonly float jumpTime = 0.4f;
    private float jumpTimer;
    private bool isJumping;

    internal bool isHoldingSomething;
    private Actor whatIsHolding;

    public override void Start()
    {
        StartCoroutine(GotPowerup(1, this));
        base.Start();
    }

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
            if (!IsTryingToHold()) {
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

            if (grounded) scoreManager.ResetIndex();
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


    internal void AddJump(bool addToScore, float time = 0.4f, float multiplied = 1.5f)
    {
        rigidBody.velocity = RigidVector(null, GetJumpForce() * multiplied);
        jumpTimer = time;

        if (addToScore) scoreManager.AddIndex(1, true);
    }

    internal IEnumerator GotPowerup(int i, Actor actor, float[] f = null, string s = "powerup", bool changeSprite = true)
    {
        float[] smallToBig = (f == null) ? new float[] { 0.6f, 0.8f, 0.7f, 0.9f, 0.8f, 1f } : f;

        bool wasNotBig = !isBig;
        int j = powerup;
        float yPosBefore = transform.position.y;

        ChangePowerupStatus(i, changeSprite);
        LevelManager.ChangePauseState(true);
        AudioManager.PlayAudio(s);
        TimerClass timerT = new TimerClass(1);

        int k = 0;
        int l = 0;
        while (true) {
            if (timerT.UntilTime(0f)) {
                if (wasNotBig) {
                    transform.position = new Vector3(transform.position.x, yPosBefore + (smallToBig[k] - 0.5f));
                    transform.localScale = new Vector3(transform.localScale.x, smallToBig[k], transform.localScale.z);
                }
                else
                    spriteR.sprite = (l % 2 == 0) ? GetSpriteFromPowerupInt(powerup, spriteR.sprite) : GetSpriteFromPowerupInt(j, spriteR.sprite);

                k++;
                l++;
                timerT.ResetTimer(1, -0.12f);

                if (k >= smallToBig.Length) {
                    LevelManager.ChangePauseState(false);
                    anim.speed = 1;

                    yield break; 
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }
    internal void HitPlayer(GameObject GO, Actor actor)
    {
        float[] f = new float[] { 0.8f, 0.9f, 0.7f, 0.8f, 0.6f, 0.5f };

        if (!IsInvincible()) {
            if (isBig) {
                StartCoroutine(GotPowerup(powerup > 1 ? 1 : 0, actor, f, "player_hit", powerup > 1));
                StartCoroutine(InvincibleFrames(spriteR, 0f, 1f, StopInvincibleFrames(), false, 0.5f));
            }
            else {
                anim.SetBool("death", true);
                LevelManager.StatusOfScene(true);
            }
        }
    }

    private IEnumerator StopInvincibleFrames()
    {
        ResetInvincibleFrames();
        yield break;
    }


    public void ChangePowerupStatus(int i, bool changeSprite = true)
    {
        powerup = i;
        spriteR.sprite = GetSpriteFromPowerupInt(i, spriteR.sprite, true, changeSprite);
    }
    internal Sprite GetSpriteFromPowerupInt(int i, Sprite sprite, bool playAnim = false, bool changeSprite = true)
    {
        string[] animString = new string[] {
            "big", //mushroom
            "fire" //fire flower
        };
        string s = (i != 0 ? $"_{ animString[i - 1] }" : string.Empty);

        if (playAnim) anim.Play("stopped" + s);
        return changeSprite ? Resources.Load<Sprite>(GetSpritePath() + "player_mario" + s) : sprite;
    }

    public int GetPowerupInt() { return powerup; }

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

    public bool IsInvincible() { return timer.GetTime(99) > 0f; }
    public bool IsOnGround() { return ColliderCheck.CollidedWithWall(ColliderCheck.WallDirection.Ground, boxCollider, layerMask); }
    private bool IsFalling() { return !IsOnGround(); }
    private int GetIndexOfBCS() { return (IsCrouching() && !IsJumping()) ? (isBig ? 3 : 1) : (isBig ? 2 : 0); }

    private float GetJumpForce() { return 9f; }
    private string GetJumpSound() { return "small_mario_jump"; }
    internal static string GetSpritePath() { return SpritePath("Player"); }

    
    public bool IsTryingToHold() { return Input.GetKey(GetHoldingKeyCode()); }
    public static KeyCode GetHoldingKeyCode() { return KeyCode.UpArrow; }


    public static bool IsPlayer(GameObject GO) { return GO.GetComponent<Player>() != null; }

    public static bool IsPlayer(GameObject GO, out Player player) {
        player = GO.GetComponent<Player>();
        return player != null; 
    }

}
