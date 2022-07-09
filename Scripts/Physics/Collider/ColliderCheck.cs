using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColliderCheck
{
    public static RaycastHit2D[] GetRaycast(WallDirection direction, BoxCollider2D boxCollider, LayerMask layerMask, float shiftDivided = 1f)
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

        return new RaycastHit2D[] { rh1, rh2, rh3 };
    }

    public static bool CollidedWithWall(WallDirection direction, BoxCollider2D boxCollider, LayerMask layerMask, RaycastThird returnRaycast = RaycastThird.All, float shiftDivided = 1f, bool checkIfEnabled = false, RaycastHit2D[] rh0 = null)
    {
        if (checkIfEnabled && !boxCollider.enabled)
            return false;

        RaycastHit2D[] rh = (rh0 == null) ? GetRaycast(direction, boxCollider, layerMask, shiftDivided) : rh0;
        switch (returnRaycast) {
            default:
            case RaycastThird.All:
                return rh[0].collider != null || rh[1].collider != null || rh[2].collider != null;

            case RaycastThird.AllTrue:
                return rh[0].collider != null && rh[1].collider != null && rh[2].collider != null;

            case RaycastThird.Left:
                return rh[2].collider != null;

            case RaycastThird.Right:
                return rh[1].collider != null;

            case RaycastThird.Middle:
                return rh[0].collider != null;
        }
    }

    public static bool GetActorCollided(WallDirection direction, BoxCollider2D boxCollider, LayerMask layerMask, out Actor[] actor, RaycastThird returnRaycast = RaycastThird.All, float shiftDivided = 1f, bool checkIfEnabled = false)
    {
        RaycastHit2D[] rh = GetRaycast(direction, boxCollider, layerMask, shiftDivided);
        List<Actor> actorList = new List<Actor>();

        foreach (RaycastHit2D rhCol in rh)
            if (rhCol.collider != null) actorList.Add(rhCol.collider.gameObject.GetComponent<Actor>());
        actor = actorList.ToArray();

        return CollidedWithWall(direction, boxCollider, layerMask, returnRaycast, shiftDivided, checkIfEnabled, rh);
    }

    public static bool InsideCollider(Vector2 pos, Vector2 size, LayerMask layerMask)
    {
        return Physics2D.OverlapBox(pos, size, 0f, layerMask) != null;
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