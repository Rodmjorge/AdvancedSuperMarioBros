using System.Collections;
using UnityEngine;

public class WalkingEnemies : Enemies
{
    public float moveSpeed = 2f;
    public bool startsGoingRight;

    private bool walkingRight;
    private bool reachedGround;

    public override void Start()
    {
        walkingRight = startsGoingRight;
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

            if (ColliderCheck.CollidedWithWall(IsWalkingRight() ? ColliderCheck.WallDirection.RightWall : ColliderCheck.WallDirection.LeftWall, boxCollider, layerMask)) {
                walkingRight = !walkingRight;
                ChangedDirections();
            }

            if (DoTurnOnEdges()) {
                if (!ColliderCheck.CollidedWithWall(ColliderCheck.WallDirection.Ground, boxCollider, layerMask, IsWalkingRight() ? ColliderCheck.RaycastThird.Right : ColliderCheck.RaycastThird.Left, GetCloserEdgeFloat())
                  && ColliderCheck.CollidedWithWall(ColliderCheck.WallDirection.Ground, boxCollider, layerMask, ColliderCheck.RaycastThird.All, GetCloserEdgeFloat())) {
                    walkingRight = !walkingRight;
                    ChangedDirectionsOnEdges();
                }
            }
        }
        else
            reachedGround = ColliderCheck.CollidedWithWall(ColliderCheck.WallDirection.Ground, boxCollider, LayerMaskInterface.grounded);
    }


    public virtual void ChangedDirections() { return; }
    public virtual void ChangedDirectionsOnEdges() { return; }

    public virtual bool DoTurnOnEdges() { return false; }
    public virtual bool IsWalkingRight() { return walkingRight; }

    public virtual float GetCloserEdgeFloat() { return 2f; }
}