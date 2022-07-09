using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public abstract class Actor : MonoBehaviour
{
    public bool pauseActor;

    internal ushort targetIDSet;
    internal bool targetIDBool;

    internal bool isBeingHeld;
    internal bool isBeingThrown;

    public SpriteRenderer spriteR { get { return gameObject.GetComponent<SpriteRenderer>(); } }
    public Rigidbody2D rigidBody { get { return gameObject.GetComponent<Rigidbody2D>(); } }
    public Animator anim { get { return gameObject.GetComponent<Animator>(); } }

    public BoxCollider2D[] boxColliderArray { get { return gameObject.GetComponents<BoxCollider2D>(); } }
    public BoxCollider2D boxCollider { get { return boxColliderArray[0]; } }

    public static string SpritePath(string s) { return $"Sprites/{ s }/"; }
    public static string AnimatorPath(string s) { return $"Animations/{ s }/"; }

    protected BoxColliderSettings bcs;
    protected TimerClass timer;
    protected LevelManager.ScoreManager scoreManager;
    internal ActorRegistry.ActorSettings settings;

    public virtual void Start() 
    {
        bcs = new BoxColliderSettings(this, gameObject);
        timer = new TimerClass(GetNumberOfTimers(), this);
        scoreManager = new LevelManager.ScoreManager(gameObject);

        SetBoxColliderBounds();
        UpdateComponents();

        transform.position += ShiftPos();
    }

    private void FixedUpdate() 
    { 
        if (rigidBody.velocity.y < -15f) { rigidBody.velocity = RigidVector(null, -15); }

        if (Resume()) { 
            Tick();

            if (isBeingHeld) IsBeingHeldTick();

            if (IsInsideBloq() && !LayerMaskInterface.IsCreatedLayer(gameObject.layer) && boxCollider.enabled && !isBeingHeld) 
                InsideCollider();
        }
        else { PausedTick(); }
    }

    private bool changeUpdate;
    private bool[] updateComponents = new bool[3]; //1 - boxcollider; 2 - rigidbody; 3 - animator
    private void Update()
    {
        if (Resume()) {
            Framed();

            if (changeUpdate) {
                BooleanBoxAndRigid(true, false, false);
                BooleanAnim(true, false);

                changeUpdate = false;
            }

            UpdateComponents();
        }

        else {
            if (!pauseActor || LevelManager.IsPaused()) {
                BooleanBoxAndRigid(false, true);
                BooleanAnim(false);

                changeUpdate = true;
            }
        }
    }

    public virtual void Tick() { return; }
    public virtual void PausedTick() { return; }
    public virtual void IsBeingHeldTick() { return; }
    public virtual void Framed() { return; }

    public abstract void DataLoaded(string s, string beforeEqual);


    public virtual void SetTargetBoolean(bool b, float time = 0f)
    {
        if (IsValidTrigger(targetIDSet)) {
            targetIDBool = b;
            ActorRegistry.GetActorBase().StartCoroutine(timer.RunAfterTime(SetTrigger(b), time, null, false, true));
        }
    }

    protected ushort SetTargetID(string s, string beforeEqual) { return LevelLoader.CreateVariable(s, beforeEqual, "targetId", targetIDSet); }
    protected ushort SetTriggeredID(string s, string beforeEqual, ushort triggeredID) { return LevelLoader.CreateVariable(s, beforeEqual, "triggeredId", triggeredID); }


    private IEnumerator SetTrigger(bool activate)
    {
        LevelLoader.LevelSettings.SetTrigger(targetIDSet, activate);
        yield break;
    }
    public virtual bool? IsTargetActive(ushort triggeredID) { return (triggeredID <= 0) ? null : LevelLoader.LevelSettings.GetTrigger(triggeredID); }
    public virtual bool IsValidTrigger(ushort triggeredID) { return triggeredID > 0; }

    public virtual Vector3 ShiftPos(Sprite spriteR = null) { return ShiftBySpriteRenderer(spriteR); } 
    public virtual Vector3 ShiftBySpriteRenderer(Sprite spriteR0 = null) { return new Vector3(((spriteR0 == null ? spriteR.sprite.bounds.size.x : spriteR0.bounds.size.x) - 1f) / 2f, ((spriteR0 == null ? spriteR.sprite.bounds.size.y : spriteR0.bounds.size.y) - 1f) / 2f); }

    public virtual Vector2 RigidVector(float? x, float? y, bool groundTouch = false, float? time = null, int? numberOfTimer = null)
    {
        float x0 = (x == null) ? rigidBody.velocity.x : x.Value;
        float y0 = (y == null) ? rigidBody.velocity.y : y.Value;

        if (groundTouch) {
            if (time != null) StartCoroutine(timer.RunAfterTime(ZeroRigidbodyWhenGroundHit(), time.Value, numberOfTimer));
            else StartCoroutine(ZeroRigidbodyWhenGroundHit());
        }

        return new Vector2(x0, y0);
    }
    public virtual void UpdateComponents()
    {
        updateComponents[0] = boxCollider.enabled;
        updateComponents[1] = rigidBody.isKinematic;
        updateComponents[2] = anim.enabled;
    }

    public virtual bool IsInsideBloq()
    {
        return false;
    }
    public virtual void InsideCollider() { return; }

    public virtual void BooleanBoxCollider(bool b, bool ignoreComponents = true)
    {
        bool bb = ignoreComponents ? b : updateComponents[0];

        boxCollider.enabled = bb;
        if (boxColliderArray.Length > 1) boxColliderArray[1].enabled = bb;
    }
    public virtual void BooleanBoxAndRigid(bool b, bool zeroRigidbody = false, bool ignoreComponents = true)
    {
        BooleanBoxCollider(b, ignoreComponents);

        rigidBody.isKinematic = ignoreComponents ? !b : updateComponents[1];
        rigidBody.velocity = zeroRigidbody ? RigidVector(0, 0) : rigidBody.velocity;
    }
    public virtual void BooleanAnim(bool b, bool ignoreComponents = true)
    {
        anim.enabled = ignoreComponents ? b : updateComponents[2];
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

    public virtual IEnumerator ZeroRigidbodyWhenGroundHit()
    {
        while (true) {
            if (ColliderCheck.CollidedWithWall(ColliderCheck.WallDirection.Ground, boxCollider, LayerMaskInterface.grounded) && Resume()) {
                rigidBody.velocity = RigidVector(0f, 0f);
                yield break;
            }

            yield return new WaitForFixedUpdate();
        }
    }

    public virtual IEnumerator RigidbodyJumpsWhenGroundHit(bool lastOneStop, bool invertXWhenCollideWithWall, float time, params Vector2[] jump)
    {
        isBeingThrown = true;

        int i = 0;
        TimerClass timerT = new TimerClass(1);

        bool invert = false;
        while (true) {

            if (ColliderCheck.CollidedWithWall(ColliderCheck.WallDirection.Ground, boxCollider, LayerMaskInterface.grounded) && Resume()
                && timerT.UntilTime(time)) {
                rigidBody.velocity = RigidVector(invert ? -jump[i].x : jump[i].x, jump[i].y);
                i++;

                timerT.ResetTimer();
                if (i >= jump.Length) {
                    if (lastOneStop) RigidVector(0f, 0f, true, time);

                    scoreManager.ResetIndex();
                    isBeingThrown = false;

                    yield break;
                }
            }

            if (invertXWhenCollideWithWall && rigidBody.velocity.x != 0f) {
                if (ColliderCheck.CollidedWithWall(rigidBody.velocity.x > 0f ? ColliderCheck.WallDirection.RightWall : ColliderCheck.WallDirection.LeftWall, boxCollider, LayerMaskInterface.grounded)) {
                    rigidBody.velocity = RigidVector(-rigidBody.velocity.x, null);
                    invert = !invert;
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }

    //invincible frames - 99 timer
    public virtual IEnumerator InvincibleFrames(SpriteRenderer spriteR, float timeToStart, float timeToDie = 2f, IEnumerator runWhenStop = null, bool destroyGO = true)
    {
        GameObject GO = spriteR.gameObject;

        int i = 0;
        int j = 0;
        bool invert = false;
        while (true) {
            if (Resume() && timer != null) {
                if (timer.UntilTime(timeToStart, 99)) {
                    i += invert ? -1 : 1;

                    if (i > (8 - j * 2)) {
                        invert = true;
                        spriteR.color = new Color(spriteR.color.r, spriteR.color.g, spriteR.color.b, 0f);
                    }
                    else if (i < (-8 + j * 2)) {
                        invert = false;
                        spriteR.color = new Color(spriteR.color.r, spriteR.color.g, spriteR.color.b, 1f);
                    }

                    if (timer.UntilTime(timeToStart + 1f, 99, false) && j < 1)
                        j++;
                    else if (timer.UntilTime(timeToStart + 2f, 99, false) && j < 2)
                        j++;
                    else if (timer.UntilTime(timeToStart + 3f, 99, false) && j < 3)
                        j++;
                    else if (timer.UntilTime(timeToDie + timeToStart + 3f, 99, false)) {
                        if (runWhenStop != null) StartCoroutine(runWhenStop);
                        if (destroyGO) Destroy(GO);
                    }
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }
    public virtual void ResetInvincibleFrames(float resetTo = 0f)
    {
        if (timer != null) timer.ResetTimer(99, resetTo);
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

    public virtual bool IsHoldable() { return false; }
    public virtual bool CancelsOutWhenHolding() { return false; }
    public virtual bool DiesWhenCancelledOut() { return false; }

    public virtual void CreateAreaEffector(bool b, LayerMask layerMask = default)
    {
        if (b) {
            if (GetComponent<AreaEffector2D>() == null) {
                ActorRegistry.ActorSettings areaEffector = new ActorRegistry.ActorSettings(gameObject, settings);
                areaEffector.create(layerMask);
            }
        }
        else {
            if (GetComponent<AreaEffector2D>() != null) {
                Destroy(GetComponent<AreaEffector2D>());
                Destroy(boxColliderArray.Where(x => x.usedByEffector).ToArray()[0]);
            }
        }
    }

    public virtual void ChangeHoldingStatus(bool b) 
    { 
        isBeingHeld = b;
        CreateAreaEffector(b, LayerMaskInterface.enemyBlock);
    }

    public virtual void StartedHolding(Player player) { return; }

    public virtual void Thrown(Player player, bool changeHoldingStatus = true)
    {
        if (changeHoldingStatus) ChangeHoldingStatus(false);
        rigidBody.velocity = RigidVector(null, 0f);

        JumpThrown(player);

        if (IsInsideBloq())
            transform.position = player.bcs.GetCenterPosition();
    }

    public virtual void JumpThrown(Player player)
    {
        bool flipped = player.spriteR.flipX;
        if (player.IsCrouching()) {
            transform.position += new Vector3(flipped ? -0.4f : 0.4f, 0f);
            rigidBody.velocity = RigidVector(flipped ? -2f : 2f, null, true, 0.05f);
        }
        else {
            bool grounded = player.IsOnGround();
            float f = (flipped ? -9f : 9f) * (grounded ? 1f : 1.5f);

            rigidBody.velocity = RigidVector(f, grounded ? 6f : 8f);

            if (grounded) StartCoroutine(RigidbodyJumpsWhenGroundHit(true, false, 0.08f, RigidVector(f / 3f, 6f), RigidVector(f / 6f, 4f)));
            else StartCoroutine(RigidbodyJumpsWhenGroundHit(true, false, 0.08f, RigidVector(f / 2f, 7f), RigidVector(f / 5f, 5f), RigidVector(f / 7f, 3f)));

            AudioManager.PlayAudio("kick_enemy");
        }
    }

    public virtual bool IsActor<T>(out T actor) where T : Actor
    {
        actor = (T)GetComponent(typeof(T));
        return actor != null;
    }

    public virtual bool Resume() { return ResumeGaming() && !pauseActor; }
    public virtual bool ResumeGaming() { return !LevelManager.IsPaused(); }

    public virtual void SetBoxColliderBounds() { return; }
    public virtual byte GetNumberOfTimers() { return 100; }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnTriggerEnter2D(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Resume()) {
            GameObject colliderGameObject = collision.gameObject;
            Actor colliderActor = colliderGameObject.GetComponent<Actor>();

            BoxColliderSettings colliderBcs = new BoxColliderSettings(colliderActor, colliderGameObject);

            if (colliderBcs.GetExtentsYNeg() > bcs.GetExtentsYPos() - bcs.boxColliderBounds[0])
                CollidedAbove(colliderGameObject, colliderActor);
            if (colliderBcs.GetExtentsYNeg() < bcs.GetExtentsYNeg() + bcs.boxColliderBounds[1])
                CollidedBelow(colliderGameObject, colliderActor);

            Collided(colliderGameObject, colliderActor);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        OnTriggerStay2D(collision.collider);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Resume()) {
            GameObject colliderGameObject = collision.gameObject;
            Actor colliderActor = colliderGameObject.GetComponent<Actor>();

            BoxColliderSettings colliderBcs = new BoxColliderSettings(colliderActor, colliderGameObject);

            if (colliderBcs.GetExtentsYNeg() > bcs.GetExtentsYPos() - bcs.boxColliderBounds[0])
                StayingCollidedAbove(colliderGameObject, colliderActor);
            if (colliderBcs.GetExtentsYNeg() < bcs.GetExtentsYNeg() + bcs.boxColliderBounds[1])
                StayingCollidedBelow(colliderGameObject, colliderActor);

            StayingCollided(colliderGameObject, colliderActor);
        }
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

    public virtual bool HitByBlockBelow(Actor actor, out HitBlock hitBlock)
    {
        if (actor.IsActor(out HitBlock hitBlock0)) {
            hitBlock = hitBlock0;
            return (hitBlock0.TheBloqHasIndeedBeenHit() && hitBlock0.TimeOfHitting() <= 0.034f && bcs.GetExtentsYNeg() + 0.1f > actor.bcs.GetExtentsYPos());
        }
        else {
            hitBlock = null;
            return false;
        }
    }

    public virtual void PlayerCollided(Player player) { return; }
    public virtual void PlayerCollidedAbove(Player player) { return; }
    public virtual void PlayerCollidedBelow(Player player) { return; }


    public virtual void StayingCollided(GameObject GO, Actor actor) {
        if (Player.IsPlayer(GO, out Player player)) {
            PlayerStayingCollided(player);
        }
    }
    public virtual void StayingCollidedAbove(GameObject GO, Actor actor) {
        if (Player.IsPlayer(GO, out Player player)) {
            PlayerStayingCollidedAbove(player);
        }
    }
    public virtual void StayingCollidedBelow(GameObject GO, Actor actor) {
        if (Player.IsPlayer(GO, out Player player)) {
            PlayerStayingCollidedBelow(player);
        }
    }

    public virtual void PlayerStayingCollided(Player player) { return; }
    public virtual void PlayerStayingCollidedAbove(Player player) { return; }
    public virtual void PlayerStayingCollidedBelow(Player player) { return; }


    public class TimerClass
    {
        internal int numberOfTimers;
        private float[] deltaTimes;

        Actor actor;

        public TimerClass(byte numberOfTimers, Actor actor = null)
        {
            deltaTimes = new float[numberOfTimers];
            this.numberOfTimers = numberOfTimers;
            this.actor = actor;
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


        public IEnumerator IncreaseTime(float time, int numberOfTimer = 1, bool runEvenIfDisabled = false)
        {
            while (true) {
                if (runEvenIfDisabled || actor.Resume()) {

                    if (UntilTime(time, numberOfTimer))
                        yield break;
                }

                yield return new WaitForFixedUpdate();
            }
        }

        public IEnumerator RunAfterTime(IEnumerator i, float time, int? numberOfTimer = null, bool resetTimerAfter = false, bool runEvenIfDisabled = false, bool runEvenIfGamingDisabled = false)
        {
            TimerClass timerT = new TimerClass(1);

            while (true) {
                if (runEvenIfDisabled ? (runEvenIfGamingDisabled || actor.ResumeGaming()) : actor.Resume()) {
                    if ((numberOfTimer == null) ? timerT.UntilTime(time) : UntilTime(time, numberOfTimer.Value)) {
                        ActorRegistry.GetActorBase().StartCoroutine(i);

                        if (resetTimerAfter && numberOfTimer != null)
                            ResetTimer(numberOfTimer.Value);

                        yield break;
                    }
                }

                yield return new WaitForFixedUpdate();
            }
        }

        public IEnumerator ResetTimerAfterTime(float time, int numberOfTimer = 1, float resetNumber = 0f, bool runEvenIfDisabled = false)
        {
            while (true) {
                if (runEvenIfDisabled || actor.Resume()) {
                    if (UntilTime(time, numberOfTimer)) {
                        ResetTimer(numberOfTimer, resetNumber);
                        yield break;
                    }
                }

                yield return new WaitForFixedUpdate();
            }
        }

        public void AddToTimer(int numberOfTimer = 1) { deltaTimes[GetIndex(numberOfTimer)] += Time.deltaTime; }
        public void ResetTimer(int numberOfTimer = 1, float resetNumber = 0f) { deltaTimes[GetIndex(numberOfTimer)] = resetNumber; }
        public void SetTimer(float time, int numberOfTimer = 1) { deltaTimes[GetIndex(numberOfTimer)] = time; }
        public float GetTime(int numberOfTimer = 1) { return deltaTimes[GetIndex(numberOfTimer)]; }

        private int GetIndex(int i) { return i - 1; }
    }
}