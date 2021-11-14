using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    private Enemy enemy;

    private void Start()
    {
        enemy = Enemy.GetEnemy(gameObject);
    }

    private void Update()
    {
        if (enemy.playerHit) {
            Player player = Player.GetPlayer(enemy.playerThatHit);
            player.PlayerGotHit();
        }
    }
}
