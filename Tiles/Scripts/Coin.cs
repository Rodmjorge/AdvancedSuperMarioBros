using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour 
{
    private Rigidbody2D rigidBody;
    private Animator animator;
    private Block block;

    [HideInInspector] public bool containerBlockCoin = default;
    private float containerBlockCoinAnim;

    private void Start() {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        block = Block.GetBlock(gameObject);
    }

    private void Update() {

        if (block.playerCollided && !containerBlockCoin) {
            CoinCollected();
        }

        //for container block coin
        if (containerBlockCoin) {
            CoinFromContainer();
        }
    }

    public void CoinCollected() {
        MainSettings.ChangeCoinCounter(1);
        gameObject.SetActive(false);
    }

    public void CoinFromContainer() {
        block.PlayAnim("coin_rotating");
        containerBlockCoinAnim += Time.deltaTime;

        if (containerBlockCoinAnim < 0.3f) { rigidBody.velocity = new Vector2(rigidBody.velocity.x, 2.5f / (containerBlockCoinAnim + 0.08f)); }
        else if (containerBlockCoinAnim < 0.5f) { rigidBody.velocity = new Vector2(rigidBody.velocity.x, -2.5f / (0.5f - (containerBlockCoinAnim - 0.08f))); }
        else {
            MainSettings.ChangeCoinCounter(1);
            Destroy(gameObject);
        }
    }
}
