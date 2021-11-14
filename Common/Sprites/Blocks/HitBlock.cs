using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBlock : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private Block block;

    [HideInInspector] public bool canHit = true;
    private float blockAnim = default;
    private Vector3 firstPos;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        block = Block.GetBlock(gameObject);

        firstPos = transform.position;
    }

    private void Update()
    {
        if (blockAnim > 0f) { BlockAnim(); }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        OnTriggerStay2D(collision.collider);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (blockAnim > 1f && blockAnim < 1.04f) {
            switch (Sprite.TypeOfSprite(collision.gameObject)) {

                case "enemy":
                    Enemy enemy = Enemy.GetEnemy(collision.gameObject);
                    enemy.HitBySomething();

                    break;

                case "block":
                    string blockName = Block.GetBlockParent(collision.gameObject);
                    GetTypeOfBlock(blockName, collision.gameObject);

                    break;

                default:
                    break;
            }
        }
    }

    public void BlockAnim()
    {
        blockAnim += Time.deltaTime;

        byte size = Block.GetBlockSize(gameObject);
        canHit = false;

        if (blockAnim < 1.15f) {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, 2.5f);
            transform.localScale = new Vector3(transform.localScale.x + 0.005f * size, transform.localScale.y + 0.005f * size, 1f);
        }
        else if (blockAnim < 1.30f) {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, -2.5f);
            transform.localScale = new Vector3(transform.localScale.x - 0.005f * size, transform.localScale.y - 0.005f * size, 1f);
        }
        else {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0f);
            transform.position = firstPos;
            transform.localScale = new Vector3(size, size, 1f);

            block.DestroyPlayer();
            blockAnim = 0f;
            canHit = true;
        }
    }

    public void GetTypeOfBlock(string name, GameObject block)
    {
        switch (name) {

            case "coin":
                block.GetComponent<Coin>().containerBlockCoin = true;
                break;

            default:
                break;
        }
    }

    public bool IsDoingAnimation()
    {
        return (blockAnim != 0f);
    }

    public bool CanHitBlock()
    {
        return canHit;
    }

    public void DoAnimation()
    {
        blockAnim++;
    }
}
