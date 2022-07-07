using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public static class LevelLoader
{
    public readonly static string levelPath = "Assets/Resources/Levels/";
    private static List<int> numberIDs = new List<int>();

    public static GameObject actorParent;

    public static void LoadLevel(string path, int chunkOrder = 0, int chunk = 100)
    {
        string[] lines = File.ReadAllLines(levelPath + path + ".lvl");

        bool b = (chunkOrder == 0);
        if (b) {
            actorParent = new GameObject() { name = "Actors" };

            string line = lines[0];
            CheckLine("id=player;" + line, 1, true);
        }

        int j = chunk * (chunkOrder + 1);
        for (int i = b ? 1 : 0; i < ((j < lines.Length) ? j : lines.Length); i++) {
            int lineNumber = i + 1;
            string line = lines[i];

            if (CheckForID(line) == false) {
                CheckLine(line, lineNumber);
            }
        }
    }

    public static void ResetLevel() { numberIDs.Clear(); }

    private static GameObject CheckLine(string line, int lineNumber, bool levelSettings = false, bool insideBrackets = false, ActorRegistry.ActorSettings.CreatedActorTypes type = ActorRegistry.ActorSettings.CreatedActorTypes.None, float? time = null)
    {
        string u = Regex.Replace(line, @"\[(?>[^[\]]+|(?<o>)\[|(?<-o>)\])*(?(o)(?!))\]|(;)", x => x.Groups[1].Success ? "§" : x.Value);
        string[] seperatedCommas = u.Split('§');

        GameObject GO = null;
        foreach (string s in seperatedCommas) {
            string[] t = s.Split(new[] { '=' }, 2);

            switch (t[0]) {
                case "idNumber":
                    if (!insideBrackets) numberIDs.Add(int.Parse(t[1]));
                    break;

                case "id":
                    GO = ActorRegistry.SetActor(t[1], null, null, type, time).gameObject;
                    break;

                case "pos":
                    GO.transform.position = CreateVariable<Vector3>(t[1]);
                    break;

                case "size":
                    GO.transform.localScale = CreateVariable<Vector3>(t[1], "", "", default, 1);
                    break;

                default:
                    if (levelSettings) {
                        LevelSettings.LoadSettings(t, lineNumber);
                        break;
                    }
                    else {
                        Actor actor = GO.GetComponent<Actor>();
                        actor.DataLoaded(t[1], t[0]);
                    }

                    break;
            }
        }

        return GO;
    }

    public static Actor CheckLineInBrackets(string bracketsAndInside, GameObject gameObject, bool changeRelativePos = true, int? changeSortingOrder = null, ActorRegistry.ActorSettings.CreatedActorTypes type = ActorRegistry.ActorSettings.CreatedActorTypes.None, float? time = null)
    {
        if (bracketsAndInside.StartsWith("[") && bracketsAndInside.EndsWith("]")) {
            bracketsAndInside = bracketsAndInside.Remove(bracketsAndInside.Length - 1, 1).Remove(0, 1);

            GameObject GO = CheckLine(bracketsAndInside, 0, false, true, type, time);
            Actor actor = GO.GetComponent<Actor>();

            GO.transform.position = changeRelativePos ? gameObject.transform.position : GO.transform.position;
            actor.spriteR.sortingOrder = (changeSortingOrder != null) ? changeSortingOrder.Value : actor.spriteR.sortingOrder;

            return (GO != null) ? GO.GetComponent<Actor>() : null;
        }

        return null;
    }
    public static GameObject CheckLineInBracketsGO(string bracketsAndInside, GameObject GO, bool changeRelativePos = true, int? changeSortingOrder = null, ActorRegistry.ActorSettings.CreatedActorTypes type = ActorRegistry.ActorSettings.CreatedActorTypes.None, float? time = null)
    {
        Actor actor = CheckLineInBrackets(bracketsAndInside, GO, changeRelativePos, changeSortingOrder, type, time);
        return (actor != null) ? actor.gameObject : null;
    }

    private static bool? CheckForID(string s)
    {
        if (int.TryParse(s.Split(';').Where(x => x.StartsWith("idNumber")).ToArray()[0].Split('=')[1], out int i)) {
            return numberIDs.Contains(i);
        }

        return null;
    }

    private static bool IsNotEqual(string s, string t)
    {
        if (s != "" && t != "") {
            return s != t;
        }

        return false;
    }

    public static bool RandomBoolean() { return new System.Random().NextDouble() > 0.5f; }

    public static T CreateVariable<T>(string s, string beforeEqual = "", string whatItHasToBe = "", T t = default, float integerValue = 0f)
    {
        if (IsNotEqual(beforeEqual, whatItHasToBe)) return t;

        object value = null;
        try {
            Type type = t.GetType();

            if (type == typeof(Vector2)) {
                Vector3 v3 = CreateVariable<Vector3>(s);
                value = new Vector2(v3.x, v3.y);
            }
            else if (type == typeof(Vector3)) {
                float[] f = CreateArrayOfDot(s).Select(y => CreateVariable<float>(y)).ToArray();
                value = new Vector3(f[0], f[1], (f.Length > 2) ? f[2] : integerValue);
            }

            else if (type == typeof(float))
                value = float.Parse(s.Replace('.', ','));
            else if (type == typeof(byte))
                value = (byte)CreateVariable<float>(s);
            else if (type == typeof(int))
                value = (int)CreateVariable<float>(s);
            else if (type == typeof(ushort))
                value = (ushort)CreateVariable<int>(s);

            else if (type == typeof(ushort[]))
                value = CreateArrayOf(s).Select(x => CreateVariable<ushort>(x)).ToArray();

            else if (type == typeof(bool))
                value = bool.Parse(s.ToLower());
        }
        catch {
            value = s;
        }

        return (T)value;
    }

    private static string[] CreateArrayOf(string s, char seperator = ',')
    {
        return s.Split(seperator);
    }
    private static string[] CreateArrayOfDot(string s, char seperator = ',')
    {
        return CreateArrayOf(s, seperator).Select(x => x.Replace('.', ',')).ToArray();
    }

    public static string PlayThemedMusic(bool isHurryUp = false)
    {
        string theme = LevelSettings.GetTheme();

        AudioManager.PlayMusic($"{ theme }_music{ (isHurryUp ? "_hurryUp" : string.Empty) }");
        return theme;
    }


    public static class LevelSettings
    {
        private static string theme = "overworld";
        private static ushort timer = 300;

        public static bool[] triggers = new bool[ushort.MaxValue];

        internal static void LoadSettings(string[] equalBreak, int lineNumber)
        {
            switch(equalBreak[0]) {
                case "theme":
                    theme = equalBreak[1];
                    break;

                case "triggers": //triggers that activate upon starting the level
                    ushort[] usArr = equalBreak[1].Split(',').Select(x => CreateVariable<ushort>(x)).ToArray();

                    foreach (ushort us in usArr)
                        SetTrigger(us, true);

                    break;

                case "timer":
                    timer = CreateVariable<ushort>(equalBreak[1]);
                    break;
            }
        }

        public static bool GetTrigger(ushort triggerID) { return triggers[triggerID - 1]; }
        public static void SetTrigger(ushort triggerID, bool b, bool setNoMatterWhat = false)
        {
            int i = triggerID - 1;
            Actor[] actor = GameObject.FindObjectsOfType<Actor>().Where(x => x.targetIDSet == triggerID).ToArray();

            if (actor.Where(x => b ? x.targetIDBool : !x.targetIDBool).ToArray().Length == actor.Length || setNoMatterWhat)
                triggers[i] = b;
        }

        internal static string GetTheme() { return theme; }
        internal static ushort GetTimer() { return timer; }
    }

    public enum TransformPos
    {
        X,
        Y,
        Z
    }
}