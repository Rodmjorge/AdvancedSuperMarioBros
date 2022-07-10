using System.Collections;
using UnityEngine;

public class ContainerBlock : HitBlock
{
    protected bool isUsedBlock { get { return anim.GetBool(UsedBlockAnimString()); } }
    protected Actor containerActor = null;

    public string containerObject = null;
    public ushort timesCanUse = 1;
    public float timeUntilUsedBlock = 0f;
    public bool progressive = true;

    private ushort usedTimes;
    private bool getUsed;

    public override Particle GetParticle() { return Particling(isUsedBlock ? Particle.ParticleType.UsedBlock : GetBlockParticleType()); }
    public virtual Particle.ParticleType GetBlockParticleType() { return Particle.ParticleType.QuestionBlock; }

    public override void DataLoaded(string s, string beforeEqual)
    {
        containerObject = LevelLoader.CreateVariable(s, beforeEqual, "containing", containerObject);
        timesCanUse = LevelLoader.CreateVariable(s, beforeEqual, "uses", timesCanUse);
        timeUntilUsedBlock = LevelLoader.CreateVariable(s, beforeEqual, "timeForUses", timeUntilUsedBlock);
        progressive = LevelLoader.CreateVariable(s, beforeEqual, "progressive", progressive);

        base.DataLoaded(s, beforeEqual);
    }

    public override void CollidedHitBlock(Player player)
    {
        if (!isUsedBlock) {
            if (usedTimes <= 0) 
                StartCoroutine(UsedTimeCounter());
            
            usedTimes++;
            base.CollidedHitBlock(player);
        }
    }

    public override void HasHitBlock()
    {
        containerObject = (containerObject == null) ? GetDefaultContainer() : containerObject;

        if (containerObject != "null") {
            Actor actor = LevelLoader.CheckLineInBrackets(containerObject, gameObject, true, null);
            actor.transform.position = new Vector3(actor.transform.position.x, bcs.GetExtentsYPos());

            if (actor.IsActor(out Coin coin))
                coin.SetAsContainerCoin();
            else
                Destroy(actor.gameObject);

            containerActor = actor;
        }
    }

    public override void FinishedAnim(Player player)
    {
        const float time = 0.15f;

        if (containerActor == null) {
            if (containerObject != "null") {
                Actor actor = LevelLoader.CheckLineInBrackets(containerObject, gameObject, true, null, ActorRegistry.ActorSettings.CreatedActorTypes.EnableAfterTime, time);
                actor.transform.position = new Vector3(actor.transform.position.x, bcs.GetExtentsYPos() - 0.5f);

                if (actor.IsActor(out PowerUp powerUp)) {
                    if (player != null) {
                        if (progressive && powerUp.GetPowerUpInt() > 1 && player.GetPowerupInt() <= 0) {
                            Destroy(actor.gameObject);

                            Actor actor0 = LevelLoader.CheckLineInBrackets("[id=mushroom]", gameObject, true, null, ActorRegistry.ActorSettings.CreatedActorTypes.EnableAfterTime, time);
                            actor0.transform.position = new Vector3(actor0.transform.position.x, bcs.GetExtentsYPos() - 0.5f);

                            powerUp = actor0.GetComponent<PowerUp>();
                        }
                    }

                    StartCoroutine(PowerUpOffAnim(powerUp));
                }
                else actor.rigidBody.velocity = RigidVector(null, 12f);

                if (actor.gameObject.transform.localScale.x > 1f || actor.gameObject.transform.localScale.y > 1f)
                    StartCoroutine(SizeIncreaseOfActor(actor.gameObject.transform, time, 0.05f));

                AudioManager.PlayAudio("container_block");
            }
            else
                WhenContainerIsNull();
        }

        containerActor = null;
        anim.SetBool(UsedBlockAnimString(), UsedBoolean());
    }

    public virtual bool UsedBoolean() { return getUsed; }

    private IEnumerator PowerUpOffAnim(PowerUp powerup)
    {
        float yPosBefore = powerup.transform.position.y;

        while (true) {
            powerup.BooleanBoxAndRigid(false);

            if (Resume()) {
                powerup.transform.position += new Vector3(0f, 0.025f, 0f);
                
                if (powerup.transform.position.y >= yPosBefore + 1f) {
                    powerup.BooleanBoxAndRigid(true);
                    yield break;
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator SizeIncreaseOfActor(Transform transF, float untilTime, float timeTakeToIncrease)
    {
        Vector3 expectedSize = transF.localScale;

        transF.localScale = Vector3.one;
        TimerClass timerT = new TimerClass(1);

        while (true) {
            if (Resume()) {

                if (timerT.UntilTime(untilTime)) {
                    transF.localScale += new Vector3((expectedSize.x / (timeTakeToIncrease / 0.05f)) * Time.fixedDeltaTime * 7f, (expectedSize.y / (timeTakeToIncrease / 0.05f)) * Time.fixedDeltaTime * 7f, 0f);

                    if (transF.localScale.x >= expectedSize.x && transF.localScale.y >= expectedSize.y) {
                        transF.localScale = expectedSize;
                        yield break;
                    }
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }

    public virtual IEnumerator UsedTimeCounter()
    {
        TimerClass timerT = new TimerClass(1);

        if (timeUntilUsedBlock == 0f)
            timeUntilUsedBlock = timesCanUse / 2.2f;

        while (true) {
            if (Resume()) {
                if (timerT.UntilTime(timeUntilUsedBlock) || (usedTimes >= timesCanUse)) {
                    getUsed = true;
                    yield break;
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }

    public virtual void WhenContainerIsNull() { return; }

    public virtual string UsedBlockAnimString() { return "used"; }
    public virtual string GetDefaultContainer() { return "[id=coin]"; }
}