using System.Collections;
using UnityEngine;

public class TriggerRegistry : ActorRegistry
{
    internal override void Awake()
    {
        RegisterActor("trigger_or", new ActorSettings() { actorClass = new TriggerOr() });
        RegisterActor("trigger_chain", new ActorSettings() { actorClass = new TriggerChain() });

        base.Awake();
    }

    internal override string GetSpritePath() { return string.Empty; }
    internal override string GetAnimatorPath() { return string.Empty; }
}