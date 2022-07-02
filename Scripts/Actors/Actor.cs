using System;
using System.Collections;
using UnityEngine;

public abstract class Actor : MonoBehaviour
{
    public bool pauseActor;

    public SpriteRenderer spriteR { get { return gameObject.GetComponent<SpriteRenderer>(); } }
    public Rigidbody2D rigidBody { get { return gameObject.GetComponent<Rigidbody2D>(); } }
    public Animator anim { get { return gameObject.GetComponent<Animator>(); } }

    public BoxCollider2D[] boxColliderArray { get { return gameObject.GetComponents<BoxCollider2D>(); } }
    public BoxCollider2D boxCollider { get { return boxColliderArray[0]; } }

    public static string SpritePath(string s) { return $"Sprites/{ s }/"; }
    public static string AnimatorPath(string s) { return $"Animations/{ s }/"; }


    protected BoxColliderSettings bcs;
    protected TimerClass timer;

    private bool isAlreadyKinematic;
    public virtual void Start() 
    {
        bcs = new BoxColliderSettings(this, gameObject);
        timer = new TimerClass(GetNumberOfTimers());

        isAlreadyKinematic = rigidBody.isKinematic;

        SetBoxColliderBounds();
    }

    private void FixedUpdate() 
    { 
        if (rigidBody.velocity.y < -15f) { rigidBody.velocity = RigidVector(null, -15); }

        if (Resume()) { Tick(); }
        else { PausedTick(); }
    }

    private bool changeUpdate;
    private void Update()
    {
        if (Resume()) {
            Framed();

            if (changeUpdate) {
                BooleanBoxAndRigid(true);
                BooleanAnim(true);

                changeUpdate = false;
            }
        }

        else {
            if (!pauseActor) {
                BooleanBoxAndRigid(false, true);
                BooleanAnim(false);

                changeUpdate = true;
            }
        }
    }

    public virtual void Tick() { return; }
    public virtual void PausedTick() { return; }
    public virtual void Framed() { return; }

    public abstract void DataLoaded(string s, string beforeEqual);


    public virtual Vector2 RigidVector(float? x, float? y)
    {
        float x0 = (x == null) ? rigidBody.velocity.x : x.Value;
        float y0 = (y == null) ? rigidBody.velocity.y : y.Value;

        return new Vector2(x0, y0);
    }

    public virtual void BooleanBoxCollider(bool b)
    {
        boxCollider.enabled = b;
        if (boxColliderArray.Length > 1) boxColliderArray[1].enabled = b;
    }
    public virtual void BooleanBoxAndRigid(bool b, bool zeroRigidbody = false)
    {
        BooleanBoxCollider(b);

        rigidBody.isKinematic = isAlreadyKinematic ? true : !b;
        rigidBody.velocity = zeroRigidbody ? RigidVector(0, 0) : rigidBody.velocity;
    }
    public virtual void BooleanAnim(bool b)
    {
        anim.enabled = b;
    }

