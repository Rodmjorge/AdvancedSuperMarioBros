using System.Collections;
using UnityEngine;

public class ItemRegistry : ActorRegistry
{
    internal override void Awake()
    {
        RegisterActor("mushroom", new ActorSettings() {
            actorClass = new Mushroom(),
            layer = LayerMaskInterface.itemLayer,

            defaultSprite = Resources.Load<Sprite>(GetSpritePath() + "mushroom"),
            size = new Vector2(0.8f, 0.8f),
            offset = new Vector2(0f, -0.1f),
            sortingLayer = SortingLayerInterface.itemLayer
        });
        RegisterActor("fire_flower", new ActorSettings() {
            actorClass = new FireFlower(),
            layer = LayerMaskInterface.itemLayer,

            defaultSprite = Resources.Load<Sprite>(GetSpritePath() + "fire_flower_1"),
            size = new Vector2(0.9f, 0.9f),
            offset = new Vector2(0f, -0.05f),
            sortingLayer = SortingLayerInterface.itemLayer,
            animatorController = Resources.Load<RuntimeAnimatorController>(GetAnimatorPath() + "fire_flower")
        });

        base.Awake();
    }

    internal override string GetSpritePath() { return Actor.SpritePath("Items"); }
    internal override string GetAnimatorPath() { return Actor.AnimatorPath("Items"); }
}