﻿using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager UI;
    public new static Camera camera { get { return FindObjectOfType<Camera>(); } }


    internal TextMeshProUGUI lifeCounter { get { return GetText(transform, "life_counter", true, "life_text"); } }
    internal TextMeshProUGUI coinCounter { get { return GetText(transform, "coin_counter", true, "coin_text"); } }
    internal TextMeshProUGUI scoreCounter { get { return GetText(transform, "scoreboard_text", false); } }
    internal TextMeshProUGUI timerCounter { get { return GetText(transform, "timer_counter", true, "timer_text"); } }

    private int[] lengths = new int[4]; //1 - life, 2 - coin, 3 - score, 4 - timer

    public static GameObject CreateUI()
    {
        GameObject UIPrefab = (GameObject)Instantiate(Resources.Load("UI/UI"));
        UI = UIPrefab.AddComponent<UIManager>();

        UI.SetLengths();
        UI.SetCamera();

        return UIPrefab;
    }

    private void Update()
    {
        lifeCounter.text = MaxOut(lengths[0], LevelManager.lifeCounter.ToString());
        coinCounter.text = MaxOut(lengths[1], LevelManager.coinCounter.ToString());
        scoreCounter.text = MaxOut(lengths[2], LevelManager.score.ToString());
        timerCounter.text = MaxOut(lengths[3], Math.Floor(LevelManager.timer).ToString());
    }

    public GameObject SetCamera()
    {
        Canvas canv = gameObject.GetComponent<Canvas>();

        canv.worldCamera = camera;
        canv.planeDistance = camera.farClipPlane;

        return canv.gameObject;
    }

    public int[] SetLengths()
    {
        int[] parses = new int[] { int.Parse(lifeCounter.text), int.Parse(coinCounter.text), int.Parse(scoreCounter.text), int.Parse(timerCounter.text) };

        for (int i = 0; i < lengths.Length; i++)
            lengths[i] = parses[i];

        return lengths;
    }

    public string MaxOut(int i, string s)
    {
        return string.Join(',', Enumerable.Repeat("0", i).ToArray()).Replace(",", "").Remove(0, s.Length) + s;
    }

    private TextMeshProUGUI GetText(Transform trans, string name, bool isChild, string childName = "")
    {
        Transform trans0 = trans.Find(name);
        TextMeshProUGUI text = trans0.GetComponent<TextMeshProUGUI>();

        return isChild ? GetText(trans0, childName, false) : text;
    }
}