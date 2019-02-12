using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    const string BUTTON_ROTATE = "Rotate";
    const string BUTTON_RIGHT = "MoveRight";
    const string BUTTON_LEFT = "MoveLeft";
    const string BUTTON_DOWN = "MoveDown";
    const string BUTTON_PLAY_AGAIN = "PlayAgain";
    const string BUTTON_EXIT_GAME = "ExitGame";
    const string BUTTON_PAUSE_GAME = "PauseGame";

    [SerializeField] Board board = null;
    [SerializeField] Spawner spawner = null;
    [SerializeField] PanelManager panelManager = null;
    [Header("Input")]
    [SerializeField] float dropDownNormalSpeed = .5f;
    [SerializeField] float dropDownFastSpeed = .05f;
    [SerializeField] float keyRepeatStart = .5f;
    [SerializeField] float keyRepeatRate = .25f;
    [Header("Level Up")]
    [SerializeField] int levelUpAfterCompletedLines = 10;
    [SerializeField] float speedIncreaseFactorPerLevel = .1f;
    [Header("Score")]
    [SerializeField] int shapeLandScore = 10;
    [SerializeField] int[] fullLineScore = new int[4];
    [SerializeField] bool resetHighScore = false;

    Shape activeShape = null;
    SoundManager soundManager;
    float timeToDrop = 0f;
    int linesCompletedOverAll = 0;
    bool gameOver = false;
    bool fastDropRequired = false;
    float timeToNextKey = 0;
    float timeToRepeat = 0;
    bool keyRepeatStarted = false;
    int actualLevel = 1;
    int totalScore = 0;
    bool paused = false;

    void Start() {
        soundManager = GetComponent<SoundManager>();
        if (resetHighScore) {
            ScoreManager.ResetHighScore();
        }
    }

    void Update() {
        if (!gameOver && !paused) {
            if (!activeShape) {
                activeShape = spawner.SpawnShape();
            } else {
                DropDownOverTime();
                CheckForInput();
            }
        } else if (gameOver) {
            CheckForInputGameOver();
        } else if (paused) {
            CheckForInputGamePaused();
        }
    }

    public void PauseResumeGame() {
        paused = !paused;
        panelManager.ShowPausedPanel(paused);
    }

    void CheckForInput() {
        if (Input.GetButtonDown(BUTTON_ROTATE)) {
            activeShape.RotateLeft();
            if (!board.HasShapeValidPosition(activeShape)) {
                activeShape.RotateRight();
            }
        }
        if (Input.GetButton(BUTTON_LEFT) && Time.time > timeToRepeat && Time.time > timeToNextKey || Input.GetButtonDown(BUTTON_LEFT)) {
            if (!keyRepeatStarted) {
                timeToRepeat = Time.time + keyRepeatStart;
                keyRepeatStarted = true;
            }
            timeToNextKey = Time.time + keyRepeatRate;
            activeShape.MoveLeft();
            if (!board.HasShapeValidPosition(activeShape)) {
                activeShape.MoveRight();
            }
        }
        if (Input.GetButton(BUTTON_RIGHT) && Time.time > timeToRepeat && Time.time > timeToNextKey || Input.GetButtonDown(BUTTON_RIGHT)) {
            if (!keyRepeatStarted) {
                timeToRepeat = Time.time + keyRepeatStart;
                keyRepeatStarted = true;
            }
            timeToNextKey = Time.time + keyRepeatRate;
            activeShape.MoveRight();
            if (!board.HasShapeValidPosition(activeShape)) {
                activeShape.MoveLeft();
            }
        }
        if (Input.GetButtonDown(BUTTON_DOWN)) {
            timeToDrop = Time.time;
            fastDropRequired = true;
            DropDownOverTime();
        }
        if (Input.GetButtonUp(BUTTON_DOWN)) {
            fastDropRequired = false;
            DropDownOverTime();
        }
        if (Input.GetButtonUp(BUTTON_LEFT) || Input.GetButtonUp(BUTTON_RIGHT)) {
            keyRepeatStarted = false;
        }
        if (Input.GetButtonDown(BUTTON_PAUSE_GAME)) {
            PauseResumeGame();
        }
    }

    void CheckForInputGameOver() {
        if (Input.GetButtonDown(BUTTON_PLAY_AGAIN)) {
            SceneManager.LoadScene(0);
        }
        if (Input.GetButtonDown(BUTTON_EXIT_GAME)) {
            Application.Quit();
            Debug.Log("Quit Application requested");
        }
    }

    void CheckForInputGamePaused() {
        if (Input.GetButtonDown(BUTTON_PAUSE_GAME)) {
            PauseResumeGame();
        }
    }

    void DropDownOverTime() {
        if (Time.time > timeToDrop) {
            activeShape.MoveDown();
            timeToDrop = Time.time + (fastDropRequired ? dropDownFastSpeed : dropDownNormalSpeed);
            if (!board.HasShapeValidPosition(activeShape)) {
                SpawnNewShapeWhenPlaced();
            }
        }
    }

    void SpawnNewShapeWhenPlaced() {
        fastDropRequired = false;
        activeShape.MoveUp();
        soundManager.PlayClipShapeLands();
        totalScore += shapeLandScore;
        gameOver = board.IsGameOver(activeShape);
        if (!gameOver) {
            board.StoreShapeInGrid(activeShape);
            activeShape = spawner.SpawnShape();
            CheckForFullLines();
        } else {
            HandleGameOver();
        }
        panelManager.SetTotalScore(totalScore);
    }

    void HandleGameOver() {
        bool highScore = false;
        if (totalScore > ScoreManager.GetHighScore()) {
            highScore = true;
            ScoreManager.SaveScore(totalScore);
            soundManager.PlayNewHighScoreClip();
        }
        panelManager.HandleGameOver(totalScore, highScore);
        board.PutShapesToLayername("Default");
        soundManager.PlayGameOverClip();
    }

    void CheckForFullLines() {
        int linesCompleted = board.RemoveFullLines();
        if (linesCompleted > 0) {
            ManageCompletedLines(linesCompleted);
        }
    }

    void ManageCompletedLines(int linesCompleted) {
        for (int i = 1; i <= linesCompleted; i++) {
            linesCompletedOverAll++;
            panelManager.SetLinesToNumber(linesCompletedOverAll);
            CheckForLevelUp();
        }
        totalScore += fullLineScore[linesCompleted - 1];
        panelManager.ShowFlyingScore(fullLineScore[linesCompleted - 1], linesCompleted == 4 ? true : false);
    }

    void CheckForLevelUp() {
        if (linesCompletedOverAll % levelUpAfterCompletedLines == 0) {
            actualLevel++;
            panelManager.SetLevelToNumber(actualLevel);
            dropDownNormalSpeed *= 1f - actualLevel * speedIncreaseFactorPerLevel;
        }
    }
}
