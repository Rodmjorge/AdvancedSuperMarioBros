using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSettings : MonoBehaviour
{
    public static short coinCounter = 0;
    public static short lifeCounter = 5;
    public static ulong scoreCounter = 0;

    public static void ChangeCoinCounter(short addition) {
        coinCounter += addition;

        do {
            coinCounter -= 99;
            ChangeLifeCounter(1);
        }
        while (coinCounter > 99);
    }

    public static void ChangeLifeCounter(short addition) {
        lifeCounter += addition;

        if (lifeCounter > 99) {
            lifeCounter = 99;
        }
    }
}
