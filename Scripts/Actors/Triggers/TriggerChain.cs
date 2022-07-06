using System.Collections;
using UnityEngine;

public class TriggerChain : TriggeringBase
{
    public ushort triggeringID;
    public ushort[] targetIDs = new ushort[1];

    public float delay = 0f;

    public override void DataLoaded(string s, string beforeEqual)
    {
        triggeringID = SetTriggeredID(s, beforeEqual, triggeringID);
        targetIDs = LevelLoader.CreateVariable(s, beforeEqual, "targetIds", targetIDs);
        delay = LevelLoader.CreateVariable(s, beforeEqual, "delay", delay);

        base.DataLoaded(s, beforeEqual);
    }

    private IEnumerator Chain()
    {
        SetTargetIndex(0, activate);
        int i = 1;

        TimerClass timerT = new TimerClass(1);
        while (true) {
            if (Resume()) {

                if (timerT.UntilTime(delay)) {
                    SetTargetIndex(i, activate);
                    i++;

                    if (i >= targetIDs.Length)
                        yield break;

                    timerT.ResetTimer();
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }

    public override void TriggerTick() 
    {
        if (IsTargetActive(triggeringID) == true)
            StartCoroutine(Chain());
    }
    public override void ActivatedTick() { return; }

    private void SetTargetIndex(int i, bool activate)
    {
        targetIDBool = activate;
        LevelLoader.LevelSettings.SetTrigger((i < targetIDs.Length) ? targetIDs[i] : targetIDs[i - 1], activate);
    }

    protected override ushort GetTargetID() { return targetIDs[0]; }
}