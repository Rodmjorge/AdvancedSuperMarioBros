using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private static bool pause;

    public static uint lifeCounter = 5;
    public static uint coinCounter;
    public static ulong score;
    public static ushort timer { get { return LevelLoader.LevelSettings.GetTimer(); } }

    public new static Camera camera { get { return FindObjectOfType<Camera>(); } }
    public static LevelManager levelManager;
    public static GameObject UI;
    
    private void Awake()
    {
        levelManager = this;

        UI = SetCamera((GameObject)Instantiate(Resources.Load("UI/UI")));
        AudioManager.CreateAudioManager();
    }

    private void Start()
    {
        Application.targetFrameRate = 350;
        LevelLoader.LoadLevel("test");

        AudioManager.PlayMusic("overworld_music");
    }



    public static GameObject SetCamera(GameObject GO)
    {
        Canvas canv = GO.GetComponent<Canvas>();

        canv.worldCamera = camera;
        canv.planeDistance = camera.farClipPlane;

        return canv.gameObject;
    }
    public static void StatusOfScene(bool pause, bool resetLevel = false)
    {
        ChangePauseState(pause);
        if (resetLevel) LevelLoader.ResetLevel();
    }

    public static void ChangePauseState(bool b) { pause = b; }
    public static bool IsPaused() { return pause; }
}