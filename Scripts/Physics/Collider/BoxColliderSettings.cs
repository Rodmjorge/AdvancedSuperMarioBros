using System.Collections;
using UnityEngine;

public class BoxColliderSettings
{
    protected readonly Actor actor;

    protected readonly BoxCollider2D[] boxcolliders;
    protected BoxCollider2D boxcollider { get { return boxcolliders[0]; } }

    protected readonly GameObject gameObject;

    /*1 - y positive
     *2 - y negative
     *3 - x positive
     *4 - x negative*/
    internal float[] boxColliderBounds = new float[4];

    public Vector2 center
    {
        get {
            Transform transform = gameObject.transform;
            return new Vector2(boxcollider.offset.x * transform.localScale.x, boxcollider.offset.y * transform.localScale.y);
        }
    }
    public Vector2 extents { 
        get {
            Transform transform = gameObject.transform;
            return new Vector2((boxcollider.size.x * transform.localScale.x) / 2f, (boxcollider.size.y * transform.localScale.y) / 2f);
        } 
    }

    public BoxColliderSettings(Actor actor, GameObject gameObject)
    {
        this.actor = actor;
        this.boxcolliders = gameObject.GetComponents<BoxCollider2D>();
        this.gameObject = gameObject;
    }

    public BoxCollider2D SetBoxColliderBounds(float yPos = 0f, float yNeg = 0f, float xPos = 0f, float xNeg = 0f)
    {
        boxColliderBounds[0] = yPos;
        boxColliderBounds[1] = yNeg;
        boxColliderBounds[2] = xPos;
        boxColliderBounds[3] = xNeg;

        return boxcollider;
    }

    public BoxCollider2D SetBoxColliderBoundsPos(float yPos = 0f, float xPos = 0f) { return SetBoxColliderBounds(yPos, boxcollider.size.y - yPos, xPos, boxcollider.size.x - xPos); }
    public BoxCollider2D SetBoxColliderBoundsNeg(float yNeg = 0f, float xNeg = 0f) { return SetBoxColliderBounds(boxcollider.size.y - yNeg, yNeg, boxcollider.size.x - xNeg, xNeg); }

    public BoxCollider2D SetBoxCollider(Vector2? size, Vector2? offset)
    {
        Vector2 size0 = (size == null) ? boxcollider.size : size.Value;
        Vector2 offset0 = (offset == null) ? boxcollider.offset : offset.Value;

        boxcollider.size = size0;
        boxcollider.offset = offset0;
        if (boxcolliders.Length > 1) {
            boxcolliders[1].size = size0;
            boxcolliders[1].offset = offset0;
        }

        return boxcollider;
    }

    public Vector2 GetCenterPosition()
    {
        Vector2 pos = gameObject.transform.position;
        return new Vector2(pos.x + center.x, pos.y + center.y);
    }

    public float GetExtentsYPos() { return GetCenterPosition().y + extents.y; }
    public float GetExtentsYNeg() { return GetCenterPosition().y - extents.y; }
}
