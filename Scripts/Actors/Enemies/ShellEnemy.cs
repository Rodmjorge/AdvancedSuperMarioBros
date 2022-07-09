using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ShellEnemy : BackToLifeEnemies
{
    protected bool isInShell { get { return anim.GetBool("shell"); } }
    protected bool isShellMoving { get { return anim.GetBool("shellMoving"); } }
    protected bool isGettingUp { get { return anim.GetBool("gettingUp"); } }

    public bool startsInShell;
    public bool isRed;


    public override void DataLoaded(string s, string beforeEqual)
    {
        startsInShell = LevelLoader.CreateVariable(s, beforeEqual, "startInShell", startsInShell);
        isRed = LevelLoader.CreateVariable(s, beforeEqual, "red", isRed);

        base.DataLoaded(s, beforeEqual);
    }

    public override void Start()
    {
        anim.SetBool("shell", startsInShell);
        anim.SetBool("red", isRed);

        base.Start();
    }

    public override void DeadMaybeTick()
    {
        if (isShellMoving) {
            WalkTick();
            timer.ResetTimer(10);
        }
    }

    public override void SetBoxColliderBounds() { bcs.SetBoxColliderBoundsPos(0.35f); }
    public override bool IsHoldable() { return NotMovingInShell() && transform.localScale.x <= 1f && transform.localScale.y <= 1f && DeadMaybe(); }
    public override bool FlipXWhenChangingDirection() { return true; }
    public override void ChangeHoldingStatus(bool b) { isBeingHeld = b; }

    public override void KillEnemy()
    {
        anim.SetBool("shell", true);
        SetBoxColliderBounds();

        StartCoroutine(timer.IncreaseTime(0.08f, 15));
        CreateAreaEffector(true, LayerMaskInterface.enemyBlockT);
        ChangeTransform(true);

        base.KillEnemy();
    }

    public override void HitByShell(Enemies enemy)
    {
        anim.SetBool("shell", true);
        base.HitByShell(enemy);
    }
    public override void CancelledOut(GameObject GO, bool goLeft)
    {
        anim.SetBool("shell", true);
        base.CancelledOut(GO, goLeft);
    }

    public override void JumpThrown(Player player)
    {
        if (player.IsCrouching() && player.isHoldingSomething)
            base.JumpThrown(player); 
        else {
            anim.SetBool("shellMoving", true);
            StartCoroutine(timer.IncreaseTime(0.25f, 30));

            walkingRight = player.transform.position.x < transform.position.x;
            moveSpeed *= 4;

            PlayKickedSound();
        }
    }

    public override void PlayerCollided(Player player)
    {
        if (!player.IsTryingToHold()) {
            if (isInShell && !isShellMoving && DeadMaybe())
                JumpThrown(player);
        }
    }

    public override void PlayerCollidedAbove(Player player)
    {
        if (PlayerCollidingBoolean()) {
            if (isShellMoving) {
                anim.SetBool("shellMoving", false);

                timer.ResetTimer(15);
                timer.ResetTimer(30);

                moveSpeed /= 4;
                ChangeTransform(false);
            }

            base.PlayerCollidedAbove(player);
        }
    }

    public override void ChangedDirectionsWithActor(Actor actor)
    {
        if (isShellMoving)
            CollidedBaseWithHitBlock(actor, this, isShellMoving, scoreManager);
    }

    public override IEnumerator LifeBeingHeld()
    {
        GetOffTheFuckingShell();
        yield break;
    }
    public override IEnumerator CameBackToLife()
    {
        anim.SetBool("gettingUp", true);
        TimerClass timerT = new TimerClass(1);

        while (true) {
            if (Resume()) {
                if (!isShellMoving) {
                    if (timerT.UntilTime(0.3f)) {
                        anim.SetBool("gettingUp", false);
                        GetOffTheFuckingShell();

                        StartCoroutine(base.CameBackToLife());
                        yield break;
                    }
                } 
                else {
                    anim.SetBool("gettingUp", false);
                    yield break;
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }

    public virtual void GetOffTheFuckingShell()
    {
        anim.SetBool("shell", false);
        SetBoxColliderBounds();

        timer.ResetTimer(15);
        spriteR.flipY = false;

        CreateAreaEffector(false);
        ChangeTransform(false);
    }

    public virtual void ChangeTransform(bool shell)
    {
        Sprite sp = settings.defaultSprite;
        transform.position += shell ? -ShiftPos(sp) : ShiftPos(sp);

        Vector2 offset = new Vector2(settings.offset.x, -0.075f);
        bcs.SetBoxCollider(shell ? new Vector2(settings.size.x, 0.85f) : settings.size, shell ? (spriteR.flipY ? -offset : offset) : settings.offset);
    }

    protected override bool InvertOffsetWhenFlippedY() { return true; }
    protected override bool StopTick() { return isInShell; }
    protected override bool DeadMaybe() { return base.DeadMaybe() && timer.GetTime(15) >= 0.08f; }
    protected override bool PlayerCollidingBoolean() { return base.PlayerCollidingBoolean() || (isShellMoving && timer.GetTime(30) >= 0.25f); }

    public override bool DoTurnOnEdges() { return isRed && NotMovingInShell(); }
    public override LayerMask GetLayerMask() { return isShellMoving ? LayerMaskInterface.grounded : base.GetLayerMask(); }
    public virtual bool NotMovingInShell() { return !isShellMoving; }

    public static void CollidedBase(Actor actor, Enemies enemies, bool b, LevelManager.ScoreManager scoreManager)
    {
        if (b && !LayerMaskInterface.IsCreatedLayer(actor.gameObject.layer)) {
            if (actor.IsActor(out Enemies enemy) && !enemy.pauseActor) {
                scoreManager.AddIndex(1, true);
                enemy.HitByShell(enemies);
            }

            else if (actor.IsActor(out Coin coin)) 
                coin.CollidedWithCoin();
        }
    }
    public static void CollidedBaseWithHitBlock(Actor actor, Enemies enemies, bool b, LevelManager.ScoreManager scoreManager)
    {
        if (b && !LayerMaskInterface.IsCreatedLayer(actor.gameObject.layer)) {
            CollidedBase(actor, enemies, true, scoreManager);

            if (actor.IsActor(out HitBlock hitBlock)) 
                hitBlock.CollidedHitBlock();
        }
    }
}