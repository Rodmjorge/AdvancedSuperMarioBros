using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    [HideInInspector] public string typeOfParticle = "brick_block"; //default value
    [HideInInspector] public byte value = 0;

    private Animator animator;
    private Rigidbody2D rigidBody;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();

        animator.Play(typeOfParticle + "_particle");
        switch (value) {
            case 0:
            default:
                rigidBody.velocity = new Vector2(-4f, 9f);
                break;

            case 1:
                rigidBody.velocity = new Vector2(-4f, 15f);
                break;

            case 2:
                rigidBody.velocity = new Vector2(4f, 9f);
                break;

            case 3:
                rigidBody.velocity = new Vector2(4f, 15f);
                break;
        }
    }

    private void Update()
    {
        if (!LevelSettings.playerDied) {
            switch (value) {
                case 0:
                case 1:
                default:
                    rigidBody.velocity = new Vector2(-4f, rigidBody.velocity.y);
                    break;

                case 2:
                case 3:
                    rigidBody.velocity = new Vector2(4f, rigidBody.velocity.y);
                    break;
            }

            rigidBody.isKinematic = false;
        }
        else { 
            rigidBody.velocity = Vector2.zero;
            rigidBody.isKinematic = true;
        }
    }
}
