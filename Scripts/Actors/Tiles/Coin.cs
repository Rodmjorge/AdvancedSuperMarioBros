using System.Collections;
using UnityEngine;

public class Coin : Tiles
{
    protected bool containerBlockCoin;

    public override void SetBoxColliderBounds() { bcs.SetBoxColliderBoundsPos(0.3f); }
    public override void PlayerCollided(Player player)
    {
        CollidedWithCoin();
    }

    public override void StayingCollidedBelow(GameObject GO, Actor actor)
    {
        if (HitByBlockBelow(actor, out _))
            SetAsContainerCoin();

        base.StayingCollidedBelow(GO, actor);
    }

    public virtual void CollidedWithCoin()
    {
        if (!containerBlockCoin) {
            SetTargetBoolean(true);

            LevelManager.AddCoin(GetCoinInt());
            LevelManager.AddToScore(GetCoinScore());
            AudioManager.PlayAudio(GetSound());

            Destroy(gameObject);
        }
    }

    public virtual void SetAsContainerCoin()
    {
        containerBlockCoin = true;
        anim.SetBool("containerBlock", true);

        StartCoroutine(ContainerBlockCoinAnim());
        ResetInvincibleFrames();
    }
    public virtual void SetAsPhysicsCoin()
    {
        rigidBody.isKinematic = false;
        float f = LevelLoader.RandomBoolean() ? -1.5f : 1.5f;

        StartCoroutine(RigidbodyJumpsWhenGroundHit(true, true, 0.08f, RigidVector(f, 9f), RigidVector(f, 5f)));
        StartCoroutine(InvincibleFrames(spriteR, 1.5f));
    }

    protected virtual IEnumerator ContainerBlockCoinAnim()
    {
        rigidBody.isKinematic = false;
        rigidBody.velocity = RigidVector(null, 15f);

        BooleanBoxCollider(false);
        ContainerCoinCollected();

        TimerClass timerT = new TimerClass(1);
        while (true) {
            if (Resume()) {

                if (timerT.UntilTime(0.6f)) {
                    ContainerCoinDestroyed();
                    Destroy(gameObject);
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }

    public virtual void ContainerCoinCollected() {
        AudioManager.PlayAudio(GetSound());
    }
    public virtual void ContainerCoinDestroyed() {
        scoreManager.SetScore(GetCoinScore(), true, transform.position);
        LevelManager.AddCoin(GetCoinInt());
    }

    public virtual uint GetCoinInt() { return 1; }
    public virtual ulong GetCoinScore() { return 100; }
    public virtual string GetSound() { return "coin"; }

    public override bool PlayDestroySound() { return false; }
    public override Particle GetParticle() { return null; }
    public override bool IsDestructable() { return false; }
}