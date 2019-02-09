using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour
{
    [SerializeField] GameObject panelGameOver = null;
    [SerializeField] Text linesText = null;
    [SerializeField] Text levelText = null;
    [SerializeField] Text scoreText = null;
    [SerializeField] Text flyingScoreText = null;
    [SerializeField] float timeShowingFlyingScore = 2f;
    [SerializeField] float flyingScoreEndPosition = 300f;
    [SerializeField] Color[] flyingScoreColors;

    Vector3 originalPositionFlyingScore = Vector3.zero;

    void Start() {
        panelGameOver.SetActive(false);
        linesText.text = "0";
        levelText.text = "1";
        originalPositionFlyingScore = flyingScoreText.transform.position;
        flyingScoreText.enabled = false;
    }

    public void ActivatePanelGameOver() {
        panelGameOver.SetActive(true);
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

    public void ShowFlyingScore(int score) {
        StopAllCoroutines();
        flyingScoreText.transform.position = originalPositionFlyingScore;
        flyingScoreText.enabled = true;
        flyingScoreText.text = "+" + score.ToString();
        flyingScoreText.color = flyingScoreColors[UnityEngine.Random.Range(0, flyingScoreColors.Length)];
        StartCoroutine(ShowFlyingScoreOverTime(score));
    }

    IEnumerator ShowFlyingScoreOverTime(float score) {
        float elapsedTime = 0f;
        Vector3 endPos = new Vector3(originalPositionFlyingScore.x, originalPositionFlyingScore.y + flyingScoreEndPosition);
        while (elapsedTime < timeShowingFlyingScore) {
            flyingScoreText.transform.position = Vector3.Lerp(originalPositionFlyingScore, endPos, elapsedTime / timeShowingFlyingScore);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        flyingScoreText.transform.position = originalPositionFlyingScore;
        flyingScoreText.enabled = false;
    }
}
