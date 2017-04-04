using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLibrary : MonoBehaviour {

    public SoundGroup[] soundGroups;

    Dictionary<string, AudioClip[]> soundGroupDictionary = new Dictionary<string, AudioClip[]>();

    void Awake() {
        foreach (SoundGroup soundGroup in soundGroups) {
            soundGroupDictionary.Add(soundGroup.groupID, soundGroup.groupClips);
        }
    }

    public AudioClip GetClipByName(string clipName) {
        if (soundGroupDictionary.ContainsKey(clipName)) {
            AudioClip[] sounds = soundGroupDictionary[clipName];
            return sounds[Random.Range(0, sounds.Length)];
        }
        return null;
    }

    [System.Serializable]
    public class SoundGroup {
        public string groupID;
        public AudioClip[] groupClips;
    }
}
