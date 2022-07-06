using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesRegistry : ActorRegistry
{
    internal override void Awake()
    {
        RegisterActor("ground", new ActorSettings() {
            actorClass = new Ground(),
            layer = LayerMaskInterface.groundLayer,

            defaultSprite = Resources.Load<Sprite>(GetSpritePath() + "overworld_ground"),
            size = new Vector2(1f, 1f),
            sortingLayer = SortingLayerInterface.groundLayer,
            isKinematic = true
        });

        RegisterActor("particle", new ActorSettings() {
            actorClass = new Particle(),
            layer = LayerMaskInterface.particleLayer,

            defaultSprite = Resources.Load<Sprite>(GetSpritePath() + "question_block_particle"),
            isTrigger = true,
            size = new Vector2(0.4f, 0.4f),
            sortingLayer = SortingLayerInterface.playerLayer,
            animatorController = Resources.Load<RuntimeAnimatorController>(GetAnimatorPath() + "particle")
        });

        RegisterActor("question_block", new ActorSettings() {
            actorClass = new ContainerBlock(),
            layer = LayerMaskInterface.blockLayer,

            defaultSprite = Resources.Load<Sprite>(GetSpritePath() + "question_blok1"),
            size = new Vector2(1f, 1f),
            sortingLayer = SortingLayerInterface.blockLayer,
            isKinematic = true,
            animatorController = Resources.Load<RuntimeAnimatorController>(GetAnimatorPath() + "question_block")
        });
        RegisterActor("3wide_question_block", new ActorSettings() {
            actorClass = new ThreeWideContainerBlock(),
            layer = LayerMaskInterface.blockLayer,

            defaultSprite = Resources.Load<Sprite>(GetSpritePath() + "3block_question_blok1"),
            size = new Vector2(3f, 1f),
            sortingLayer = SortingLayerInterface.blockLayer,
            isKinematic = true,
            animatorController = Resources.Load<RuntimeAnimatorController>(GetAnimatorPath() + "3wide_question_block")
        });

        RegisterActor("flip_block", new ActorSettings() {
            actorClass = new FlipBlock(),
            layer = LayerMaskInterface.blockLayer,

            defaultSprite = Resources.Load<Sprite>(GetSpritePath() + "flip_block"),
            size = new Vector2(1f, 1f),
            sortingLayer = SortingLayerInterface.blockLayer,
            isKinematic = true,
            animatorController = Resources.Load<RuntimeAnimatorController>(GetAnimatorPath() + "flip_block")
        });

        RegisterActor("coin", new ActorSettings() {
            actorClass = new Coin(),
            layer = LayerMaskInterface.tBlockLayer,

            defaultSprite = Resources.Load<Sprite>(GetSpritePath() + "coin_1"),
            size = new Vector2(0.7f, 0.8f),
            isTrigger = true,
            sortingLayer = SortingLayerInterface.blockLayer,
            isKinematic = true,
            animatorController = Resources.Load<RuntimeAnimatorController>(GetAnimatorPath() + "coin")
        });

        base.Awake();
    }

    internal override string GetSpritePath() { return Actor.SpritePath("Tiles"); }
    internal override string GetAnimatorPath() { return Actor.AnimatorPath("Tiles"); }
}
