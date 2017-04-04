using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameUI : MonoBehaviour {

    public Image fadePlane;
    public GameObject GameOverUI;

    public RectTransform waveBanner;
    public Text waveBannerNumber;
    public Text waveBannerEnemies;

    public RectTransform waveCompleteBanner;
    public Text waveCompleteMessage1;
    public Text waveCompleteMessage2;

    public Text scoreUI;
    public Text gameOverScoreUI;
    public RectTransform healthBar;

    Spawner spawner;
    Player player;

	void Start () {
        player = FindObjectOfType<Player>();
        player.OnDeath += OnGameOver;
	}

    void Awake() {
        spawner = FindObjectOfType<Spawner>();
        spawner.OnNewWave += OnNewWave;
        spawner.OnEveryoneIsDead += OnEveryoneIsDead;
    }

    void Update() {
        scoreUI.text = KeepScore.score.ToString("D6");
        float healthPercent = 0;
        if (player != null) {
            healthPercent = player.health / player.startingHealth;
        }
        healthBar.localScale = new Vector3(healthPercent, 1, 1);
    }

    void OnNewWave(int waveNumber) {
        string[] numbers = { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten" };
        waveBannerNumber.text = "/// Wave # " + numbers[waveNumber];
        waveBannerEnemies.text = "Enemies to shoot := " + ((spawner.waves[waveNumber - 1].infiniteEnemies) ? "Infinite" : spawner.waves[waveNumber - 1].enemyCount + "");
        IEnumerator startWave = AnimateBanner(waveBanner);
        StopCoroutine(startWave);
        StartCoroutine(startWave);
    }

    void OnEveryoneIsDead() {
        AudioManager.audioManager.PlaySound2D("Wave Complete");
        IEnumerator completeWave = AnimateBanner(waveCompleteBanner);
        StopCoroutine(completeWave);
        StartCoroutine(completeWave);
    }

    void OnGameOver() {
        Cursor.visible = true;
        gameOverScoreUI.text = scoreUI.text;
        StartCoroutine(Fade(Color.clear, new Color(1, 1, 1, 0.8f), 1));
        scoreUI.gameObject.SetActive(false);
        healthBar.transform.parent.gameObject.SetActive(false);
        GameOverUI.SetActive(true);
    }

    IEnumerator AnimateBanner(RectTransform banner) {
        float animationSpeed = 2.75f;
        float delayTime = 1.25f;
        float noMoreDelays = Time.time + 1 / animationSpeed + delayTime;
        float animationPercentage = 0;
        int animationDirection = 1;

        while (animationPercentage >= 0) {
            animationPercentage += Time.deltaTime * animationSpeed * animationDirection;
            if (animationPercentage >= 1) {
                animationPercentage = 1;
                if (Time.time > noMoreDelays) {
                    animationDirection = -1;
                }
            }
            banner.anchoredPosition = Vector2.up * Mathf.Lerp(-235, 0, animationPercentage);
            yield return null;
        }
    }
    
    IEnumerator Fade(Color fadeFrom, Color fadeInto, float fadeInterval) {
        float fadeSpeed = 1 / fadeInterval;
        float fadePercentage = 0;

        while (fadePercentage < 1) {
            fadePercentage += Time.deltaTime * fadeSpeed;
            fadePlane.color = Color.Lerp(fadeFrom, fadeInto, fadePercentage);
            yield return null;
        }
    }

    // Restarting

    public void StartNewGame() {
        SceneManager.LoadScene("level0");
    }

    public void ReturnToMenu() {
        SceneManager.LoadScene("menu");
    }
}