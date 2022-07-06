using System.Collections;
using UnityEngine;

public abstract class Tiles : Actor
{
    public bool appearWhenActivated = true;
    public bool appearParticles = true;

    public ushort triggeredID;

    public override void DataLoaded(string s, string beforeEqual)
    {
        appearWhenActivated = LevelLoader.CreateVariable(s, beforeEqual, "appearInTrigger", appearWhenActivated);
        appearParticles = LevelLoader.CreateVariable(s, beforeEqual, "appearParticles", appearParticles);

        targetIDSet = SetTargetID(s, beforeEqual);
        triggeredID = SetTriggeredID(s, beforeEqual, triggeredID);
    }

    public override void Start()
    {
        base.Start();
        CheckTriggeredEvent(IsTargetActive(triggeredID));
    }

    public override void Tick() { CheckTriggeredEvent(false); }

    public override void PausedTick() { CheckTriggeredEvent(true); }


    public virtual void CheckTriggeredEvent(bool? b)
    {
        if (IsValidTrigger(triggeredID))
            ChangeTriggeredEvent(b);
    }
    public virtual void ChangeTriggeredEvent(bool? active)
    {
        if (active != null) {
            bool b = active.Value;
            bool ignore = !b;

            if ((appearWhenActivated ? b : !b) ? IsTargetActive(triggeredID).Value : !IsTargetActive(triggeredID).Value) {
                BooleanBoxAndRigid(b, ignore, ignore);
                BooleanAnim(b, ignore);

                spriteR.sprite = b ? settings.defaultSprite : null;
                pauseActor = !b;

                if (!b && appearParticles) {
                    if (ShowParticleWhenDestroyed()) GetParticle();
                }
            }
        }
    }


    public virtual Particle Particling(Particle.ParticleType type, int number = 4) { return Particle.CreateParticle(transform.position, type, transform.localScale, number); }
    public abstract Particle GetParticle();

    public virtual bool ShowParticleWhenDestroyed() { return true; }
    public virtual bool IsDestructable() { return true; }
}