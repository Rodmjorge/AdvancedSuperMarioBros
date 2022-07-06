using System;
using System.Collections;
using UnityEngine;

public class BackToLifeEnemies : WalkingEnemies
{
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
    }

    public override void HitByBlock(HitBlock hitBlock, bool hitOnLeft)
    {
        KillEnemy();
        StartCoroutine(Rotate180Degrees(hitOnLeft));
    }

    public override void PlayerCollidedAbove(Player player)
    {
        if (VeryMuchAlive()) {
            KillEnemy();
            base.PlayerCollidedAbove(player);
        }
    }
    public override void PlayerStayingCollidedBelow(Player player)
    {
        if (VeryMuchAlive()) base.PlayerStayingCollidedBelow(player);
    }


    public IEnumerator Rotate180Degrees(bool hitOnLeft)
    {
        spriteR.flipY = false;
        rigidBody.velocity = RigidVector(hitOnLeft ? -2f : 2f, 12f, true, 0.08f);

        timer.SetTimer(-1f, 10);
        while (true) {
            if (Resume()) {
                transform.eulerAngles += new Vector3(0f, 0f, -30f);

                if (transform.eulerAngles.z <= 180f) {
                    transform.eulerAngles = new Vector3(0f, 0f, 0f);
                    spriteR.flipY = true;

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
        while (true) {

            if (Resume()) {
                if (timer.UntilTime(timeUntilLifeAgain - 2f, 10)) {

                    if (timer.WhileTime(timeUntilLifeAgain, 10, false)) {
                        transform.eulerAngles = new Vector3(0f, 0f, i * 4f);

                        if (i >= 3) { descending = true; }
                        else if (i <= -3) { descending = false; }
                        i += descending ? ((i == 1) ? -2 : -1) : ((i == -1) ? 2 : 1);
                    }
                    else {
                        if (canResetTimer) transform.eulerAngles = new Vector3(0f, 0f, 0f);
                        canResetTimer = false;

                        if (isBeingHeld) {
                            ChangeHoldingStatus(false);
                            StartCoroutine(LifeBeingHeld());

                            timer.ResetTimer(10);
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
                                    spriteR.flipY = false;

                                    timer.ResetTimer(10);
                                    StartCoroutine(CameBackToLife());

                                    yield break;
                                }
                            }
                            else {
                                timer.ResetTimer(10);
                                StartCoroutine(CameBackToLife());

                                yield break;
                            }
                        }
                    }
                }
                else {
                    if (timer.UntilTime(0f, 10, false)) transform.localEulerAngles = new Vector3(0f, 0f, 0f); 
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }

    public virtual IEnumerator CameBackToLife() { yield break; }
    public virtual IEnumerator LifeBeingHeld() { yield break; }

    public virtual void ResetLifeTimer() { if (canResetTimer) timer.ResetTimer(10); }
    public virtual void KillEnemy()
    {
        rigidBody.velocity = RigidVector(0f, null);
        PlaySteppedSound();

        StartCoroutine(ShakingAnimAndLife());
    }

    protected bool VeryMuchAlive() { return !DeadMaybe(); }
    protected virtual bool StopTick() { return false; }
    protected virtual bool DeadMaybe() { return StopTick(); }
    protected bool IsFlipped() { return spriteR.flipY; }
}