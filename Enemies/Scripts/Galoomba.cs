using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Galoomba : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    private BasicSpriteMovement enemyMovement;
    private Enemy enemy;

    public bool isGoombud = default;

    private void Start() {
        boxCollider = GetComponent<BoxCollider2D>();
        enemyMovement = gameObject.AddComponent<BasicSpriteMovement>();
        enemy = Enemy.GetEnemy(gameObject);
    }
}
