using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepScore : MonoBehaviour {

    public static int score { get; private set; }
    float lastEnemyKilledTime;
    int streakProgression;
    float streakExpiryTime = 1;
    
	void Start () {
        Enemy.OnDeathStatic += OnEnemyKilled;
        FindObjectOfType<Player>().OnDeath += OnPlayerDeath;
	}

    void OnEnemyKilled() {
        if (Time.time < lastEnemyKilledTime + streakExpiryTime) {
            streakProgression++;
        }
        else {
            streakProgression = 0;
        }
        lastEnemyKilledTime = Time.time;
        score += 5 + (int)Mathf.Pow(2, streakProgression);
    }

    void OnPlayerDeath() {
        Enemy.OnDeathStatic -= OnEnemyKilled;
    }
}
