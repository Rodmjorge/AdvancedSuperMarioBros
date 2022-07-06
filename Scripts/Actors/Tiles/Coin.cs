using System.Collections;
using UnityEngine;

public class Coin : Tiles
{
    protected bool containerBlockCoin;

    public override void PlayerCollided(Player player)
    {
        CollidedWithCoin();
    }
    public override void CollidedBelow(GameObject GO, Actor actor)
    {
        if (HitByBlockBelow(actor, out _))
            SetAsContainerBlock();

        base.CollidedBelow(GO, actor);
    }

    public virtual void CollidedWithCoin()
    {
        if (!containerBlockCoin) {
            SetTargetBoolean(true);
            AudioManager.PlayAudio("coin");

            Destroy(gameObject);
        }
    }

    public virtual void SetAsContainerBlock()
    {
        containerBlockCoin = true;
        anim.SetBool("containerBlock", true);

        StartCoroutine(ContainerBlockCoinAnim());
    }

    protected virtual IEnumerator ContainerBlockCoinAnim()
    {
        rigidBody.velocity = RigidVector(null, 11f);

        while (true) {
            if (Resume()) {
                yield break;
            }

            yield return new WaitForFixedUpdate();
        }
    }

    public override bool PlayDestroySound() { return false; }
    public override Particle GetParticle() { return null; }
    public override bool IsDestructable() { return false; }
}