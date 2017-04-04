using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public enum AudioChannel {Master, Sfx, Music}

    public static AudioManager audioManager;

    Transform audioListener;
    Transform playerTransform;

    SoundLibrary soundLibrary;

    public float masterVolume { get; private set; }
    public float effectsVolume { get; private set; }
    public float musicVolume { get; private set; }

    AudioSource sfx2DSource;
    AudioSource[] musicTracks;
    int currentTrack;
        
    void Awake() {
        if (audioManager == null)  {
            audioManager = this;
            DontDestroyOnLoad(gameObject);
            soundLibrary = GetComponent<SoundLibrary>();
            musicTracks = new AudioSource[2];
            for (int i = 0; i < 2; i++)
            {
                GameObject newTrack = new GameObject("Music track " + (i + 1));
                musicTracks[i] = newTrack.AddComponent<AudioSource>();
                newTrack.transform.parent = transform;
            }

            GameObject newSfx2DSource = new GameObject("2D Sfx Source");
            sfx2DSource = newSfx2DSource.AddComponent<AudioSource>();
            newSfx2DSource.transform.parent = transform;

            audioListener = FindObjectOfType<AudioListener>().transform;
            if (FindObjectOfType<Player>() != null) {
                playerTransform = FindObjectOfType<Player>().transform;
            }
            
            masterVolume =  PlayerPrefs.GetFloat("master volume", 1);
            effectsVolume = PlayerPrefs.GetFloat("effects volume", 1);
            musicVolume = PlayerPrefs.GetFloat("music volume", 1);
        }
    }

    private void Update() {
        if (playerTransform != null) {
            audioListener.position = playerTransform.position;
        }
    }

    public void SetVolume(float volumePercent, AudioChannel channel) {
        switch (channel) {
            case AudioChannel.Master:
                masterVolume = volumePercent;
                break;
            case AudioChannel.Sfx:
                effectsVolume = volumePercent;
                break;
            case AudioChannel.Music:
                musicVolume = volumePercent;
                break;
        }

        musicTracks[0].volume = musicVolume * volumePercent;
        musicTracks[1].volume = musicVolume * volumePercent;

        PlayerPrefs.SetFloat("master volume", masterVolume);
        PlayerPrefs.SetFloat("effects volume", effectsVolume);
        PlayerPrefs.SetFloat("music volume", musicVolume);
        PlayerPrefs.Save();
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
        PlaySound(soundLibrary.GetClipByName(audioClipName), soundPosition);
    }

    public void PlaySound2D(string clipName) {
        sfx2DSource.PlayOneShot(soundLibrary.GetClipByName(clipName), effectsVolume * masterVolume);
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
