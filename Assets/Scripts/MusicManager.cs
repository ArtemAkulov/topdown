using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    public AudioClip mainTheme;
    public AudioClip menuTune;

    void Start() {
        AudioManager.audioManager.PlayMusic(menuTune, 2);
    }
    
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space)) {
            AudioManager.audioManager.PlayMusic(mainTheme, 2);
        }
	}
}
