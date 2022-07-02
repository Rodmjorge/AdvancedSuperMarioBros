﻿using System.Collections;
using UnityEngine;

public interface LayerMaskInterface
{
    public static int playerLayer { get { return LayerMask.NameToLayer("Player"); } }
    public static int groundLayer { get { return LayerMask.NameToLayer("Ground"); } }
    public static int enemyLayer { get { return LayerMask.NameToLayer("Enemy"); } }
    public static int blockLayer { get { return LayerMask.NameToLayer("Block"); } }

    public static LayerMask justGround { get { return 1 << groundLayer; } }
    public static LayerMask justBloked { get { return 1 << blockLayer; } }
    public static LayerMask grounded { get { return justGround + justBloked; } }
    public static LayerMask enemy { get { return 1 << enemyLayer; } }
}