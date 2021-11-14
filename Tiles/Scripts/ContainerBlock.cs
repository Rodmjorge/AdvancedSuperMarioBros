using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerBlock : MonoBehaviour
{
    private Block block;
    private BoxCollider2D boxCollider;
    private HitBlock hitBlock;

    private bool playUsedBlockAnim = default;
    private bool usedBlock = default;
    private float multipleCoinsCounter = default;

    public GameObject coinAsset;
    public GameObject spriteAsset;
    public GameObject particleAsset;
    private string nameOfParticle;

    public enum TypeOfContainerBlock {
        QuestionBlock,
        BrickBlock,
        InvisibleBlock,
        ThreeUpMoonBlock,
        ThreeLongQuestionBlock,
        SpikedQuestionBlock,
        FlipBlock
    }
    public TypeOfContainerBlock typeOfContainerBlock;

    private GameObject spikesFromSpikedQB;
    private float spikesRotatingDT;
    private bool rotated180Degrees;
    public float spikedQuestionBlockTimeToRotate = 3f;

    public enum WhatItContains {
        Coin,
        MultipleCoins,
        Sprite,
        MultipleSprites,
        BreaksWhenPoweredUp,

        //these last two are just to test animations and shit i forgor
        Nothing,
        NothingButDoesntTurnToUsedBlock
    }
    public WhatItContains whatItContains;

    public byte numberWhenMultiple = default;
    private byte numberOfHits = 0;

    private void Start() {
        boxCollider = GetComponent<BoxCollider2D>();
        hitBlock = gameObject.AddComponent<HitBlock>();
        block = Block.GetBlock(gameObject);
        
        switch (typeOfContainerBlock) {
            case TypeOfContainerBlock.QuestionBlock:
            default:
                block.PlayAnim("question_block");
                nameOfParticle = "question_block";
                break;

            case TypeOfContainerBlock.BrickBlock:
                block.PlayAnim("brick_block");
                nameOfParticle = "brick_block";
                break;

            case TypeOfContainerBlock.InvisibleBlock:
                block.PlayAnim("invisible_block");
                nameOfParticle = "used_block";
                break;

            case TypeOfContainerBlock.ThreeUpMoonBlock:
                block.PlayAnim("3up_moon_block");
                nameOfParticle = "3up_moon_block";
                break;

            case TypeOfContainerBlock.ThreeLongQuestionBlock:
                block.PlayAnim("threeblock_question_block");
                nameOfParticle = "question_block";
                break;

            case TypeOfContainerBlock.SpikedQuestionBlock:
                block.PlayAnim("spiked_question_block");
                nameOfParticle = "spiked_question_block";

                spikesFromSpikedQB = transform.GetChild(0).gameObject;
                break;

            case TypeOfContainerBlock.FlipBlock:
                block.PlayAnim("flip_block");
                nameOfParticle = "question_block";
                break;
        }
    }

    private void Update() {

        if (block.playerCollided && !usedBlock && hitBlock.CanHitBlock() && block.PlayerCollidedBelow(block.playerThatCollided)) {

            if (typeOfContainerBlock == TypeOfContainerBlock.InvisibleBlock) {

                if (block.playerThatCollided.GetComponent<Rigidbody2D>().velocity.y >= 0f) { BlockWork(); }
                else { block.DestroyPlayer(); }
            }
            else {
                BlockWork();
            }
        }
        else {
            block.DestroyPlayer();
        }

        //check to turn into used block
        if (hitBlock.IsDoingAnimation()) { BlockAnim(playUsedBlockAnim); }

        if (multipleCoinsCounter != 0f) { StartMultipleCoinCounter(); }
        if (typeOfContainerBlock == TypeOfContainerBlock.SpikedQuestionBlock && !usedBlock) {
            RotateSpikedQuestionBlock();
        }
    }

    public void BlockWork() {

        if (!hitBlock.IsDoingAnimation()) {

            switch (whatItContains) {
                case WhatItContains.Coin:

                    if (Sprite.IsCoin(coinAsset)) {
                        Transform transformC = Sprite.CreateAssetWithReturn(coinAsset, transform.position, true, gameObject, true);
                        transformC.GetComponent<Coin>().containerBlockCoin = true;

                        if (typeOfContainerBlock == TypeOfContainerBlock.ThreeLongQuestionBlock) {
                            Transform transformC2 = Sprite.CreateAssetWithReturn(coinAsset, new Vector3(transform.position.x - 1f, transform.position.y, transform.position.z), true, gameObject, true);
                            Transform transformC3 = Sprite.CreateAssetWithReturn(coinAsset, new Vector3(transform.position.x + 1f, transform.position.y, transform.position.z), true, gameObject, true);
                            transformC2.GetComponent<Coin>().containerBlockCoin = transformC3.GetComponent<Coin>().containerBlockCoin = true;
                        }
                    }

                    playUsedBlockAnim = true;
                    break;

                case WhatItContains.MultipleCoins:

                    if (Sprite.IsCoin(coinAsset)) {
                        Transform transformC = Sprite.CreateAssetWithReturn(coinAsset, transform.position, true, gameObject, true);
                        transformC.GetComponent<Coin>().containerBlockCoin = true;

                        if (typeOfContainerBlock == TypeOfContainerBlock.ThreeLongQuestionBlock) {
                            Transform transformC2 = Sprite.CreateAssetWithReturn(coinAsset, new Vector3(transform.position.x - 1f, transform.position.y, transform.position.z), true, gameObject, true);
                            Transform transformC3 = Sprite.CreateAssetWithReturn(coinAsset, new Vector3(transform.position.x + 1f, transform.position.y, transform.position.z), true, gameObject, true);
                            transformC2.GetComponent<Coin>().containerBlockCoin = transformC3.GetComponent<Coin>().containerBlockCoin = true;
                        }

                        numberOfHits++;
                    }

                    if (typeOfContainerBlock == TypeOfContainerBlock.InvisibleBlock) {
                        block.PlayAnim("question_block");
                    }

                    StartMultipleCoinCounter();
                    playUsedBlockAnim = (multipleCoinsCounter < 1f);
                    break;

                case WhatItContains.Sprite:
                    if (spriteAsset != null) {
                        CheckWhatSprite();
                    }

                    playUsedBlockAnim = true;
                    break;

                case WhatItContains.MultipleSprites:
                    if (spriteAsset != null) {
                        CheckWhatSprite();
                        numberOfHits++;
                    }

                    if (typeOfContainerBlock == TypeOfContainerBlock.InvisibleBlock) {
                        block.PlayAnim("question_block");
                    }

                    StartMultipleCoinCounter();
                    playUsedBlockAnim = (multipleCoinsCounter < 1f);
                    break;

                case WhatItContains.BreaksWhenPoweredUp:
                    Player.GetPlayerMovement(block.playerThatCollided).ResetVelocityY();
                    block.CreateParticles(particleAsset, gameObject, nameOfParticle);

                    break;

                case WhatItContains.Nothing:
                    playUsedBlockAnim = true;
                    break;

                case WhatItContains.NothingButDoesntTurnToUsedBlock:
                    playUsedBlockAnim = false;
                    break;

                default:
                    break;
            }
            hitBlock.DoAnimation();

            if (typeOfContainerBlock == TypeOfContainerBlock.InvisibleBlock) {
                boxCollider.isTrigger = false;
                gameObject.layer = 10;
            }
        }
    }

    public void CheckWhatSprite()
    {
        Transform transformS = Sprite.CreateAssetWithReturn(spriteAsset, 
            new Vector3(transform.position.x, transform.position.y + 0.75f * Block.GetBlockSize(gameObject), 0f), true, gameObject, true);

        switch (Sprite.TypeOfSprite(transformS.gameObject)) {

            case "player":
            case "enemy":
                Rigidbody2D rb = transformS.gameObject.GetComponent<Rigidbody2D>();
                rb.velocity = new Vector2(rb.velocity.x, 15f);
                break;

            case "object":
                Rigidbody2D objectRB = transformS.gameObject.GetComponent<Rigidbody2D>();
                transformS.gameObject.GetComponent<BoxCollider2D>().enabled = false;
                transformS.position -= new Vector3(0f, 0.75f * Block.GetBlockSize(gameObject), 0f);

                ObjectM.GetObject(transformS.gameObject).dontSnapToPosition = true;

                ObjectM.GetObject(transformS.gameObject).fromContainer = true;
                ObjectM.GetObject(transformS.gameObject).blockPos = transform;
                ObjectM.GetObject(transformS.gameObject).boxColliderGO = Block.GetBoxColliderSize(gameObject).y;
                break;

            case "block":
                CheckWhatBlock(transformS.gameObject);
                break;
        }
    }

    public void CheckWhatBlock(GameObject GO)
    {
        switch (Block.GetBlockName(GO)) {
            case "coin":
                GO.transform.position -= new Vector3(0f, 0.75f * Block.GetBlockSize(gameObject), 0f);
                GO.GetComponent<Coin>().containerBlockCoin = true;

                break;

            default:
                break;
        }
    }

    public void StartMultipleCoinCounter() {

        //just do this one time to calculate the time
        if (multipleCoinsCounter == 0f) {
            multipleCoinsCounter = float.Parse(numberWhenMultiple.ToString()) / 2f + 1f;
        }

        multipleCoinsCounter = (multipleCoinsCounter > 1f) ? multipleCoinsCounter - Time.deltaTime : multipleCoinsCounter;
        if (numberOfHits >= numberWhenMultiple) {
            multipleCoinsCounter = 1f / 2f;
        }
    }

    public void BlockAnim(bool transformToUsedBlock) {

        switch (typeOfContainerBlock) {

            case TypeOfContainerBlock.ThreeLongQuestionBlock:
                block.PlayAnim("threeblock_used_block", transformToUsedBlock);
                break;

            case TypeOfContainerBlock.SpikedQuestionBlock:
                block.PlayAnim("spiked_used_block", transformToUsedBlock);

                //when it transforms into a used block, destroy the spikes and rotate it to 0 on z-axis.
                if (transformToUsedBlock) {
                    transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
                    Destroy(spikesFromSpikedQB); 
                }

                break;

            default:
                block.PlayAnim("used_block", transformToUsedBlock);
                break;
        }

        usedBlock = transformToUsedBlock;
    }

    public void RotateSpikedQuestionBlock()
    {
        spikesRotatingDT += Time.deltaTime;

        if (spikesRotatingDT > spikedQuestionBlockTimeToRotate + 0.5f) {
            transform.Rotate(0f, 0f, -2f);
            hitBlock.canHit = false;
        }

        if (spikesRotatingDT > spikedQuestionBlockTimeToRotate + 0.7f) {
            transform.rotation = rotated180Degrees ? new Quaternion(0f, 0f, 0f, 0f) : new Quaternion(0f, 0f, 180f, 0f);
            hitBlock.canHit = true;

            rotated180Degrees = !rotated180Degrees;
            spikesRotatingDT = 0f;
        }
    }
}
