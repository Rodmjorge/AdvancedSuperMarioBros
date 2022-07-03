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

    public virtual IEnumerator Disintegrated(GameObject GO)
    {
        pauseActor = true;

        BooleanBoxCollider(false);
        rigidBody.velocity = RigidVector(null, 10f);

        bool goLeft = GO.transform.position.x > transform.position.x;
        while (true) {
            if (ResumeGaming()) {
                rigidBody.velocity = RigidVector(goLeft ? -5f : 5f, null);
                transform.localEulerAngles += new Vector3(0f, 0f, goLeft ? -15f : 15f);
            }

            yield return new WaitForFixedUpdate();
        }
    }

    public virtual bool UseTargetID() { return true; }

    public override void StayingCollidedBelow(GameObject GO, Actor actor)
    {
        if (actor.IsActor(out HitBlock hitBlock)) {
            if (hitBlock.TheBloqHasIndeedBeenHit() && hitBlock.TimeOfHitting() < 0.1f)
                StartCoroutine(Disintegrated(hitBlock.gameObject));
        }
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