    public virtual IEnumerator EnableBoxcollider(float time)
    {
        BooleanBoxCollider(false);
        TimerClass timerT = new TimerClass(1);

        while (true) {
            if (Resume()) {
                if (timerT.UntilTime(time)) {
                    BooleanBoxCollider(true);
                    yield break;
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }

    public virtual string PositionString(LevelLoader.TransformPos posRelative)
    {
        switch (posRelative) {
            default:
            case LevelLoader.TransformPos.X:
                return transform.position.x.ToString().Replace(',', '.');

            case LevelLoader.TransformPos.Y:
                return transform.position.y.ToString().Replace(',', '.');

            case LevelLoader.TransformPos.Z:
                return transform.position.z.ToString().Replace(',', '.');
        }
    }

    public virtual bool IsActor<T>(out T actor) where T : Actor
    {
        actor = (T)GetComponent(typeof(T));
        return actor != null;
    }

    public virtual bool Resume() { return ResumeGaming() && !pauseActor; }
    public virtual bool ResumeGaming() { return !LevelLoader.LevelSettings.IsPaused(); }

    public virtual void SetBoxColliderBounds() { return; }
    public virtual byte GetNumberOfTimers() { return 25; }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnTriggerEnter2D(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject colliderGameObject = collision.gameObject;
        Actor colliderActor = colliderGameObject.GetComponent<Actor>();

        BoxColliderSettings colliderBcs = new BoxColliderSettings(colliderActor, colliderGameObject);

        if (colliderBcs.GetExtentsYNeg() > bcs.GetExtentsYPos() - bcs.boxColliderBounds[0])
            CollidedAbove(colliderGameObject, colliderActor);
        if (colliderBcs.GetExtentsYNeg() < bcs.GetExtentsYNeg() + bcs.boxColliderBounds[1])
            CollidedBelow(colliderGameObject, colliderActor);

        Collided(colliderGameObject, colliderActor);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        OnTriggerStay2D(collision.collider);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        GameObject colliderGameObject = collision.gameObject;
        Actor colliderActor = colliderGameObject.GetComponent<Actor>();

        BoxColliderSettings colliderBcs = new BoxColliderSettings(colliderActor, colliderGameObject);

        if (colliderBcs.GetExtentsYNeg() > bcs.GetExtentsYPos() - bcs.boxColliderBounds[0])
            StayingCollidedAbove(colliderGameObject, colliderActor);
        if (colliderBcs.GetExtentsYNeg() < bcs.GetExtentsYNeg() + bcs.boxColliderBounds[1])
            StayingCollidedBelow(colliderGameObject, colliderActor);

        StayingCollided(colliderGameObject, colliderActor);
    }


    public virtual void Collided(GameObject GO, Actor actor) {

        if (Player.IsPlayer(GO, out Player player)) {
            PlayerCollided(player);
        }
    }
    public virtual void CollidedAbove(GameObject GO, Actor actor) {
        if (Player.IsPlayer(GO, out Player player)) {
            PlayerCollidedAbove(player);
        }
    }
    public virtual void CollidedBelow(GameObject GO, Actor actor) {
        if (Player.IsPlayer(GO, out Player player)) {
            PlayerCollidedBelow(player);
        }
    }

    public virtual void PlayerCollided(Player player) { return; }
    public virtual void PlayerCollidedAbove(Player player) { return; }
    public virtual void PlayerCollidedBelow(Player player) { return; }


    public virtual void StayingCollided(GameObject GO, Actor actor) { return; }
    public virtual void StayingCollidedAbove(GameObject GO, Actor actor) { return; }
    public virtual void StayingCollidedBelow(GameObject GO, Actor actor) { return; }


    public class TimerClass
    {
        internal int numberOfTimers;
        private float[] deltaTimes;

        public TimerClass(byte numberOfTimers)
        {
            deltaTimes = new float[numberOfTimers];
            this.numberOfTimers = numberOfTimers;
        }

        public bool UntilTime(float time, int numberOfTimer = 1, bool addToTimer = true)
        {
            int i = GetIndex(numberOfTimer);
            deltaTimes[i] += addToTimer ? Time.deltaTime : 0f;

            return deltaTimes[i] >= time;
        }

        public bool WhileTime(float time, int numberOfTimer = 1, bool addToTimer = true)
        {
            return !UntilTime(time, numberOfTimer, addToTimer);
        }

        public void AddToTimer(int numberOfTimer = 1) { deltaTimes[GetIndex(numberOfTimer)] += Time.deltaTime; }
        public void ResetTimer(int numberOfTimer = 1, float resetNumber = 0f) { deltaTimes[GetIndex(numberOfTimer)] = resetNumber; }
        public float GetTime(int numberOfTimer = 1) { return deltaTimes[GetIndex(numberOfTimer)]; }

        private int GetIndex(int i) { return i - 1; }
    }
}