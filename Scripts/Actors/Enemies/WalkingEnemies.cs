using System.Collections;
using UnityEngine;

public class WalkingEnemies : Enemies
{
    public float moveSpeed = 2f;
    public bool startsGoingRight;

    protected bool walkingRight;
    private bool reachedGround;

    public override void Start()
    {
        if (startsGoingRight) {
            walkingRight = startsGoingRight;
            ChangedDirections(this);
        }

        anim.speed = moveSpeed / 2f;

        base.Start();
    }

    public override void DataLoaded(string s, string beforeEqual)
    {
        moveSpeed = LevelLoader.CreateVariable(s, beforeEqual, "moveSpeed", moveSpeed);
        startsGoingRight = LevelLoader.CreateVariable(s, beforeEqual, "startRight", startsGoingRight);

        base.DataLoaded(s, beforeEqual);
    }

    public override void Tick()
    {
        if (reachedGround) {
            rigidBody.velocity = RigidVector(IsWalkingRight() ? moveSpeed : -moveSpeed, null);

            if (ColliderCheck.GetActorCollided(GetWallDirection(), boxCollider, GetLayerMask(), out Actor[] actorArr)) {
                walkingRight = !walkingRight;

                foreach (Actor actor in actorArr)
                    ChangedDirections(actor);
            }

            if (DoTurnOnEdges()) {
                if (!ColliderCheck.CollidedWithWall(ColliderCheck.WallDirection.Ground, boxCollider, GetLayerMask(), IsWalkingRight() ? ColliderCheck.RaycastThird.Right : ColliderCheck.RaycastThird.Left, GetCloserEdgeFloat())
                  && ColliderCheck.GetActorCollided(ColliderCheck.WallDirection.Ground, boxCollider, GetLayerMask(), out Actor[] actorArr0, ColliderCheck.RaycastThird.All, GetCloserEdgeFloat())) {
                    walkingRight = !walkingRight;

                    foreach (Actor actor in actorArr0)
                        ChangedDirectionsOnEdges(actor);
                }
            }
        }
        else
            reachedGround = ColliderCheck.CollidedWithWall(ColliderCheck.WallDirection.Ground, boxCollider, LayerMaskInterface.grounded, ColliderCheck.RaycastThird.All, 1, true);
    }


    public virtual void ChangedDirections(Actor actor) 
    {
        if (FlipXWhenChangingDirection())
            spriteR.flipX = !spriteR.flipX;
    }
    public virtual void ChangedDirectionsOnEdges(Actor actor) { ChangedDirections(actor); }

    public virtual bool FlipXWhenChangingDirection() { return false; }
    public virtual bool DoTurnOnEdges() { return false; }
    public virtual bool IsWalkingRight() { return walkingRight; }
    public virtual ColliderCheck.WallDirection GetWallDirection() { return IsWalkingRight() ? ColliderCheck.WallDirection.RightWall : ColliderCheck.WallDirection.LeftWall; }

    public virtual float GetCloserEdgeFloat() { return 2f; }
}