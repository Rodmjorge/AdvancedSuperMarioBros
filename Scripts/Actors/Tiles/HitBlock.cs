using System.Collections;
using UnityEngine;

public class HitBlock : Tiles
{
    public float sizeIncreasingTime = 0.12f;

    protected bool doingAnim;
    private float sizeTimer;

    public override Particle GetParticle() { return null; }
    public override void SetBoxColliderBounds() { bcs.SetBoxColliderBoundsNeg(0.075f); }

    public override void DataLoaded(string s, string beforeEqual)
    {
        sizeIncreasingTime = LevelLoader.CreateVariable(s, beforeEqual, "sizeIncreasingTime", sizeIncreasingTime);
        base.DataLoaded(s, beforeEqual);
    }

    public override void PlayerCollidedBelow(Player player)
    {
        if ((player.transform.position.x <= bcs.GetExtentsXPos() - 0.1f && player.transform.position.x > bcs.GetExtentsXNeg() + 0.1f) || 
            player.transform.position.y < bcs.GetExtentsYNeg())
            CollidedHitBlock();
    }

    public virtual void CollidedHitBlock()
    {
        if (!doingAnim) {
            HasHitBlock();
            StartCoroutine(HitBlockAnimation());
        }
    }

    public virtual IEnumerator HitBlockAnimation()
    {
        Vector3 size = transform.localScale;
        float posY = transform.position.y;

        doingAnim = true;
        bool b = true;

        TimerClass timerT = new TimerClass(1);
        while (true) {

            if (Resume()) {
                float f = (sizeIncreasingTime / 0.1f);
                float g = Time.fixedDeltaTime * 15f;
                Vector2 h = size;

                Vector3 posIncrease = new Vector3(0f, ((0.2f * h.y) / f) * g, 0f);
                Vector3 sizeIncrease = IncreaseSizeInAnim() ? new Vector3(((GetSizeIncrease().x * h.x) / f) * g, ((GetSizeIncrease().y * h.y) / f) * g, 0f) : new Vector3(0f, 0f, 0f);

                if (timerT.WhileTime(sizeIncreasingTime)) {
                    transform.position += posIncrease;
                    transform.localScale += sizeIncrease;
                }
                else {
                    if (b) {
                        ReachedMaxSizeInAnim();
                        b = false;
                    }

                    transform.position -= posIncrease;
                    transform.localScale -= sizeIncrease;

                    if (timerT.UntilTime(sizeIncreasingTime * 2, 1, false)) {
                        transform.position = new Vector3(transform.position.x, posY, transform.position.z);
                        transform.localScale = size;

                        doingAnim = false;
                        FinishedAnim();

                        yield break;
                    }
                }

                sizeTimer = timerT.GetTime();
            }

            yield return new WaitForFixedUpdate();
        }
    }


    public virtual void HasHitBlock() { return; }
    public virtual void ReachedMaxSizeInAnim() { return; }
    public virtual void FinishedAnim() { return; }

    public virtual bool TheBloqHasIndeedBeenHit() { return doingAnim; }
    public virtual float TimeOfHitting() { return sizeTimer; }


    public virtual bool IncreaseSizeInAnim() { return true; }
    public virtual Vector2 GetSizeIncrease() { return new Vector2(0.075f, 0.075f); }
}