using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRegistry : ActorRegistry
{
    internal override void Awake()
    {
        RegisterActor("goomba", new ActorSettings() {
            actorClass = new Goomba(),
            layer = LayerMaskInterface.enemyLayer,

            defaultSprite = Resources.Load<Sprite>(GetSpritePath() + "gumba_1"),
            size = new Vector2(0.85f, 0.8f),
            offset = new Vector2(0f, -0.1f),
            sortingLayer = SortingLayerInterface.enemiesLayer,
            animatorController = Resources.Load<RuntimeAnimatorController>(GetAnimatorPath() + "goomba")
        });
        RegisterActor("goombrat", new ActorSettings() {
            actorClass = new Goombrat(),
            layer = LayerMaskInterface.enemyLayer,

            defaultSprite = Resources.Load<Sprite>(GetSpritePath() + "gumbrat_1"),
            size = new Vector2(0.85f, 0.8f),
            offset = new Vector2(0f, -0.1f),
            sortingLayer = SortingLayerInterface.enemiesLayer,
            animatorController = Resources.Load<RuntimeAnimatorController>(GetAnimatorPath() + "goombrat")
        });

        base.Awake();
    }

    internal override string GetSpritePath() { return Actor.SpritePath("Enemies"); }
    internal override string GetAnimatorPath() { return Actor.AnimatorPath("Enemies"); }
}
