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
        anim.SetBool(UsedBlockAnimString(), getUsed);

        string container = (containerObject == null) ? "[id=goomba]" : containerObject;

        Actor actor = LevelLoader.CheckLineInBrackets(container, gameObject, true, null, ActorRegistry.ActorSettings.CreatedActorTypes.EnableAfterTime, 0.1f);
        actor.rigidBody.velocity = RigidVector(null, 15f);
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

    public virtual string UsedBlockAnimString() { return "used"; }
}