using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour {

    public AudioClip mainTheme;
    public AudioClip menuTune;

    string sceneName;

    void Start() {
        OnLevelWasLoaded(0);
    }

    void OnLevelWasLoaded(int level) {
        string newSceneName = SceneManager.GetActiveScene().name;
        if (newSceneName != sceneName) {
            sceneName = newSceneName;
            Invoke("PlayMusic", .2f);
        }
    }

    void PlayMusic() {
        AudioClip clipToPlay = null;
        if (sceneName == "menu")
        {
            clipToPlay = menuTune;
        }
        else if (sceneName == "level0") {
            clipToPlay = mainTheme;
        }
        if (clipToPlay != null) {
            AudioManager.audioManager.PlayMusic(clipToPlay, 2);
            Invoke("PlayMusic", clipToPlay.length);
        }
    }
}
