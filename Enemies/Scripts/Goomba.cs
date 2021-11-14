using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goomba : MonoBehaviour
{
    public LayerMask layerMask;
    private BoxCollider2D boxCollider;
    private Animator animator;
    private BasicSpriteMovement enemyMovement;
    private Enemy enemy;

    public enum TypeOfGoomba {
        Goomba,
        Goombrat,
        Galoomba,
        Goombud
    };
    public TypeOfGoomba typeOfGoomba;
    public bool startGoingLeft = true;

    private float killingDeltaTime = 0f;


    private void Start() {
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        enemyMovement = gameObject.AddComponent<BasicSpriteMovement>();
        enemy = Enemy.GetEnemy(gameObject);

        enemyMovement.layerMask = layerMask;
        enemyMovement.leftDirection = startGoingLeft;

        //if it's a galoomba/goombud, delete this component and add the Galoomba component
        switch (typeOfGoomba) {

            case TypeOfGoomba.Goomba:
                animator.Play("walking");
                enemyMovement.dontWalkOffEdges = false;

                break;

            case TypeOfGoomba.Galoomba:
                animator.Play("walking_galoomba");
                enemyMovement.dontWalkOffEdges = false;

                Galoomba galoomba = gameObject.AddComponent<Galoomba>();
                galoomba.isGoombud = false;
                Destroy(this);

                break;

            case TypeOfGoomba.Goombrat:
                animator.Play("walking_goombrat");
                enemyMovement.dontWalkOffEdges = true;

                break;

            case TypeOfGoomba.Goombud:
                animator.Play("walking_goombud");
                enemyMovement.dontWalkOffEdges = true;

                Galoomba goombud = gameObject.AddComponent<Galoomba>();
                goombud.isGoombud = true;
                Destroy(this);

                break;

            default:
                break;
        }
    }

    private void Update() {

        if (enemy.playerHit) {
            PlayerHitEnemy();
        }
    }

    public void PlayerHitEnemy() {

        if (enemy.EnemyGotJumpedOn(enemy.playerThatHit, 0.25f)) {

            switch (typeOfGoomba) {
                case TypeOfGoomba.Goomba:
                    animator.Play("death");
                    KilledGoombaOrGoombrat();

                    break;

                case TypeOfGoomba.Goombrat:
                    animator.Play("death_goombrat");
                    KilledGoombaOrGoombrat();

                    break;

                default:
                    break;
            }
        }
    }

    //only for goombas and goombrats
    public void KilledGoombaOrGoombrat() {

        //will only do this once
        if (killingDeltaTime == 0) {
            enemy.EnemyOnAllAxis(true, false, new MonoBehaviour[] { this });
            enemy.KillEnemy(gameObject, enemy.playerThatHit);

            killingDeltaTime++;
        }

        killingDeltaTime += Time.deltaTime;
        if (killingDeltaTime > 1.15f) {
            boxCollider.enabled = false;
        }
        if (killingDeltaTime > 1.3f) {
            gameObject.SetActive(false);
        }
    }
}
