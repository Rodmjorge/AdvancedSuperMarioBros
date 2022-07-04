using System.Collections;
using UnityEngine;

public class Enemies : Actor
{
    public ushort targetID;
    public LayerMask layerMask { get { return LayerMaskInterface.grounded + LayerMaskInterface.enemy; } }

    public override void DataLoaded(string s, string beforeEqual) 
    {
        targetID = LevelLoader.CreateVariable(s, beforeEqual, "targetId", targetID);
    }

    public override bool CancelsOutWhenHolding() { return true; }
    public override bool DiesWhenCancelledOut() { return true; }

    public virtual void HitByBlock(HitBlock hitBlock, bool hitOnLeft) { StartCoroutine(Disintegrated(hitBlock.gameObject, hitOnLeft)); }
    public virtual void HitByShell(Enemies enemy) { StartCoroutine(Disintegrated(enemy.gameObject, new System.Random().NextDouble() > 0.5f)); }
    public virtual void CancelledOut(GameObject GO, bool goLeft) { StartCoroutine(Disintegrated(GO, goLeft)); }

    public virtual IEnumerator Disintegrated(GameObject GO, bool goLeft)
    {
        pauseActor = true;

        BooleanBoxCollider(false);
        rigidBody.velocity = RigidVector(null, 10f);

        while (true) {
            if (ResumeGaming()) {
                rigidBody.velocity = RigidVector(goLeft ? -5f : 5f, null);
                transform.localEulerAngles += new Vector3(0f, 0f, goLeft ? -15f : 15f);
            }

            yield return new WaitForFixedUpdate();
        }
    }

    public virtual bool UseTargetID() { return true; }


    public override void Collided(GameObject GO, Actor actor)
    {
        if (actor.IsActor(out Enemies enemy) && !LayerMaskInterface.IsCreatedLayer(GO.layer)) {
            if (enemy.DiesWhenCancelledOut() && this.CancelsOutWhenHolding() && this.isBeingHeld) {
                enemy.CancelledOut(gameObject, false);
                this.CancelledOut(GO, true);
            }
        }

        base.Collided(GO, actor);
    }

    public override void StayingCollidedBelow(GameObject GO, Actor actor)
    {
        if (actor.IsActor(out HitBlock hitBlock)) {
            if (hitBlock.TheBloqHasIndeedBeenHit() && hitBlock.TimeOfHitting() <= 0.034f)
                HitByBlock(hitBlock, hitBlock.transform.position.x > transform.position.x);
        }

        base.StayingCollidedBelow(GO, actor);
    }

    public override void PlayerCollidedAbove(Player player)
    {
        player.AddJump();
    }
    public override void PlayerStayingCollidedBelow(Player player)
    {
        player.HitPlayer(this.gameObject, this);
    }
}