using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesRegistry : ActorRegistry
{
    internal override void Awake()
    {
        RegisterActor("ground", new ActorSettings() {
            actorClass = new Tiles(),
            layer = LayerMaskInterface.groundLayer,

            defaultSprite = Resources.Load<Sprite>(GetSpritePath() + "overworld_ground"),
            size = new Vector2(1f, 1f),
            sortingLayer = SortingLayerInterface.groundLayer,
            isKinematic = true
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

        base.Awake();
    }

    internal override string GetSpritePath() { return Actor.SpritePath("Tiles"); }
    internal override string GetAnimatorPath() { return Actor.AnimatorPath("Tiles"); }
}
