using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScoreManager {

    const string HIGH_SCORE = "HighScore";

    public static void SaveScore(int score) {
        if (!PlayerPrefs.HasKey(HIGH_SCORE)) {
            PlayerPrefs.SetInt(HIGH_SCORE, 0);
        }
        if (PlayerPrefs.GetInt(HIGH_SCORE) < score) {
            PlayerPrefs.SetInt(HIGH_SCORE, score);
        }
    }

    public static int GetHighScore() {
        return PlayerPrefs.GetInt(HIGH_SCORE);
    }

    public static void ResetHighScore() {
        PlayerPrefs.SetInt(HIGH_SCORE, 0);
    }
}
