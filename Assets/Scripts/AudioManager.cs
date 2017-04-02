using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public static AudioManager audioManager;

    Transform audioListener;
    Transform playerTransform;

    float masterVolume = 0.5f;
    float effectsVolume = 1;
    float musicVolume = 0.05f;

    AudioSource[] musicTracks;
    int currentTrack;
        
    void Awake() {
        audioManager = this;
        musicTracks = new AudioSource[2];
        for (int i = 0; i < 2; i++) {
            GameObject newTrack = new GameObject("Music track " + (i + 1));
            musicTracks[i] = newTrack.AddComponent<AudioSource>();
            newTrack.transform.parent = transform;
        }
        audioListener = FindObjectOfType<AudioListener>().transform;
        playerTransform = FindObjectOfType<Player>().transform;
    }

    private void Update() {
        if (playerTransform != null) {
            audioListener.position = playerTransform.position;
        }
    }
 
    public void PlayMusic(AudioClip musicTrack, float fadeDuration = 1) {
        currentTrack = 1 - currentTrack;
        musicTracks[currentTrack].clip = musicTrack;
        musicTracks[currentTrack].Play();
        StartCoroutine(MusicCrossFade(fadeDuration));
    }

    public void PlaySound(AudioClip audioClip, Vector3 soundPosition) {
        if (audioClip != null) {
            AudioSource.PlayClipAtPoint(audioClip, soundPosition, effectsVolume * masterVolume);
        }
    }

    public void PlaySound(string audioClipName, Vector3 soundPosition) {

    }
        IEnumerator MusicCrossFade(float fadeDuration) {
        float fadeComplete = 0;
        while (fadeComplete < 1) {
            fadeComplete += Time.deltaTime / fadeDuration;
            musicTracks[currentTrack].volume = Mathf.Lerp(0, musicVolume * masterVolume, fadeComplete);
            musicTracks[1 - currentTrack].volume = Mathf.Lerp(musicVolume * masterVolume, 0, fadeComplete);
            yield return null;
        }
    }
}
