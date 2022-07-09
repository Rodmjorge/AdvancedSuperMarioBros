using System.Collections;
using UnityEngine;

public class Galoomba : BackToLifeEnemies
{
    private bool kicked;

    public Vector2 kickingVelocity = new Vector2(7f, 7f);

    public override void SetBoxColliderBounds() { bcs.SetBoxColliderBoundsPos(0.25f); }
    public override void DataLoaded(string s, string beforeEqual)
    {
        kickingVelocity = LevelLoader.CreateVariable(s, beforeEqual, "kickingVelocity", kickingVelocity);
        base.DataLoaded(s, beforeEqual);
    }

    public override void Framed()
    {
        if (kicked && rigidBody.velocity == Vector2.zero) {
            scoreManager.ResetIndex();
            kicked = false;
        }
    }

    public override bool IsHoldable() { return transform.localScale.x <= 1f && transform.localScale.y <= 1f && DeadMaybe(); }
    public override void ChangeHoldingStatus(bool b) { isBeingHeld = b; }

    public override void StayingCollided(GameObject GO, Actor actor)
    {
        ShellEnemy.CollidedBase(actor, this, isBeingThrown || kicked, scoreManager);
        base.StayingCollided(GO, actor);
    }

    public override void KillEnemy()
    {
        spriteR.flipY = true;

        StartCoroutine(timer.IncreaseTime(0.08f, 15));
        CreateAreaEffector(true, LayerMaskInterface.enemyBlockT);

        base.KillEnemy();
    }

    public override void Thrown(Player player, bool changeHoldingStatus = true)
    {
        base.Thrown(player, changeHoldingStatus);
        ResetLifeTimer();
    }
    public override void PlayerStayingCollided(Player player)
    {
        if (PlayerCollideUpsideDown() && timer.GetTime(20) <= 0) {
            rigidBody.velocity = RigidVector((player.transform.position.x > transform.position.x) ? -kickingVelocity.x : kickingVelocity.x, kickingVelocity.y, true, 0.05f);
            StartCoroutine(timer.ResetTimerAfterTime(0.5f, 20));

            PlayKickedSound();
            ResetLifeTimer();
            kicked = true;
        }
    }

    public override IEnumerator CameBackToLife()
    {
        spriteR.flipY = false;

        timer.ResetTimer(15);
        CreateAreaEffector(false);

        return base.CameBackToLife();
    }

    protected override bool StopTick() { return dead; }
    protected override bool DeadMaybe() { return base.DeadMaybe() && timer.GetTime(15) >= 0.08f; }
    private bool PlayerCollideUpsideDown() { return DeadMaybe() && !isBeingHeld && !isBeingThrown; }
}