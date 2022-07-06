using System;
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

    public static Particle CreateParticle(Vector3 pos, ParticleType type, Vector3 size, Vector3 boxcolliderSize, int number = 4)
    {
        Particle particle = null;

        int l = 1;
        for (int i = 0; i < number; i++) {
            l += (i % 2 == 0 && i != 0) ? 1 : 0;
            
            for (int j = 0; j < (int)Math.Floor(boxcolliderSize.x); j++) {
                for (int k = 0; k < (int)Math.Floor(boxcolliderSize.y); k++) {
                    particle = ActorRegistry.SetActor("particle", new Vector3(pos.x - ((boxcolliderSize.x - 1f) / 2f) * size.x + j * size.x, pos.y - ((boxcolliderSize.y - 1f) / 2f) * size.y + k * size.y), size).GetComponent<Particle>();

                    particle.rigidBody.velocity = particle.RigidVector((i % 2 == 0) ? -4f : 4f, 7f * l);
                    particle.anim.Play(TypeToString(type) + "_particle");

                    particle.StartCoroutine(particle.Rotate(i % 2 != 0));
                }
            }
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

            case ParticleType.UsedBlock:
                return "used_block";
        }
    }

    public enum ParticleType
    {
        QuestionBlock,
        BrickBlock,
        FlipBlock,
        FrozenBrickBlock,
        IceBlock,
        ThreeUpMoonBlock,
        UsedBlock
    }
}