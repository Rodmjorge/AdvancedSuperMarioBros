using System.Collections;
using UnityEngine;

public class Particle : Tiles
{
    public override Particle GetParticle() { return this; }

    public IEnumerator Rotate(bool clockwise)
    {
        while (true) {
            if (Resume())
                transform.localEulerAngles += new Vector3(0f, 0f, clockwise ? 12f : -12f);

            yield return new WaitForFixedUpdate();
        }
    }

    public static Particle CreateParticle(Vector3 pos, ParticleType type, Vector3? size = null, int number = 4)
    {
        Particle particle = null;

        int j = 1;
        for (int i = 0; i < number; i++) {
            j += (i % 2 == 0 && i != 0) ? 1 : 0;
            particle = ActorRegistry.SetActor("particle", pos, size).GetComponent<Particle>();

            particle.rigidBody.velocity = particle.RigidVector((i % 2 == 0) ? -4f : 4f, 7f * j);
            particle.anim.Play(TypeToString(type) + "_particle");

            particle.StartCoroutine(particle.Rotate(i % 2 != 0));
        }

        return particle;
    }

    private static string TypeToString(ParticleType type)
    {
        switch (type) {
            default:
            case ParticleType.QuestionBlock:
            case ParticleType.FlipBlock:
                return "question_block";

            case ParticleType.BrickBlock:
                return "brick_block";

            case ParticleType.FrozenBrickBlock:
                return "frozen_block";

            case ParticleType.IceBlock:
                return "ice_block";

            case ParticleType.ThreeUpMoonBlock:
                return "3up_moon_block";
        }
    }

    public enum ParticleType
    {
        QuestionBlock,
        BrickBlock,
        FlipBlock,
        FrozenBrickBlock,
        IceBlock,
        ThreeUpMoonBlock
    }
}