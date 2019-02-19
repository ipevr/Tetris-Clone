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
    const string BUTTON_TOGGLE_ROTATION = "ToggleRotation";
    const string BUTTON_TOGGLE_MUSIC = "ToggleMusic";
    const string BUTTON_TOGGLE_SOUND = "ToggleSound";

    [SerializeField] Board board = null;
    [SerializeField] Spawner spawner = null;
    [SerializeField] PanelManager panelManager = null;
    [Header("Input")]
    [SerializeField] float startDropDownSpeed = 2f;
    [SerializeField] float dropDownFastSpeed = 100f;
    [SerializeField] float keyRepeatStart = .5f;
    [SerializeField] float keyRepeatRate = .25f;
    [SerializeField] IconToggle iconToggleRotation = null;
    [Header("Level Up")]
    [SerializeField] int levelUpAfterCompletedLines = 10;
    [SerializeField] float speedIncreasePerLevel = .5f;
    [Header("Score")]
    [SerializeField] int shapeLandScore = 10;
    [SerializeField] int[] fullLineScore = new int[4];
    [SerializeField] bool resetHighScore = false;

    Shape activeShape = null;
    SoundManager soundManager;
    float timeToDrop = 0f;
    int linesCompletedOverAll = 0;
    bool gameOver = false;
    float timeToRepeat = 0;
    bool keyRepeatStarted = false;
    int actualLevel = 1;
    int totalScore = 0;
    bool paused = false;
    bool rotationLeft = true;
    bool fastDropAllowed = true;
    float dropDownNormalTime = 0;
    float dropDownFastTime = 0;
    Direction swipeDirection = Direction.none;
    Direction swipeEndDirection = Direction.none;
    
    void Start() {
        soundManager = FindObjectOfType<SoundManager>();
        dropDownNormalTime = 1f / startDropDownSpeed;
        dropDownFastTime = 1f / dropDownFastSpeed;
        if (resetHighScore) {
            ScoreManager.ResetHighScore();
        }
    }

    void Update() {
        if (!gameOver && !paused) {
            if (!activeShape) {
                activeShape = spawner.SpawnShape();
            } else {
                MoveDown(false);
                CheckForInput();
            }
        } else if (gameOver) {
            CheckForInputGameOver();
        } else if (paused) {
            CheckForInputGamePaused();
        }
    }

    void OnEnable() {
        TouchController.SwipeEvent += SwipeHandler;
        TouchController.SwipeEndEvent += SwipeEndHandler;
    }

    void OnDisable() {
        TouchController.SwipeEvent -= SwipeHandler;
        TouchController.SwipeEndEvent -= SwipeEndHandler;
    }

    public void PauseResumeGame() {
        paused = !paused;
        panelManager.ShowPausedPanel(paused);
        soundManager.PlayBackgroundMusic(!paused);
    }

    public void ToggleRotation() {
        rotationLeft = !rotationLeft;
        iconToggleRotation.ToggleIcon(rotationLeft);
    }

    public void PlayAgain() {
        SceneManager.LoadScene(0);
    }

    public void ExitGame() {
        Application.Quit();
        Debug.Log("Quit Application requested");
    }

    void SwipeHandler(Vector2 swipe) {
        swipeDirection = GetDirection(swipe);
    }

    void SwipeEndHandler(Vector2 swipe) {
        swipeEndDirection = GetDirection(swipe);
        Debug.Log("SwipeEndDirection " + swipeEndDirection);
    }

    void CheckForInput() {
        CheckForButtonRotate();
        CheckForButtonDown();
        CheckForButtonsLeftRight();
        CheckForSwipesLeftRight();
        CheckForExtraButtons();
    }

    void CheckForButtonRotate() {
        if (Input.GetButtonDown(BUTTON_ROTATE)) {
            Rotate();
        } else if (swipeEndDirection == Direction.up) {
            Rotate();
            swipeDirection = Direction.none;
            swipeEndDirection = Direction.none;
        }
    }

    void CheckForButtonDown() {
        if (fastDropAllowed && Input.GetButton(BUTTON_DOWN)) {
            timeToDrop = Time.time;
            MoveDown(true);
        } else if (Input.GetButtonUp(BUTTON_DOWN)) {
            fastDropAllowed = true;
            MoveDown(false);
        } else if (fastDropAllowed && swipeDirection == Direction.down) {
            timeToDrop = Time.time;
            MoveDown(true);
            swipeDirection = Direction.none;
            swipeEndDirection = Direction.none;
        } else if (swipeEndDirection == Direction.down) {
            fastDropAllowed = true;
            MoveDown(false);
        }
    }

    void MoveDown(bool fastDrop) {
        if (Time.time > timeToDrop) {
            activeShape.MoveDown();
            timeToDrop = Time.time + (fastDrop ? dropDownFastTime : dropDownNormalTime);
            if (!board.HasShapeValidPosition(activeShape)) {
                SpawnNewShapeWhenPlaced();
                fastDropAllowed = false;
            }
        }
    }

    void CheckForButtonsLeftRight() {
        if (Input.GetButton(BUTTON_LEFT) && Time.time > timeToRepeat || Input.GetButtonDown(BUTTON_LEFT)) {
            MoveLeft();
        } else if (Input.GetButton(BUTTON_RIGHT) && Time.time > timeToRepeat || Input.GetButtonDown(BUTTON_RIGHT)) {
            MoveRight();
        } else if (Input.GetButtonUp(BUTTON_LEFT) || Input.GetButtonUp(BUTTON_RIGHT)) {
            keyRepeatStarted = false;
        }
    }

    void CheckForSwipesLeftRight() {
        if (swipeDirection == Direction.left && Time.time > timeToRepeat) {
            MoveLeft();
        } else if (swipeDirection == Direction.right && Time.time > timeToRepeat) {
            MoveRight();
        }
        if (swipeEndDirection == Direction.left || swipeEndDirection == Direction.right) {
            keyRepeatStarted = false;
            timeToRepeat = Time.time;
        }
        swipeDirection = Direction.none;
        swipeEndDirection = Direction.none;
    }

    void Rotate() {
        activeShape.Rotate(rotationLeft ? Direction.left : Direction.right);
        if (!board.HasShapeValidPosition(activeShape)) {
            activeShape.Rotate(rotationLeft ? Direction.right : Direction.left);
        }
    }

    void MoveRight() {
        if (!keyRepeatStarted) {
            timeToRepeat = Time.time + keyRepeatStart;
            keyRepeatStarted = true;
        } else {
            timeToRepeat = Time.time + keyRepeatRate;
        }
        activeShape.MoveRight();
        if (!board.HasShapeValidPosition(activeShape)) {
            activeShape.MoveLeft();
        }
    }

    void MoveLeft() {
        if (!keyRepeatStarted) {
            timeToRepeat = Time.time + keyRepeatStart;
            keyRepeatStarted = true;
        } else {
            timeToRepeat = Time.time + keyRepeatRate;
        }
        activeShape.MoveLeft();
        if (!board.HasShapeValidPosition(activeShape)) {
            activeShape.MoveRight();
        }
    }

    void CheckForExtraButtons() {
        if (Input.GetButtonDown(BUTTON_PAUSE_GAME)) {
            PauseResumeGame();
        }
        if (Input.GetButtonDown(BUTTON_TOGGLE_ROTATION)) {
            ToggleRotation();
        }
        if (Input.GetButtonDown(BUTTON_TOGGLE_MUSIC)) {
            soundManager.ToggleBackgroundMusic();
        }
        if (Input.GetButtonDown(BUTTON_TOGGLE_SOUND)) {
            soundManager.ToggleSound();
        }
    }

    void CheckForInputGameOver() {
        if (Input.GetButtonDown(BUTTON_PLAY_AGAIN)) {
            PlayAgain();
        }
        if (Input.GetButtonDown(BUTTON_EXIT_GAME)) {
            ExitGame();
        }
    }

    void CheckForInputGamePaused() {
        if (Input.GetButtonDown(BUTTON_PAUSE_GAME)) {
            PauseResumeGame();
        }
    }

    void SpawnNewShapeWhenPlaced() {
        //fastDropRequired = false;
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
        soundManager.PlayBackgroundMusic(false);
        soundManager.PlayGameOverClip();
    }

    void CheckForFullLines() {
        int linesCompleted = board.RemoveFullLines();
        if (linesCompleted > 0) {
            ManageCompletedLines(linesCompleted);
        }
    }

    void ManageCompletedLines(int linesCompleted) {
        Debug.Log("lines completed " + linesCompleted);
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
            dropDownNormalTime += 1f / speedIncreasePerLevel;
        }
    }

    Direction GetDirection(Vector2 swipeMovement) {
        Direction swipeDirection = Direction.none;
        if (Mathf.Abs(swipeMovement.x) >= Mathf.Abs(swipeMovement.y)) {
            swipeDirection = (swipeMovement.x >= 0) ? Direction.right : Direction.left;
        } else {
            swipeDirection = (swipeMovement.y >= 0) ? Direction.up : Direction.down;
        }
        return swipeDirection;
    }
}
