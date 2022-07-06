using System.Collections;
using UnityEngine;

public class Coin : Tiles
{
    public override void PlayerCollided(Player player)
    {
        SetTargetBoolean(true);
        Destroy(gameObject);
    }


    public override Particle GetParticle() { return null; }
    public override bool IsDestructable() { return false; }
}