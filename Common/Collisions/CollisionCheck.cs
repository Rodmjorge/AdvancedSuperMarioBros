using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheck
{
    public static bool isOnGround(BoxCollider2D boxCollider, LayerMask layerMask) {

        RaycastHit2D rh, rh2, rh3;
        Vector3 shiftRH2D = new Vector3(boxCollider.bounds.size.x / 2f - 0.05f, 0f, 0f);

        rh = Physics2D.Raycast(boxCollider.bounds.center, Vector2.down, boxCollider.bounds.extents.y + 0.1f, layerMask);
        rh2 = Physics2D.Raycast(boxCollider.bounds.center + shiftRH2D, Vector2.down, boxCollider.bounds.extents.y + 0.1f, layerMask);
        rh3 = Physics2D.Raycast(boxCollider.bounds.center - shiftRH2D, Vector2.down, boxCollider.bounds.extents.y + 0.1f, layerMask);

        return (rh.collider != null || rh2.collider != null || rh3.collider != null);
    }

    public static bool isTouchingLeftWall(BoxCollider2D boxCollider, LayerMask layerMask) {

        RaycastHit2D rh, rh2, rh3;
        Vector3 shiftRH2D = new Vector3(0f, boxCollider.bounds.size.y / 2f - 0.05f, 0f);

        rh = Physics2D.Raycast(boxCollider.bounds.center, Vector2.left, boxCollider.bounds.extents.x + 0.05f, layerMask);
        rh2 = Physics2D.Raycast(boxCollider.bounds.center + shiftRH2D, Vector2.left, boxCollider.bounds.extents.x + 0.05f, layerMask);
        rh3 = Physics2D.Raycast(boxCollider.bounds.center - shiftRH2D, Vector2.left, boxCollider.bounds.extents.x + 0.05f, layerMask);

        return (rh.collider != null || rh2.collider != null || rh3.collider != null);
    }

    public static bool isTouchingRightWall(BoxCollider2D boxCollider, LayerMask layerMask) {

        RaycastHit2D rh, rh2, rh3;
        Vector3 shiftRH2D = new Vector3(0f, boxCollider.bounds.size.y / 2f - 0.05f, 0f);

        rh = Physics2D.Raycast(boxCollider.bounds.center, Vector2.right, boxCollider.bounds.extents.x + 0.05f, layerMask);
        rh2 = Physics2D.Raycast(boxCollider.bounds.center + shiftRH2D, Vector2.right, boxCollider.bounds.extents.x + 0.05f, layerMask);
        rh3 = Physics2D.Raycast(boxCollider.bounds.center - shiftRH2D, Vector2.right, boxCollider.bounds.extents.x + 0.05f, layerMask);

        return (rh.collider != null || rh2.collider != null || rh3.collider != null);
    }

    public static bool isTouchingCeiling(BoxCollider2D boxCollider, LayerMask layerMask) {

        RaycastHit2D rh, rh2, rh3;
        Vector3 shiftRH2D = new Vector3(boxCollider.bounds.size.x / 2f - 0.05f, 0f, 0f);

        rh = Physics2D.Raycast(boxCollider.bounds.center, Vector2.up, boxCollider.bounds.extents.y + 0.1f, layerMask);
        rh2 = Physics2D.Raycast(boxCollider.bounds.center + shiftRH2D, Vector2.up, boxCollider.bounds.extents.y + 0.1f, layerMask);
        rh3 = Physics2D.Raycast(boxCollider.bounds.center - shiftRH2D, Vector2.up, boxCollider.bounds.extents.y + 0.1f, layerMask);

        return (rh.collider != null || rh2.collider != null || rh3.collider != null);
    }

    public static bool checkForMiddleGrounded(BoxCollider2D boxCollider, LayerMask layerMask) {
        RaycastHit2D rh = Physics2D.Raycast(boxCollider.bounds.center, Vector2.down, boxCollider.bounds.extents.y + 0.1f, layerMask);
        return rh.collider != null;
    }
}
