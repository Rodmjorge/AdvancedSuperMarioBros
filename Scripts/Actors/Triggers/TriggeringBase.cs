using System.Collections;
using UnityEngine;

public abstract class TriggeringBase : Actor
{
    public bool activate = true;

    public override void DataLoaded(string s, string beforeEqual)
    {
        targetIDSet = SetTargetID(s, beforeEqual);
        activate = LevelLoader.CreateVariable(s, beforeEqual, "activate", activate);
    }

    public override void Start()
    {
        boxCollider.isTrigger = true;
        spriteR.sprite = null;
        rigidBody.isKinematic = true;

        base.Start();
    }

    public override void Tick()
    {
        if (IsTargetActive(GetTargetID()) == false)
            TriggerTick();
        else
            ActivatedTick();
    }

    public abstract void TriggerTick();
    public abstract void ActivatedTick();

    protected virtual ushort GetTargetID() { return targetIDSet; }
}