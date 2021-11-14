using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*most enemies have this kind of movement, so why not make a whole script which moves them this way*/
public class BasicSpriteMovement : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private BoxCollider2D boxCollider;

    private Enemy enemy;
    private ObjectM objectM;

    public bool leftDirection = true;
    public bool dontWalkOffEdges = default;
    public float speed = 2f;
    public LayerMask layerMask;


    private void Start() {
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        switch (Sprite.TypeOfSprite(gameObject)) {
            case "enemy":
                enemy = Enemy.GetEnemy(gameObject);
                enemy.EnemyOnXAxis(true, false);
                break;

            case "object":
                objectM = ObjectM.GetObject(gameObject);
                objectM.ObjectOnXAxis(true, false);
                break;
        }
    }

    private void Update() {
        if (!(rigidBody.constraints.HasFlag(RigidbodyConstraints2D.FreezePosition))) {
            Movement(rigidBody, boxCollider, speed); 
        }

        if (CollisionCheck.isOnGround(boxCollider, layerMask)) {
            if (Sprite.TypeOfSprite(gameObject) == "enemy") { enemy.EnemyOnXAxis(false, false); }
            else { objectM.ObjectOnXAxis(false, false); }
        }
    }

    public void Movement(Rigidbody2D rigidBody, BoxCollider2D boxCollider, float velocity) {

        if (leftDirection) { rigidBody.velocity = new Vector2(-velocity, rigidBody.velocity.y); }
        else { rigidBody.velocity = new Vector2(velocity, rigidBody.velocity.y); }

        TurnAround(boxCollider, leftDirection);

        if (dontWalkOffEdges) {
            TurnAroundOnEdges(boxCollider, leftDirection);
        }
    }

    //check to turn around
    public void TurnAround(BoxCollider2D boxCollider, bool isLeft) {

        if (isLeft ? CollisionCheck.isTouchingLeftWall(boxCollider, layerMask) : CollisionCheck.isTouchingRightWall(boxCollider, layerMask)) {
            leftDirection = !leftDirection;
        }
    }

    //don't walk off edges lol goombas are such fucking idiots lol mfs walking off edges what a bunch of dumbfucks, #redKoopaTroopaAndGoombratGang
    public void TurnAroundOnEdges(BoxCollider2D boxCollider, bool isLeft) {

        Vector3 shiftRH2D = isLeft ? new Vector3(-0.1f, 0f, 0f) : new Vector3(0.1f, 0f, 0f);
        RaycastHit2D rh = Physics2D.Raycast(boxCollider.bounds.center + shiftRH2D, Vector2.down, boxCollider.bounds.extents.y + 0.05f, layerMask);

        if (rh.collider == null && CollisionCheck.checkForMiddleGrounded(boxCollider, layerMask)) {
            leftDirection = !leftDirection;
        }
    }
}
