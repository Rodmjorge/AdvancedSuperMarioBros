using System.Collections;
using UnityEngine;

public class Goombud : Galoomba
{
    public override bool DoTurnOnEdges() { return true; }

    public override float GetCloserEdgeFloat() { return 3f; }
}