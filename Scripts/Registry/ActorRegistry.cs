using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActorRegistry : MonoBehaviour
{
    internal static Dictionary<string, ActorSettings> actors = new Dictionary<string, ActorSettings>();
    private static ActorRegistry actorBase;

    internal static bool isPaused { get { return LevelManager.IsPaused(); } }


    internal virtual void Awake() { actorBase = this.gameObject.GetComponent<ActorRegistry>(); }
    internal abstract string GetSpritePath();
    internal abstract string GetAnimatorPath();


    public static void RegisterActor(string internalName, ActorSettings settings) { 
        if (!actors.ContainsKey(internalName)) actors.Add(internalName, settings); 
    }


    public static ActorRegistry GetActorBase() { return actorBase; }

    public static Actor GetActor(string internalName) 
    { 
        ActorSettings settings = actors.GetValueOrDefault(internalName);
        return (settings != null) ? settings.actorClass : null;
    }
    public static bool TryGetActor(string internalName, out Actor actor)
    {
        bool b = actors.TryGetValue(internalName, out ActorSettings settings);
        actor = b ? settings.actorClass : null;

        return b;
    }
    public static bool GetActorComponent(GameObject GO, out Actor actor)
    {
        actor = GO.GetComponent<Actor>();
        return actor != null;
    }

    //createdToNormalTime = null means that it will only become a real actor when it touches the ground
    public static Actor SetActor(string internalName, Vector3? pos = null, Vector3? size = null, ActorSettings.CreatedActorTypes type = ActorSettings.CreatedActorTypes.None, float? time = null)
    {
        Actor actor = SetGameobject(internalName, pos, size).GetComponent<Actor>();

        switch (type) {
            case ActorSettings.CreatedActorTypes.CreatedLayer:
                actor.gameObject.layer = LayerMask.NameToLayer("Created" + LayerMask.LayerToName(actor.gameObject.layer));
                actor.StartCoroutine(ChangeCreatedLayer(actor.gameObject, time));
                break;

            case ActorSettings.CreatedActorTypes.UntilGroundTouch:
                actor.BooleanBoxCollider(false);
                actor.StartCoroutine(ChangeBoxColliderEnabling(actor, ColliderCheck.WallDirection.Ground));
                break;

            case ActorSettings.CreatedActorTypes.EnableAfterTime:
                actor.StartCoroutine(actor.EnableBoxcollider((time != null) ? time.Value : 0.5f));
                break;
        }

        return actor;
    }
    public static GameObject SetGameobject(string internalName, Vector3? pos = null, Vector3? size = null)
    {
        return CreateGameobject(internalName, pos, size, LevelLoader.actorParent);
    }
    public static GameObject CreateGameobject(string internalName, Vector3? pos = null, Vector3? size = null, GameObject parent = null)
    {
        if (actors.TryGetValue(internalName, out ActorSettings settings)) {
            GameObject GO = new GameObject() { 
                name = internalName,
                layer = settings.layer
            };

            Actor actor = (Actor)GO.AddComponent(settings.actorClass.GetType());
            if (pos != null) GO.transform.position = pos.Value;
            if (size != null) GO.transform.localScale = size.Value;
            if (parent != null) GO.transform.parent = parent.transform;


            ActorSettings createSettings = new ActorSettings(GO, settings);

            //sprite renderer
            createSettings.create(settings.defaultSprite, new bool[] { settings.flipX, settings.flipY }, settings.color, settings.layerOrder, settings.sortingLayer);
            //box collider
            createSettings.create(settings.size, settings.offset, settings.smoothing, settings.isTrigger, settings.boxColliderMaterial);
            //rigidbody
            createSettings.create(settings.isKinematic, settings.rigidbodyMaterial, settings.collisionDetectionMode);
            //animator
            createSettings.create(settings.animatorController);
            //area effector
            if (settings.useColliderMask) createSettings.create(settings.colliderMask, settings.forceTarget);

            actor.settings = settings;
            return GO;
        }

        return null;
    }

    public static IEnumerator ChangeCreatedLayer(GameObject GO, float? time)
    {
        Actor.TimerClass timer = new Actor.TimerClass(1);
        Actor actor = GO.GetComponent<Actor>();

        while (true) {
            if (!isPaused) {
                int i = LayerMask.NameToLayer(LayerMask.LayerToName(GO.layer).Replace("Created", "")); ;

                if ((time != null && timer.UntilTime(time.Value)) || (time == null && ColliderCheck.CollidedWithWall(ColliderCheck.WallDirection.Ground, actor.boxCollider, LayerMaskInterface.grounded))) {
                    GO.layer = i;
                    yield break;
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }
    public static IEnumerator ChangeBoxColliderEnabling(Actor actor, ColliderCheck.WallDirection direction)
    {
        while (true) {
            if (!isPaused) {
                if (ColliderCheck.CollidedWithWall(direction, actor.boxCollider, LayerMaskInterface.grounded)) {
                    actor.BooleanBoxCollider(true);
                    yield break;
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }


    public class ActorSettings
    {
        public Actor actorClass;
        public int layer;

        [Header("Sprite Renderer")]
        public Sprite defaultSprite;
        public bool flipX;
        public bool flipY;
        public Color? color;
        public int layerOrder;
        public int sortingLayer;

        [Header("Box Collider")]
        public Vector2 size = new Vector2(0.1f, 0.1f);
        public Vector2 offset;
        public float smoothing = defaultSmoothing;
        public PhysicsMaterial2D boxColliderMaterial = defaultPhysicMaterial;
        public bool isTrigger;

        [Header("Rigidbody")]
        public bool isKinematic;
        public PhysicsMaterial2D rigidbodyMaterial;
        public CollisionDetectionMode2D collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        [Header("Animator")]
        public RuntimeAnimatorController animatorController;

        [Header("Area Effector")]
        public bool useColliderMask;
        public LayerMask colliderMask;
        public EffectorSelection2D forceTarget;

        private GameObject gameObject;
        private ActorSettings actorSettings;
        public ActorSettings(GameObject gameObject = null, ActorSettings actorSettings = null)
        {
            this.gameObject = gameObject;
            this.actorSettings = actorSettings;
        }

        //default
        protected static float defaultSmoothing { get { return 0.0035f; } }
        protected static PhysicsMaterial2D defaultPhysicMaterial { get { return PhysicMaterialInterface.NoFriction(); } }


        protected readonly static bool useFullKinematicContacts = true;
        protected readonly static float rbGravityScale = 5;
        protected readonly static bool rbFreezeRot = true;
        protected readonly static bool animApplyRootMotion = false;
        protected readonly static float forceMagnitude = 0;

        public SpriteRenderer create(Sprite sprite, bool[] flipped = null, Color? color = null, int order = 0, int sortingLayer = 0)
        {
            SpriteRenderer spriteR = gameObject.AddComponent<SpriteRenderer>();

            spriteR.sprite = sprite;
            spriteR.color = (color != null) ? color.Value : Color.white;
            spriteR.sortingOrder = order;
            spriteR.sortingLayerID = sortingLayer;

            bool flippedNotNull = (flipped != null);
            spriteR.flipX = flippedNotNull ? (flipped.Length > 0 ? flipped[0] : false) : false;
            spriteR.flipY = flippedNotNull ? (flipped.Length > 1 ? flipped[1] : false) : false;

            return spriteR;
        }

        public BoxCollider2D create(Vector2 size, Vector2 offset, float smooth, bool isTrigger = false, PhysicsMaterial2D physics = null, bool usedByEffector = false)
        {
            BoxCollider2D boxCollider = gameObject.AddComponent<BoxCollider2D>();

            boxCollider.size = size;
            boxCollider.offset = offset;
            boxCollider.edgeRadius = smooth;
            boxCollider.isTrigger = isTrigger;
            boxCollider.sharedMaterial = physics;
            boxCollider.usedByEffector = usedByEffector;

            return boxCollider;
        }

        public Rigidbody2D create(bool isKinematic = false, PhysicsMaterial2D physics = null, CollisionDetectionMode2D collisionDetection = CollisionDetectionMode2D.Discrete)
        {
            Rigidbody2D rigidBody = gameObject.AddComponent<Rigidbody2D>();

            rigidBody.isKinematic = isKinematic;
            rigidBody.useFullKinematicContacts = useFullKinematicContacts;
            rigidBody.sharedMaterial = physics;
            rigidBody.collisionDetectionMode = collisionDetection;

            rigidBody.gravityScale = rbGravityScale;
            rigidBody.freezeRotation = rbFreezeRot;

            return rigidBody;
        }

        public Animator create(RuntimeAnimatorController controller)
        {
            Animator animator = gameObject.AddComponent<Animator>();

            animator.runtimeAnimatorController = controller;
            animator.applyRootMotion = animApplyRootMotion;

            return animator;
        }

        public AreaEffector2D create(LayerMask colliderMask, EffectorSelection2D effectorSelection = EffectorSelection2D.Rigidbody)
        {
            AreaEffector2D areaEffector = gameObject.AddComponent<AreaEffector2D>();

            areaEffector.useColliderMask = true;
            areaEffector.colliderMask = colliderMask;
            areaEffector.forceTarget = effectorSelection;
            areaEffector.forceMagnitude = forceMagnitude;

            create(actorSettings.size, actorSettings.offset, actorSettings.smoothing, true, actorSettings.boxColliderMaterial, true);

            return areaEffector;
        }

        public enum CreatedActorTypes
        {
            None,
            CreatedLayer,
            UntilGroundTouch,
            EnableAfterTime
        }
    }
}