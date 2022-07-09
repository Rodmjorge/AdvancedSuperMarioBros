using System;
using System.Collections;
using UnityEngine;

public class BackToLifeEnemies : WalkingEnemies
{
    protected bool dead;
    protected bool shakingAlready;

    private bool canResetTimer = true;
    public float timeUntilLifeAgain = 9f;

    public override void DataLoaded(string s, string beforeEqual)
    {
        timeUntilLifeAgain = LevelLoader.CreateVariable(s, beforeEqual, "timeUntilLife", timeUntilLifeAgain);
        base.DataLoaded(s, beforeEqual);
    }

    public override void Tick()
    {
        if (!StopTick()) base.Tick();
        else DeadMaybeTick();
    }
    public virtual void DeadMaybeTick() { return; }
    protected void WalkTick() { base.Tick(); }

    public override void HitByBlock(HitBlock hitBlock, bool hitOnLeft)
    {
        if (!dead)
            KillEnemy();

        if (FlipYWhenHitByBlockBoolean())
            StartCoroutine(Rotate180Degrees(hitOnLeft));
    }

    public override void PlayerCollidedAbove(Player player)
    {
        if (PlayerCollidingBoolean()) {
            KillEnemy();
            base.PlayerCollidedAbove(player);
        }
    }
    public override void PlayerStayingCollidedBelow(Player player)
    {
        if (PlayerCollidingBoolean()) base.PlayerStayingCollidedBelow(player);
    }


    public IEnumerator Rotate180Degrees(bool hitOnLeft)
    {
        pauseActor = true;
        spriteR.flipY = false;

        rigidBody.velocity = RigidVector(hitOnLeft ? -2f : 2f, 12f, true, 0.08f);

        timer.SetTimer(-1f, 10);
        while (true) {
            if (ResumeGaming()) {
                transform.eulerAngles += new Vector3(0f, 0f, -30f);

                if (transform.eulerAngles.z <= 180f) {
                    transform.eulerAngles = new Vector3(0f, 0f, 0f);

                    pauseActor = false;
                    FlipY(true);

                    yield break;
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }
    public IEnumerator ShakingAnimAndLife()
    {
        int i = 1;
        bool descending = true;
        bool b = false;

        canResetTimer = true;
        shakingAlready = true;
        while (true) {

            if (Resume()) {
                if (timer.UntilTime(timeUntilLifeAgain - 2f, 10)) {

                    if (timer.WhileTime(timeUntilLifeAgain, 10, false)) {
                        transform.eulerAngles = new Vector3(0f, 0f, i * 3f);
                        rigidBody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

                        if (i >= 3) { descending = true; }
                        else if (i <= -3) { descending = false; }
                        i += descending ? ((i == 1) ? -2 : -1) : ((i == -1) ? 2 : 1);
                    }
                    else {
                        if (canResetTimer) transform.eulerAngles = new Vector3(0f, 0f, 0f);
                        canResetTimer = false;

                        rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;

                        if (isBeingHeld) {
                            ChangeHoldingStatus(false);
                            StartCoroutine(LifeBeingHeld());

                            timer.ResetTimer(10);
                            shakingAlready = false;

                            yield break;
                        }
                        else {

                            if (IsFlipped()) {
                                if (!b) {
                                    rigidBody.velocity = RigidVector(null, 5f);
                                    b = true;
                                }

                                transform.eulerAngles += new Vector3(0f, 0f, 30f);
                                if (transform.eulerAngles.z >= 180f) {
                                    transform.eulerAngles = new Vector3(0f, 0f, 0f);
                                    FlipY(false);

                                    timer.ResetTimer(10);
                                    StartCoroutine(CameBackToLife());

                                    shakingAlready = false;
                                    yield break;
                                }
                            }
                            else {
                                timer.ResetTimer(10);
                                StartCoroutine(CameBackToLife());

                                shakingAlready = false;
                                yield break;
                            }
                        }
                    }
                }
                else {
                    if (timer.UntilTime(0f, 10, false)) {
                        transform.localEulerAngles = new Vector3(0f, 0f, 0f);
                        rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
                    }
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }

    public virtual IEnumerator CameBackToLife() 
    {
        dead = false;
        yield break; 
    }
    public virtual IEnumerator LifeBeingHeld() 
    {
        StartCoroutine(CameBackToLife());
        yield break; 
    }

    public virtual void ResetLifeTimer() { if (canResetTimer) timer.ResetTimer(10); }
    public virtual void KillEnemy()
    {
        dead = true;
        rigidBody.velocity = RigidVector(0f, null);

        PlaySteppedSound();
        if (!shakingAlready) StartCoroutine(ShakingAnimAndLife());
    }

    private void FlipY(bool b)
    {
        spriteR.flipY = b;
        if (InvertOffsetWhenFlippedY()) bcs.SetBoxCollider(null, -boxCollider.offset);
    }
    protected virtual bool InvertOffsetWhenFlippedY() { return false; }

    protected bool VeryMuchAlive() { return !DeadMaybe(); }
    protected virtual bool StopTick() { return false; }
    protected virtual bool DeadMaybe() { return StopTick(); }
    protected virtual bool PlayerCollidingBoolean() { return VeryMuchAlive(); }
    protected virtual bool FlipYWhenHitByBlockBoolean() { return !pauseActor && !spriteR.flipY; }
    protected bool IsFlipped() { return spriteR.flipY; }
}