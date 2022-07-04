﻿using System.Collections;
using UnityEngine;

public class Goombrat : Goomba
{
    public override bool DoTurnOnEdges() { return true; }

    public override float GetCloserEdgeFloat() { return 3f; }

    public override string GetSpawningEnemy() { return "goombrat"; }
}