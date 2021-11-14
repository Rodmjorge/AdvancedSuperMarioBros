using System;
using UnityEngine;

public class ObjectM : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private BoxCollider2D boxCollider;
    private Animator animator;

    //register
    public ulong objectID = default;
    public string objectName = default;
    public string parent = default;

    public Vector2 boxColliderSize = new Vector2(1f, 1f);
    public Vector2 boxColliderOffset = new Vector2(0f, 0f);
    public bool isBoxColliderTrigger = default;
    public bool isRigidbodyKinematic = default;
    public bool dontSnapToPosition = default;
    public byte objectSize = 1;

    //object player hits
    [HideInInspector] public bool playerCollided = default;
    [HideInInspector] public GameObject playerThatCollided = default;

    //object checks
    [HideInInspector] public bool stopped = default;

    //from container
    [HideInInspector] public bool fromContainer = default;
    [HideInInspector] public Transform blockPos = default;
    [HideInInspector] public float boxColliderGO = default;
    private bool froze = default;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();

        boxCollider.size = boxColliderSize;
        boxCollider.offset = boxColliderOffset;
        boxCollider.isTrigger = isBoxColliderTrigger;

        rigidBody.gravityScale = LevelSettings.GetGravity();
        rigidBody.isKinematic = isRigidbodyKinematic;

        gameObject.name = objectName;
        gameObject.tag = "Object";

        transform.localScale = new Vector3(objectSize, objectSize, 1);
        if (!dontSnapToPosition) {
            float f = float.Parse(objectSize.ToString());
            transform.position = LevelSettings.ShiftToPosition(transform, (f != 1) ? f / 4f : 0f);
        }

        parent = string.IsNullOrEmpty(parent) ? objectName : parent;
    }

    private void Update() 
    {
        rigidBody.velocity = GravitySettings.MaxVelocity(rigidBody, 3f);

        if (LevelSettings.playerDied || LevelSettings.stopEverything) {
            ObjectOnAllAxis(true, true);
        }

        else {
            ContainerObject();
        }
    }

    public void ContainerObject()
    {
        if (fromContainer) {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, 3f * boxColliderGO);
            AllComponentsBelow(true, new MonoBehaviour[] { this });

            float f = (Block.GetBlockSize(blockPos.gameObject) == 1) ? 1f : Block.GetBlockSize(blockPos.gameObject) / 1.5f;
            if (transform.position.y > blockPos.position.y + boxColliderGO * f) {
                fromContainer = false;

                boxCollider.enabled = true;
                AllComponentsBelow(false, null);
            }
        }
    }

    public void ObjectOnAllAxis(bool freeze, bool freezeAnim, MonoBehaviour[] notToDisable = null, bool desfreezeAnim = false)
    {
        rigidBody.isKinematic = stopped = freeze;
        rigidBody.constraints = freeze ? RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation : RigidbodyConstraints2D.FreezeRotation;

        if (gameObject.GetComponent<Animator>() != null) {  animator.enabled = freezeAnim ? false : animator.enabled; }

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

    public void ObjectOnXAxis(bool freeze, bool freezeAnim)
    {
        rigidBody.constraints = freeze ? RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation : RigidbodyConstraints2D.FreezeRotation;

        if (gameObject.GetComponent<Animator>() != null) { animator.enabled = freezeAnim ? false : animator.enabled; }
    }

    public void PlayAnim(string anim, bool checkWhenToPlay = true)
    {
        if (animator.enabled && checkWhenToPlay) {
            animator.Play(anim);
        }
    }

    public void DestroyPlayer()
    {
        playerThatCollided = null;
        playerCollided = false;
    }

    public static ObjectM GetObject(GameObject objectM)
    {
        return objectM.GetComponent<ObjectM>();
    }

    public static byte GetObjectSize(GameObject objectM)
    {
        return GetObject(objectM).objectSize;
    }
}
