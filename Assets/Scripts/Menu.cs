using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

    public GameObject mainMenuHolder;
    public GameObject optionsMenuHolder;

    public Slider[] volumeSliders;
    public Toggle[] resolutionToggles;
    public Toggle fullscreenToggle;
    public int[] screenResolutionIndexes;
    int activeScreenResolutionIndex;

    void Start() {
        activeScreenResolutionIndex = PlayerPrefs.GetInt("screen resolution index");
        bool fullscreen = PlayerPrefs.GetInt("fullscreen") == 1;
        volumeSliders[0].value = AudioManager.audioManager.masterVolume;
        volumeSliders[1].value = AudioManager.audioManager.effectsVolume;
        volumeSliders[2].value = AudioManager.audioManager.musicVolume;
        for (int i = 0; i < resolutionToggles.Length; i++) {
            resolutionToggles[i].isOn = i == activeScreenResolutionIndex;
        }
        fullscreenToggle.isOn = fullscreen;
    }

    public void Play() {
        SceneManager.LoadScene("level0");
    }

    public void EnterOptions() {
        mainMenuHolder.SetActive(false);
        optionsMenuHolder.SetActive(true);
    }

    public void Quit() {
        print("Slamming button");
        Application.Quit(); 
    }

    public void EnterMain() {
        optionsMenuHolder.SetActive(false);
        mainMenuHolder.SetActive(true);
    }

    public void SetScreenResolution(int i) {
        if (resolutionToggles[i].isOn) {
            float aspectRatio = 16 / 9f;
            activeScreenResolutionIndex = i;
            Screen.SetResolution(screenResolutionIndexes[i], (int)(screenResolutionIndexes[i] / aspectRatio), false);
            PlayerPrefs.SetInt("screen resolution index", activeScreenResolutionIndex);
            PlayerPrefs.Save();
        }
    }

    public void ToggleFullscreen(bool fullscreen) {
        for (int i = 0; i < resolutionToggles.Length; i++) {
            resolutionToggles[i].interactable = !fullscreen;
        }

        if (fullscreen)
        {
            Resolution[] allResolutions = Screen.resolutions;
            Resolution maxResolution = allResolutions[allResolutions.Length - 1];
            Screen.SetResolution(maxResolution.width, maxResolution.height, true);
        } else {
            SetScreenResolution(activeScreenResolutionIndex);
        }

        PlayerPrefs.SetInt("fullscreen", ((fullscreen) ? 1 : 0));
        PlayerPrefs.Save();
    }

    public void SetMasterVolume(float volume) {
        AudioManager.audioManager.SetVolume(volume, AudioManager.AudioChannel.Master);
    }

    public void SetEffectsVolume(float volume)
    {
        AudioManager.audioManager.SetVolume(volume, AudioManager.AudioChannel.Sfx);
    }

    public void SetMusicVolume(float volume)
    {
        AudioManager.audioManager.SetVolume(volume, AudioManager.AudioChannel.Music);
    }

}

