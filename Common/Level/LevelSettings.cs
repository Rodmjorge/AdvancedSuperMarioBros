using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSettings
{
    public static float gravity = 5;
    public static string theme = "_" + "overworld";
    public static bool playerDied = default;
    public static bool stopEverything = default; 
    public static ushort timer = 300;

    public static float GetGravity() {
        return gravity;
    }

    public static string GetLevelTheme() {
        return theme;
    }

    public static Vector3 ShiftToPosition(Transform transf, float addition) {

        int[] i = new int[] { Mathf.RoundToInt(transf.position.x), Mathf.RoundToInt(transf.position.y) };
        return new Vector3(i[0] + addition, i[1] + addition, 0f);
    }

    public static void ResetSettings()
    {
        playerDied = false;
    }
}
