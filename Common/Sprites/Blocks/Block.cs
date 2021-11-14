using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Block : MonoBehaviour 
{
    private Rigidbody2D rigidBody;
    private BoxCollider2D boxCollider;
    private Animator animator;

    //register
    public ulong blockID = default;
    public string blockName = default;
    public string parent = default;

    public Vector2 boxColliderSize = new Vector2(1f, 1f);
    public Vector2 boxColliderOffset = new Vector2(0f, 0f);
    public bool isBoxColliderTrigger = default;
    public bool isRigidbodyKinematic = default;
    public byte blockSize = 1;

    //block player hits
    [HideInInspector] public bool playerCollided = default;
    [HideInInspector] public GameObject playerThatCollided = default;

    //block checks
    [HideInInspector] public bool stopped = default;
    private bool froze = default;

    private void Awake() {
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();

        boxCollider.size = boxColliderSize;
        boxCollider.offset = boxColliderOffset;
        boxCollider.isTrigger = isBoxColliderTrigger;

        rigidBody.gravityScale = LevelSettings.GetGravity();
        rigidBody.isKinematic = isRigidbodyKinematic;

        gameObject.name = blockName;
        gameObject.tag = "Block";

        float f = float.Parse(blockSize.ToString());
        transform.localScale = new Vector3(blockSize, blockSize, 1);
        transform.position = LevelSettings.ShiftToPosition(transform, (f != 1) ? f / 4f : 0f);

        parent = string.IsNullOrEmpty(parent) ? blockName : parent;
    }

    private void Update() {
        rigidBody.velocity = GravitySettings.MaxVelocity(rigidBody, 3f);

        if (LevelSettings.playerDied || LevelSettings.stopEverything) {
            BlockOnAllAxis(true, true);
        }
    }

    public void BlockOnAllAxis(bool freeze, bool freezeAnim, MonoBehaviour[] notToDisable = null) {
        rigidBody.isKinematic = stopped = freeze;
        rigidBody.constraints = freeze ? RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation : RigidbodyConstraints2D.FreezeRotation;

        animator.enabled = freezeAnim ? false : animator.enabled;
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

    public bool PlayerCollidedBelow(GameObject playerGO) 
    {
        Vector3 checkDirection = playerGO.transform.position - gameObject.transform.position;

        return (checkDirection.y < 0.35f * Block.GetBlockSize(gameObject)); 
    }

    public void CreateParticles(GameObject particle, GameObject block, string nameOfParticle, byte numberOfParticles = 4)
    {
        if (particle.GetComponent<Particle>() != null) {

            for (byte b = 0; b < numberOfParticles; b++) {
                Transform transformP = Instantiate(particle.transform, block.transform.position, Quaternion.identity);

                transformP.GetComponent<Particle>().value = b;
                transformP.GetComponent<Particle>().typeOfParticle = nameOfParticle;
                transformP.gameObject.name = nameOfParticle + "_particle";
            }

            Destroy(block);
        }
    }

    public void PlayAnim(string anim, bool checkWhenToPlay = true) {
        if (animator.enabled && checkWhenToPlay) { 
            animator.Play(anim); 
        }
    }

    public void DestroyPlayer() {
        playerThatCollided = null;
        playerCollided = false;
    }

    public static Block GetBlock(GameObject block) 
    {
        return block.GetComponent<Block>();
    }

    public static string GetBlockName(GameObject block) 
    {
        return GetBlock(block).blockName;
    }

    public static string GetBlockParent(GameObject block)
    {
        return GetBlock(block).parent;
    }

    public static byte GetBlockSize(GameObject block) 
    {
        return GetBlock(block).blockSize;
    }

    public static Vector2 GetBoxColliderSize(GameObject block)
    {
        return GetBlock(block).boxColliderSize;
    }
}
