using System.Collections;
using UnityEngine;

public class PowerUp : Items
{
    public override void PlayerCollided(Player player)
    {
        player.StartCoroutine(player.GotPowerup(GetPowerUpInt(), this));
        Destroy(gameObject);
    }

    public virtual int GetPowerUpInt() { return 0; }
}