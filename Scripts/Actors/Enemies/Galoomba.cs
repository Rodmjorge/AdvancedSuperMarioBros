using System.Collections;
using UnityEngine;

public class Galoomba : WalkingEnemies
{
    private bool isOverRotated { get { return spriteR.flipY && timer.GetTime(15) >= 0.08f; } }
    public Vector2 kickingVelocity = new Vector2(7f, 7f);

    public override void SetBoxColliderBounds() { bcs.SetBoxColliderBoundsPos(0.3f); }
    public override void DataLoaded(string s, string beforeEqual)
    {
        kickingVelocity = LevelLoader.CreateVariable(s, beforeEqual, "kickingVelocity", kickingVelocity);
        base.DataLoaded(s, beforeEqual);
    }

    public override bool IsHoldable() { return transform.localScale.x <= 1f && transform.localScale.y <= 1f && isOverRotated; }
    public override void ChangeHoldingStatus(bool b) { isBeingHeld = b; }

    public override void Collided(GameObject GO, Actor actor)
    {
        if (actor.IsActor(out Enemies enemy) && isBeingThrown) 
            enemy.HitByShell(this);

        base.Collided(GO, actor);
    }

    public override void Tick()
    {
        if(UpOnFeet()) base.Tick();
    }

    public override void PlayerCollidedAbove(Player player)
    {
        if (UpOnFeet()) base.PlayerCollidedAbove(player);
        rigidBody.velocity = RigidVector(0f, null);

        spriteR.flipY = true;
        StartCoroutine(timer.IncreaseTime(0.08f, 15));

        CreateAreaEffector(true, LayerMaskInterface.enemyBlock);
    }

    public override void PlayerCollidedBelow(Player player)
    {
        if (UpOnFeet()) base.PlayerCollidedBelow(player);
    }

    public override void PlayerCollided(Player player)
    {
        if (PlayerCollideUpsideDown()) 
            rigidBody.velocity = RigidVector((player.transform.position.x > transform.position.x) ? -kickingVelocity.x : kickingVelocity.x, kickingVelocity.y, true, 0.05f);
    }

    private bool UpOnFeet() { return !spriteR.flipY; }
    private bool PlayerCollideUpsideDown() { return isOverRotated && !isBeingHeld && !isBeingThrown; }
}