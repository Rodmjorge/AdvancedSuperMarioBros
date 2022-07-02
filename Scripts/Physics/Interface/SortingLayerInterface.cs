using System.Collections;
using UnityEngine;

public interface SortingLayerInterface
{
    public static int playerLayer { get { return SortingLayer.NameToID("Player"); } }
    public static int enemiesLayer { get { return SortingLayer.NameToID("Enemies"); } }
    public static int groundLayer { get { return SortingLayer.NameToID("Ground"); } }
    public static int blockLayer { get { return SortingLayer.NameToID("Blocks"); } }
}