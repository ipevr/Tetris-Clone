using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour
{
    const string NEW_HIGH_SCORE_TEXT = "NEW HIGH SCORE!";
    const string HIGH_SCORE_TEXT = "HIGH SCORE:";

    [SerializeField] GameObject panelGameOver = null;
    [SerializeField] GameObject panelPaused = null;
    [SerializeField] Text linesText = null;
    [SerializeField] Text levelText = null;
    [SerializeField] Text scoreText = null;
    [SerializeField] Text highScoreText = null;
    [SerializeField] Text highScoreLabel = null;
    [SerializeField] Text flyingScoreText = null;
    [SerializeField] Text pausedHighScoreText = null;
    [SerializeField] float timeShowingFlyingScore = 2f;
    [SerializeField] float flyingScoreEndPosition = 300f;
    [SerializeField] Color[] flyingScoreColors = new Color[] { };
    [SerializeField] int noramlFontSizeFlyingScore = 60;
    [SerializeField] int superBonusFontSizeFlyingScore = 80;
    [SerializeField] float superBonusChangeColorInterval = 0.2f;

    SoundManager soundManager;
    Board board;
    Vector3 originalPositionFlyingScore = Vector3.zero;
    

    void Start() {
        soundManager = FindObjectOfType<SoundManager>();
        board = FindObjectOfType<Board>();
        panelGameOver.SetActive(false);
        panelPaused.SetActive(false);
        linesText.text = "0";
        levelText.text = "1";
        pausedHighScoreText.text = ScoreManager.GetHighScore().ToString();
        originalPositionFlyingScore = flyingScoreText.transform.position;
        flyingScoreText.fontSize = noramlFontSizeFlyingScore;
        flyingScoreText.enabled = false;
    }

    public void ShowPausedPanel(bool status) {
        panelPaused.SetActive(status);
        if (status) {
            board.PutShapesToLayername("Default");
        } else {
            board.PutShapesToLayername("Sprites");
        }
    }

    public void HandleGameOver(int score, bool isHighScore) {
        panelGameOver.SetActive(true);
        highScoreLabel.text = isHighScore ? NEW_HIGH_SCORE_TEXT : HIGH_SCORE_TEXT;
        highScoreText.text = ScoreManager.GetHighScore().ToString();
    }

    public void SetLinesToNumber (int number) {
        linesText.text = number.ToString();
    }

    public void SetLevelToNumber (int number) {
        levelText.text = number.ToString();
    }

    public void SetTotalScore(int totalScore) {
        scoreText.text = totalScore.ToString();
    }

    public void ShowFlyingScore(int score, bool superBonus) {
        StopAllCoroutines();
        SetFlyingScoreProperties(score, superBonus);
        StartCoroutine(ShowFlyingScoreOverTime(score, superBonus));
    }

    public void UpdateHighScore() {
        int highScore = ScoreManager.GetHighScore();
        highScoreText.text = highScore.ToString();
        pausedHighScoreText.text = highScore.ToString();
    }

    void SetFlyingScoreProperties(int score, bool superBonus) {
        flyingScoreText.transform.position = originalPositionFlyingScore;
        flyingScoreText.enabled = true;
        if (superBonus) {
            flyingScoreText.fontSize = superBonusFontSizeFlyingScore;
        } else {
            flyingScoreText.fontSize = noramlFontSizeFlyingScore;
        }
        flyingScoreText.text = "+" + score.ToString();
        flyingScoreText.color = flyingScoreColors[UnityEngine.Random.Range(0, flyingScoreColors.Length)];
    }

    IEnumerator ShowFlyingScoreOverTime(float score, bool superBonus) {
        float elapsedTime = 0f;
        float nextTimeToChangeColor = superBonusChangeColorInterval;
        Vector3 endPos = new Vector3(originalPositionFlyingScore.x, originalPositionFlyingScore.y + flyingScoreEndPosition);
        while (elapsedTime < timeShowingFlyingScore) {
            flyingScoreText.transform.position = Vector3.Lerp(originalPositionFlyingScore, endPos, elapsedTime / timeShowingFlyingScore);
            elapsedTime += Time.deltaTime;
            if (superBonus && elapsedTime > nextTimeToChangeColor) {
                flyingScoreText.color = flyingScoreColors[UnityEngine.Random.Range(0, flyingScoreColors.Length)];
                nextTimeToChangeColor += superBonusChangeColorInterval;
            }
            yield return new WaitForEndOfFrame();
        }
        flyingScoreText.transform.position = originalPositionFlyingScore;
        flyingScoreText.enabled = false;
    }
}
