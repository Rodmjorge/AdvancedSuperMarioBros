using System.Collections;
using UnityEngine;

public class Goomba : WalkingEnemies
{
    public Vector2 bigDestroyedVelocity = new Vector2(6f, 10f);
    public string spawnedEnemy = null;
    public byte spawningCount = 2;

    private const string deathStr = "death";
    private bool isDead;

    public override void SetBoxColliderBounds() { bcs.SetBoxColliderBoundsPos(0.3f); }
    public override void DataLoaded(string s, string beforeEqual)
    {
        bigDestroyedVelocity = LevelLoader.CreateVariable(s, beforeEqual, "bigDestroyedVelocityVector", bigDestroyedVelocity);
        spawnedEnemy = LevelLoader.CreateVariable(s, beforeEqual, "spawnedEnemy", spawnedEnemy);
        spawningCount = LevelLoader.CreateVariable(s, beforeEqual, "spawningCount", spawningCount);

        base.DataLoaded(s, beforeEqual);
    }


    public override void Tick()
    {
        if (!isDead) base.Tick();
        else rigidBody.velocity = RigidVector(0f, 0f);
    }

    public override void PlayerCollidedAbove(Player player)
    {
        base.PlayerCollidedAbove(player);
        BooleanBoxAndRigid(false);

        if (transform.localScale.y >= 2f && transform.localScale.x >= 2f) {
            float f = ((transform.localScale.x < transform.localScale.y) ? transform.localScale.x : transform.localScale.y) - 1f;
            string enemy = (spawnedEnemy == null) ? $"[id={ GetSpawningEnemy() };pos={ PositionString(LevelLoader.TransformPos.X) },{ PositionString(LevelLoader.TransformPos.Y) };size={ f },{ f }]" : spawnedEnemy;

            int j = 1;
            for (int i = 0; i < spawningCount; i++) {
                j += (i % 2 == 0 && i != 0) ? 1 : 0;

                Actor goomba = LevelLoader.CheckLineInBrackets(enemy, gameObject, true, null, ActorRegistry.ActorSettings.CreatedActorTypes.CreatedLayer);
                goomba.rigidBody.velocity = RigidVector((i % 2 == 0 ? -bigDestroyedVelocity.x : bigDestroyedVelocity.x) / j, bigDestroyedVelocity.y * (j / (j > 1 ? 2 : 1)));

                if (goomba.IsActor(out WalkingEnemies walkEnemy))
                    walkEnemy.startsGoingRight = true;
            }

            Destroy(gameObject);
        }
        else {
            PlayDeathAnim();

            isDead = true;
            StartCoroutine(timer.RunAfterTime(KillAnim(), 0.35f));
        }
    }

    public virtual void PlayDeathAnim() { anim.SetBool(deathStr, true); }
    public virtual string GetSpawningEnemy() { return "goomba"; }

    IEnumerator KillAnim()
    {
        Destroy(gameObject);
        yield break;
    }
}