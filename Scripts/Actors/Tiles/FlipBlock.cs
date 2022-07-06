using System.Collections;
using UnityEngine;

public class FlipBlock : ContainerBlock
{
    private bool isFlipping { get { return anim.GetBool("flipping"); } }
    public float flippingDuration = 5f;

    public override Particle.ParticleType GetBlockParticleType() { return base.GetBlockParticleType(); }
    public override void DataLoaded(string s, string beforeEqual)
    {
        flippingDuration = LevelLoader.CreateVariable(s, beforeEqual, "flippingDuration", flippingDuration);
        base.DataLoaded(s, beforeEqual);
    }

    public override void Tick()
    {
        BooleanBoxCollider(!isFlipping);
    }

    public override void WhenContainerIsNull() { StartCoroutine(FlippingAnim(flippingDuration)); }
    public override string GetDefaultContainer() { return "null"; }
    public override bool UsedBoolean() { return containerObject == "null" ? false : base.UsedBoolean(); }

    private IEnumerator FlippingAnim(float time)
    {
        anim.SetBool("flipping", true);
        TimerClass timerT = new TimerClass(1);

        while (true) {
            if (Resume()) {
                if (timerT.UntilTime(time)) {
                    anim.SetBool("flipping", false);
                    yield break;
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }
}