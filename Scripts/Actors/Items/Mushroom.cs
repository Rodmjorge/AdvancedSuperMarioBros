using System.Collections;
using UnityEngine;

public class Mushroom : PowerUp
{
    public override int GetPowerUpInt() { return 1; }
    public override bool UsesBasicMovement() { return true; }
    public override float DefaultMoveSpeed() { return 4f; }
}