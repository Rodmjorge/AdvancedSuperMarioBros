using System.Collections;
using UnityEngine;

public class WalkingActors : Actor
{
    public float moveSpeed = 0f;
    public bool startsGoingRight;

    protected bool walkingRight;
    private bool reachedGround;

    public override void Start()
    {
        if (startsGoingRight) {
            walkingRight = startsGoingRight;
            ChangedDirections();
        }

        moveSpeed = (moveSpeed == 0f) ? DefaultMoveSpeed() : moveSpeed;
        anim.speed = moveSpeed / 2f;

        base.Start();
    }

    public override void DataLoaded(string s, string beforeEqual)
    {
        moveSpeed = LevelLoader.CreateVariable(s, beforeEqual, "moveSpeed", moveSpeed);
        startsGoingRight = LevelLoader.CreateVariable(s, beforeEqual, "startRight", startsGoingRight);
    }

    public override void Tick()
    {
        if (UsesBasicMovement()) {
            if (reachedGround) {
                rigidBody.velocity = RigidVector(IsWalkingRight() ? moveSpeed : -moveSpeed, null);

                if (ColliderCheck.GetActorCollided(GetWallDirection(), boxCollider, GetLayerMask(), out Actor[] actorArr)) {
                    walkingRight = !walkingRight;
                    ChangedDirections();

                    foreach (Actor actor in actorArr)
                        ChangedDirectionsWithActor(actor);
                }

                if (DoTurnOnEdges()) {
                    if (!ColliderCheck.CollidedWithWall(ColliderCheck.WallDirection.Ground, boxCollider, GetLayerMask(), IsWalkingRight() ? ColliderCheck.RaycastThird.Right : ColliderCheck.RaycastThird.Left, GetCloserEdgeFloat())
                      && ColliderCheck.CollidedWithWall(ColliderCheck.WallDirection.Ground, boxCollider, GetLayerMask(), ColliderCheck.RaycastThird.All, GetCloserEdgeFloat())) {
                        walkingRight = !walkingRight;
                        ChangedDirectionsOnEdges();
                    }
                }
            }
            else
                reachedGround = ColliderCheck.CollidedWithWall(ColliderCheck.WallDirection.Ground, boxCollider, LayerMaskInterface.grounded, ColliderCheck.RaycastThird.All, 1, true);
        }
    }


    public virtual void ChangedDirections()
    {
        if (FlipXWhenChangingDirection())
            spriteR.flipX = !spriteR.flipX;
    }
    public virtual void ChangedDirectionsOnEdges() { ChangedDirections(); }

    public virtual void ChangedDirectionsWithActor(Actor actor) { return; }

    public virtual bool UsesBasicMovement() { return true; }
    public virtual float DefaultMoveSpeed() { return 2f; }
    public virtual bool FlipXWhenChangingDirection() { return false; }
    public virtual bool DoTurnOnEdges() { return false; }
    public virtual bool IsWalkingRight() { return walkingRight; }
    public virtual ColliderCheck.WallDirection GetWallDirection() { return IsWalkingRight() ? ColliderCheck.WallDirection.RightWall : ColliderCheck.WallDirection.LeftWall; }

    public virtual float GetCloserEdgeFloat() { return 2f; }
    public virtual LayerMask GetLayerMask() { return LayerMaskInterface.grounded; }
}