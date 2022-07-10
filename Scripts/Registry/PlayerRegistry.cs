using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRegistry : ActorRegistry
{
    internal override void Awake()
    {
        RegisterActor("player", new ActorSettings() {
            actorClass = new Player(),
            layer = LayerMaskInterface.playerLayer,

            defaultSprite = Resources.Load<Sprite>(GetSpritePath() + "player_mario"),
            size = Player.boxcolliderSizes[0][0],
            offset = Player.boxcolliderSizes[0][1],
            boxColliderMaterial = PhysicMaterialInterface.PlayerFriction(),
            sortingLayer = SortingLayerInterface.playerLayer,
            animatorController = Resources.Load<RuntimeAnimatorController>(GetAnimatorPath() + "player"),

            useColliderMask = true,
            colliderMask = LayerMaskInterface.enemy + LayerMaskInterface.tBloked + LayerMaskInterface.itemed
        });

        base.Awake();
    }

    internal override string GetSpritePath() { return Player.GetSpritePath(); }
    internal override string GetAnimatorPath() { return Actor.AnimatorPath("Player"); }
}