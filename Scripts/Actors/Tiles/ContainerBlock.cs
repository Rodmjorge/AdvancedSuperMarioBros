using System.Collections;
using UnityEngine;

public class ContainerBlock : HitBlock
{
    protected bool isUsedBlock { get { return anim.GetBool(UsedBlockAnimString()); } }

    public string containerObject = null;
    public ushort timesCanUse = 1;
    public float timeUntilUsedBlock = 0f;

    private ushort usedTimes;
    private bool getUsed;

    public override Particle GetParticle() { return Particling(Particle.ParticleType.QuestionBlock); }
    public override void DataLoaded(string s, string beforeEqual)
    {
        containerObject = LevelLoader.CreateVariable(s, beforeEqual, "containing", containerObject);
        timesCanUse = LevelLoader.CreateVariable(s, beforeEqual, "uses", timesCanUse);
        timeUntilUsedBlock = LevelLoader.CreateVariable(s, beforeEqual, "timeForUses", timeUntilUsedBlock);

        base.DataLoaded(s, beforeEqual);
    }

    public override void CollidedHitBlock()
    {
        if (!isUsedBlock) {
            if (usedTimes <= 0) 
                StartCoroutine(UsedTimeCounter());
            
            usedTimes++;
            base.CollidedHitBlock();
        }
    }

    public override void FinishedAnim()
    {
        containerObject = (containerObject == null) ? GetDefaultContainer() : containerObject;
        const float time = 0.15f;

        if (containerObject != "null") {
            Actor actor = LevelLoader.CheckLineInBrackets(containerObject, gameObject, true, null, ActorRegistry.ActorSettings.CreatedActorTypes.EnableAfterTime, time);

            actor.transform.position = new Vector3(actor.transform.position.x, bcs.GetExtentsYPos() - 0.5f, 0f);
            actor.rigidBody.velocity = RigidVector(null, 12f);

            if (actor.gameObject.transform.localScale.x > 1f || actor.gameObject.transform.localScale.y > 1f)
                StartCoroutine(SizeIncreaseOfActor(actor.gameObject.transform, time, 0.05f));
        }
        else
            WhenContainerIsNull();

        anim.SetBool(UsedBlockAnimString(), UsedBoolean());
    }

    public virtual bool UsedBoolean() { return getUsed; }

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

        while (true) {
            if (Resume()) {
                if ((timerT.UntilTime(timeUntilUsedBlock) && timeUntilUsedBlock != 0f) || (usedTimes >= timesCanUse)) {
                    getUsed = true;
                    yield break;
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }

    public virtual void WhenContainerIsNull() { return; }

    public virtual string UsedBlockAnimString() { return "used"; }
    public virtual string GetDefaultContainer() { return "null"; }
}