using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    private static bool pause;

    public static ulong lifeCounter = 5;
    public static ulong coinCounter;
    public static ulong score;
    public static double timer;

    public readonly static int[] counterLengths = new int[] {
        2,   //life counter
        2,   //coin counter
        9,   //score
        3    //timer
    };

    public static LevelManager levelManager;
    
    private void Awake()
    {
        levelManager = this;

        AudioManager.CreateAudioManager();
        ScoreManager.CreateScoreText(new Vector3(0f, 0f, 0f));
        UIManager.CreateUI();
    }

    private void Start()
    {
        LevelLoader.LoadLevel("test");
        LevelLoader.PlayThemedMusic();

        StartCoroutine(TimerDecrease());
    }

    public static void Add1UP(uint i, bool play1UPSound = true)
    {
        lifeCounter = MaxOut(lifeCounter + i, 0);
        if (play1UPSound) AudioManager.PlayAudio("1UP");
    }
    public static void AddCoin(uint i)
    {
        coinCounter += i;
        uint dividedBy100 = (uint)Math.Floor(coinCounter / 100f);

        if (dividedBy100 > 0) {
            Add1UP(dividedBy100);

            string s = coinCounter.ToString();
            coinCounter = ulong.Parse(s.Substring(s.Length - 2, 2));
        }
    }

    public static void AddToScore(string s)
    {
        if (ulong.TryParse(s, out ulong scoreAdder))
            AddToScore(scoreAdder);
    }
    public static void AddToScore(ulong ul)
    {
        score = MaxOut(score + ul, 2);
    }

    public static ulong MaxOut(ulong ul, int index)
    {
        ulong ulMax = ulong.Parse(string.Join(',', Enumerable.Repeat("9", counterLengths[index]).ToArray()).Replace(",", ""));
        if (ul > ulMax)
            return ulMax;

        return ul;
    }

    private IEnumerator TimerDecrease()
    {
        timer = MaxOut(LevelLoader.LevelSettings.GetTimer() + 1, 3);

        bool hasPlayedHurryUpMusic = false;
        while (true) {
            if (!IsPaused()) {
                timer -= Time.fixedDeltaTime;

                if (timer <= 101f && !hasPlayedHurryUpMusic) {
                    StartCoroutine(AudioManager.RunWhenMusicStops(HurryUpMusic(), "time_is_running_out"));
                    hasPlayedHurryUpMusic = true;
                }

                if (timer <= 0f) {
                    timer = 0f;

                    foreach (Player player in FindObjectsOfType<Player>())
                        player.HitPlayer(this.gameObject, player);
                    yield break;
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }

    public IEnumerator HurryUpMusic()
    {
        AudioManager.PlayMusic(AudioManager.GetHurryUpMusic());
        yield break;
    }

    public static void StatusOfScene(bool pause, bool resetLevel = false)
    {
        ChangePauseState(pause);
        if (resetLevel) LevelLoader.ResetLevel();
    }

    public static void ChangePauseState(bool b) { pause = b; }
    public static bool IsPaused() { return pause; }




    public class ScoreManager
    {
        private static GameObject scoreUI = null;
        private static Canvas scoreUICanvas = null;

        private readonly static uint[] scores = new uint[] { 200, 400, 800, 1000, 2000, 4000, 8000 };

        private GameObject gameObject;
        private int scoreIndex;

        public ScoreManager(GameObject gameObject)
        {
            this.gameObject = gameObject;
            ResetIndex();
        }

        internal static Canvas CreateScoreManager()
        {
            scoreUI = new GameObject() { name = "ScoreUI" };

            Canvas canvas = scoreUI.AddComponent<Canvas>();
            CanvasScaler canvasScaler = scoreUI.AddComponent<CanvasScaler>();
            GraphicRaycaster graphicRaycaster = scoreUI.AddComponent<GraphicRaycaster>();

            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = UIManager.camera;
            canvas.sortingLayerName = "UI";

            RectTransform rect = scoreUI.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(1000f, 1000f);
            rect.localScale = new Vector3(GetLocalscaleUI().x, GetLocalscaleUI().y, 1f);

            scoreUICanvas = canvas;
            return canvas;
        }
        internal static TextMeshProUGUI CreateScoreText(Vector2 pos)
        {
            CreateIfNull();

            GameObject score = new GameObject() { name = "score" };
            score.transform.position = pos;
            score.transform.parent = scoreUI.transform;
            score.transform.localScale *= GetLocalscaleUI();

            TextMeshProUGUI scoreTxt = score.AddComponent<TextMeshProUGUI>();
            scoreTxt.font = Resources.Load<TMP_FontAsset>("Fonts/PixelEmulator-xq08 SDF");
            scoreTxt.fontStyle = FontStyles.Bold;
            scoreTxt.fontSize = 4;
            scoreTxt.characterSpacing = -6;
            scoreTxt.horizontalAlignment = HorizontalAlignmentOptions.Center;
            scoreTxt.verticalAlignment = VerticalAlignmentOptions.Middle;
            scoreTxt.UpdateFontAsset();

            return scoreTxt;
        }

        public void SetScore(string s, bool addToScore, Vector2 pos, Color? color = null)
        {
            TextMeshProUGUI text = CreateScoreText(pos);

            text.text = s;
            text.color = (color == null) ? Color.white : color.Value;

            text.StartCoroutine(ScoreAnim(text.gameObject));
            if (addToScore) AddToScore(s);
        }
        public void SetScore(ulong ul, bool addToScore, Vector2 pos, Color? color = null)
        {
            SetScore(ul.ToString(), addToScore, pos, color);
        }

        public void AddIndex(int i, bool spawnScore, bool get1UP = true, Vector2? pos = null, Color? color = null)
        {
            scoreIndex += i;

            if (spawnScore) {
                string s = (scoreIndex >= scores.Length) ? (get1UP ? "1UP" : scores[scores.Length - 1].ToString()) : scores[scoreIndex].ToString();
                bool got1UP = (s == "1UP");

                Color color0 = (color == null) ? (got1UP ? Get1UPColor() : Color.white) : color.Value;
                if (got1UP) Add1UP(1);

                SetScore(s, true, (pos == null) ? gameObject.transform.position : pos.Value, color0);
            }
        }
        public void ResetIndex()
        {
            scoreIndex = -1;
        }


        private IEnumerator ScoreAnim(GameObject score)
        {
            float f = 0.05f;
            int g = 0;

            while (true) {
                if (!IsPaused()) {
                    f -= 0.002f;
                    score.transform.position += new Vector3(0f, f, 0f);

                    if (f <= 0) {
                        f = 0.002f;
                        g++;

                        if (g >= 25) {
                            Destroy(score);
                            yield break;
                        }
                    }
                }

                yield return new WaitForFixedUpdate();
            }
        }


        private static Vector2 GetLocalscaleUI()
        {
            return new Vector2(0.1f, 0.1f);
        }
        private static void CreateIfNull() 
        { 
            if (scoreUI == null) scoreUI = CreateScoreManager().gameObject; 
        }

        internal static Color Get1UPColor() { return new Color(0f, 1f, 0f); }
    }
}