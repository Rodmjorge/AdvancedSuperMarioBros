using System.Collections;
using UnityEngine;

public interface PhysicMaterialInterface
{
    private static string path = "Materials/";

    public static PhysicsMaterial2D NoFriction() { return Resources.Load<PhysicsMaterial2D>(path + "NoFriction"); }
    public static PhysicsMaterial2D PlayerFriction() { return Resources.Load<PhysicsMaterial2D>(path + "PlayerFriction"); }
}