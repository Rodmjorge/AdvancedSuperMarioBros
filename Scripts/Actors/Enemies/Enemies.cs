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

    public virtual void Disintegrated(GameObject GO)
    {
        BooleanBoxCollider(false);
        pauseActor = true;

        StartCoroutine(DisintegratedAnim(GO.transform.position.x > transform.position.x));
    }

    private IEnumerator DisintegratedAnim(bool goLeft)
    {
        rigidBody.velocity = RigidVector(null, 10f);

        while (true) {
            if (ResumeGaming()) {
                rigidBody.velocity = RigidVector(goLeft ? -3f : 3f, null);
                transform.eulerAngles += new Vector3(0f, 0f, goLeft ? -5f : 5f);
            }

            yield return new WaitForFixedUpdate();
        }
    }

    public virtual bool UseTargetID() { return true; }

    public override void StayingCollidedBelow(GameObject GO, Actor actor)
    {
        if (actor.IsActor(out HitBlock hitBlock)) {
            if (hitBlock.TheBloqHasIndeedBeenHit() && hitBlock.TimeOfHitting() < 0.1f)
                Disintegrated(hitBlock.gameObject);
        }

        base.CollidedBelow(GO, actor);
    }

    public override void PlayerCollidedAbove(Player player)
    {
        player.AddJump();
    }
    public override void PlayerCollidedBelow(Player player)
    {
        player.HitPlayer(this.gameObject, this);
    }
}