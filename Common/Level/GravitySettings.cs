using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySettings
{
    public static Vector2 MaxVelocity(Rigidbody2D rigidBody, float maxValue) {
        maxValue = -maxValue * LevelSettings.GetGravity();

        if (rigidBody.velocity.y < maxValue) {
            return new Vector2(rigidBody.velocity.x, maxValue);
        }
        else {
            return new Vector2(rigidBody.velocity.x, rigidBody.velocity.y);
        }
    }
}
