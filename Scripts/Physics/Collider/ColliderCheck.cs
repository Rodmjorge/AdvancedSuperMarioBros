using System.Collections;
using UnityEngine;

public static class ColliderCheck
{
    public static bool CollidedWithWall(WallDirection direction, BoxCollider2D boxCollider, LayerMask layerMask, RaycastThird returnRaycast = RaycastThird.All, float shiftDivided = 1f, bool checkIfEnabled = false)
    {
        RaycastHit2D rh1, rh2, rh3;

        Vector3 shiftRH2D;
        Vector2 vectorDirect;
        float extents;
        float extentsAddition;

        switch (direction) {
            default:
            case WallDirection.Ground:
                shiftRH2D = new Vector3((boxCollider.bounds.size.x / 2f) / shiftDivided - 0.045f, 0f, 0f);
                vectorDirect = Vector2.down;
                extents = boxCollider.bounds.extents.y;
                extentsAddition = 0.1f;

                break;

            case WallDirection.Ceiling:
                shiftRH2D = new Vector3((boxCollider.bounds.size.x / 2f) / shiftDivided - 0.045f, 0f, 0f);
                vectorDirect = Vector2.up;
                extents = boxCollider.bounds.extents.y;
                extentsAddition = 0.1f;

                break;

            case WallDirection.LeftWall:
                shiftRH2D = new Vector3(0f, (boxCollider.bounds.size.y / 2f) / shiftDivided - 0.045f, 0f);
                vectorDirect = Vector2.left;
                extents = boxCollider.bounds.extents.x;
                extentsAddition = 0.05f;

                break;

            case WallDirection.RightWall:
                shiftRH2D = new Vector3(0f, (boxCollider.bounds.size.y / 2f) / shiftDivided - 0.045f, 0f);
                vectorDirect = Vector2.right;
                extents = boxCollider.bounds.extents.x;
                extentsAddition = 0.05f;

                break;
        }

        rh1 = Physics2D.Raycast(boxCollider.bounds.center, vectorDirect, extents + extentsAddition, layerMask);
        rh2 = Physics2D.Raycast(boxCollider.bounds.center + shiftRH2D, vectorDirect, extents + extentsAddition, layerMask);
        rh3 = Physics2D.Raycast(boxCollider.bounds.center - shiftRH2D, vectorDirect, extents + extentsAddition, layerMask);

        if (checkIfEnabled && !boxCollider.enabled)
            return false;

        switch (returnRaycast) {
            default:
            case RaycastThird.All:
                return rh1.collider != null || rh2.collider != null || rh3.collider != null;

            case RaycastThird.AllTrue:
                return rh1.collider != null && rh2.collider != null && rh3.collider != null;

            case RaycastThird.Left:
                return rh3.collider != null;

            case RaycastThird.Right:
                return rh2.collider != null;

            case RaycastThird.Middle:
                return rh1.collider != null;
        }
    }

    public static bool InsideCollider(Vector3 pos, Transform trans, LayerMask layerMask, float? radius = null)
    {
        return Physics2D.OverlapCircle(pos, (radius == null) ? trans.localScale.x / 2f : radius.Value, layerMask) != null;
    }


    public enum WallDirection
    {
        Ground,
        Ceiling,
        LeftWall,
        RightWall
    }

    public enum RaycastThird
    {
        Left,
        Right,
        Middle,
        All,
        AllTrue
    }
}