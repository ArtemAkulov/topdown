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

    Spawner spawner;

	void Start () {
        FindObjectOfType<Player>().OnDeath += OnGameOver;
	}

    private void Awake() {
        spawner = FindObjectOfType<Spawner>();
        spawner.OnNewWave += OnNewWave;
    }

    void OnNewWave(int waveNumber) {
        string[] numbers = { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten" };
        waveBannerNumber.text = "/// Wave # " + numbers[waveNumber];
        waveBannerEnemies.text = "Enemies to shoot := " + ((spawner.waves[waveNumber - 1].infiniteEnemies) ? "Infinite" : spawner.waves[waveNumber - 1].enemyCount + "");
        StopCoroutine("AnimateBanner");
        StartCoroutine("AnimateBanner");
               
    }

    void OnGameOver() {
        Cursor.visible = true;
        StartCoroutine(Fade(Color.clear, Color.black, 1));
        GameOverUI.SetActive(true);
    }

    IEnumerator AnimateBanner() {
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
            waveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(-235, 0, animationPercentage);
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

}