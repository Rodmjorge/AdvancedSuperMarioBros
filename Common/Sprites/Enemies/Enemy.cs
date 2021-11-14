using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour 
{
    private Rigidbody2D rigidBody;
    private BoxCollider2D boxCollider;
    private Animator animator;

    //register
    public ulong enemyID = default;
    public string enemyName = default;
    public string parent = default;

    public Vector2 boxColliderSize = new Vector2(1f, 1f);
    public Vector2 boxColliderOffset = new Vector2(0f, 0f);
    public bool isBoxColliderTrigger = default;
    public bool isRigidbodyKinematic = default;
    public bool dontRoundUpPosition = default;
    public byte enemySize = 1;

    //enemy passes
    [HideInInspector] public bool playerHit = default;
    [HideInInspector] public GameObject playerThatHit = default;
    [HideInInspector] public bool hitFromAbove = default;

    //enemy checks
    [HideInInspector] public bool stopped = default;
    private bool hitBySomething = default;
    private bool froze = default;


    private void Awake() {
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();

        boxCollider.size = boxColliderSize;
        boxCollider.offset = boxColliderOffset;
        rigidBody.gravityScale = LevelSettings.GetGravity();

        gameObject.name = enemyName;
        gameObject.tag = "Enemy";

        transform.localScale = new Vector3(enemySize, enemySize, 1);
        if (!dontRoundUpPosition) {
            float f = float.Parse(enemySize.ToString());
            transform.position = LevelSettings.ShiftToPosition(transform, (f != 1) ? f / 4f : 0f);
        }

        parent = string.IsNullOrEmpty(parent) ? enemyName : parent;
    }

    private void Update() {
        rigidBody.velocity = GravitySettings.MaxVelocity(rigidBody, 3f);

        if (LevelSettings.playerDied || LevelSettings.stopEverything) {
            EnemyOnAllAxis(true, true);
        }

        if (hitBySomething) {
            HitBySomething();
        }
    }

    public void EnemyOnAllAxis(bool freeze, bool freezeAnim, MonoBehaviour[] notToDisable = null, bool desfreezeAnim = false) {
        rigidBody.isKinematic = stopped = freeze;
        rigidBody.constraints = freeze ? RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation : RigidbodyConstraints2D.FreezeRotation;

        if (gameObject.GetComponent<Animator>() != null) { 

            animator.enabled = freezeAnim ? false : animator.enabled;
            if (desfreezeAnim) { animator.enabled = true; }
        }

        AllComponentsBelow(freeze, notToDisable);
    }

    public void AllComponentsBelow(bool disable, MonoBehaviour[] notToDisable)
    {
        int i = Array.IndexOf(gameObject.GetComponents<MonoBehaviour>(), this);
        for (int j = 0; j < gameObject.GetComponents<MonoBehaviour>().Length - i; j++) {
            gameObject.GetComponents<MonoBehaviour>()[j].enabled = !disable;
        }
        this.enabled = true;

        if (notToDisable != null) {
            if (notToDisable.Length > 0) {
                for (int k = 0; k < notToDisable.Length; k++) {
                    notToDisable[k].enabled = true;
                }
            }
        }
    }

    public void EnemyOnXAxis(bool freeze, bool freezeAnim) {
        rigidBody.constraints = freeze ? RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation : RigidbodyConstraints2D.FreezeRotation;

        animator.enabled = freezeAnim ? false : animator.enabled;
    }

    public void EnemyOnYAxis(bool freeze, bool freezeAnim) {
        rigidBody.constraints = freeze ? RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation : RigidbodyConstraints2D.FreezeRotation;

        animator.enabled = freezeAnim ? false : animator.enabled;
    }

    public void HitBySomething()
    {
        boxCollider.enabled = false;

        //do this only one time
        if (!hitBySomething) { 
            rigidBody.velocity = new Vector2(0f, 11f);
            gameObject.GetComponent<SpriteRenderer>().sortingOrder += 20;

            AllComponentsBelow(true, new MonoBehaviour[] { this });
        }
        hitBySomething = true;

        transform.Rotate(0f, 0f, -2f);
        rigidBody.velocity = new Vector2(-3f, rigidBody.velocity.y);
    }

    public void JumpedOnEnemy(GameObject player) {
        PlayerMovement playerMovement = Player.GetPlayerMovement(player);
        Rigidbody2D playerRigidbody = player.GetComponent<Rigidbody2D>();

        playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, 2.5f * LevelSettings.GetGravity());
        playerMovement.ResetJump();
    }

    public bool EnemyGotJumpedOn(GameObject playerGO, float checkPlayerYAxis, string deadTag = "DeadEnemy") {

        Vector3 checkDirection = playerGO.transform.position - gameObject.transform.position;

        if (checkDirection.y > checkPlayerYAxis || gameObject.tag == deadTag) { //check if player is above the enemy
            return true;
        }
        else { //check if player is below the enemy, if he is, DIE DIE DIE BURN IN HELL TOO!!!!!! except luigi, weegee is aweoseme :)))
            Player player = Player.GetPlayer(playerGO);
            player.PlayerGotHit();

            return false; //if you are reading this, i love you! :)
        }
    }

    public void KillEnemy(GameObject enemy, GameObject player) {

        JumpedOnEnemy(player);
        enemy.tag = "DeadEnemy";
    }

    public static Enemy GetEnemy(GameObject enemy) {
        return enemy.GetComponent<Enemy>();
    }

    public static ulong GetEnemyId(GameObject enemy) {
        return GetEnemy(enemy).enemyID;
    }

    public static string GetEnemyName(GameObject enemy) {
        return GetEnemy(enemy).enemyName;
    }

    public static string GetEnemyParent(GameObject enemy) {
        Enemy component = GetEnemy(enemy);
        return !(component.parent == component.enemyName) ? component.parent : string.Empty;
    }

    public static Vector2 GetEnemyColliderSize(GameObject enemy) {
        return GetEnemy(enemy).boxColliderSize;
    }

    public static Vector2 GetEnemyColliderOffset(GameObject enemy) {
        return GetEnemy(enemy).boxColliderOffset;
    }

    public static byte GetEnemySize(GameObject enemy) {
        return GetEnemy(enemy).enemySize;
    }
}
