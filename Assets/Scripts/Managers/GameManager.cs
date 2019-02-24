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
    [Header("Touchscreen specific")]
    [Range(0.05f, 1f)]
    [SerializeField] float minTimeToDrag = 0.15f;
    [Range(0.05f, 1f)]
    [SerializeField] float minTimeToSwipe = 0.3f;
    [Header("Level Up")]
    [SerializeField] int levelUpAfterCompletedLines = 10;
    [SerializeField] float speedIncreasePerLevel = .5f;
    [Header("Score")]
    [SerializeField] int shapeLandScore = 10;
    [SerializeField] int[] fullLineScore = new int[4];
    [SerializeField] bool resetHighScore = false;
    [Header("Music")]
    [SerializeField] bool backGroundMusicMutedOnStart = false;

    Shape activeShape = null;
    SoundManager soundManager;
    float timeToDrop = 0;
    int linesCompletedOverAll = 0;
    bool gameOver = false;
    float timeToRepeat = 0;
    bool keyRepeatStarted = false;
    int actualLevel = 1;
    int totalScore = 0;
    bool paused = false;
    bool rotationLeft = true;
    bool fastDropAllowed = true;
    bool fastDrop = false;
    float dropDownNormalTime = 0;
    float dropDownFastTime = 0;
    float dropDownSpeed = 0;
    float timeToNextDrag = 0;
    float timeToNextSwipe = 0;
    Direction dragDirection = Direction.none;
    Direction swipeDirection = Direction.none;
    bool tapped = false;
    
    void Start() {
        soundManager = FindObjectOfType<SoundManager>();
        dropDownNormalTime = 1f / startDropDownSpeed;
        dropDownFastTime = 1f / dropDownFastSpeed;
        dropDownSpeed = startDropDownSpeed;
        if (resetHighScore) {
            ScoreManager.ResetHighScore();
        }
        if (backGroundMusicMutedOnStart) {
            soundManager.MuteBackGroundMusic(true);
        }
    }

    void Update() {
        if (!gameOver && !paused) {
            if (!activeShape) {
                activeShape = spawner.SpawnShape();
            } else {
                CheckForInput();
            }
        } else if (gameOver) {
            CheckForInputGameOver();
        } else if (paused) {
            CheckForInputGamePaused();
        }
    }

    void OnEnable() {
        TouchController.DragEvent += DragHandler;
        TouchController.SwipeEvent += SwipeHandler;
        TouchController.TapEvent += TapHandler;
        Board.OnLineDeletedEvent += OnLineDeletedHandler;
    }

    void OnDisable() {
        TouchController.DragEvent -= DragHandler;
        TouchController.SwipeEvent -= SwipeHandler;
        TouchController.TapEvent -= TapHandler;
        Board.OnLineDeletedEvent -= OnLineDeletedHandler;
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

    void DragHandler(Vector2 swipe) {
        dragDirection = GetDirection(swipe);
    }

    void SwipeHandler(Vector2 swipe) {
        swipeDirection= GetDirection(swipe);
    }

    void TapHandler(Vector2 tap) {
        tapped = true;
    }

    void OnLineDeletedHandler(int amount) {
        linesCompletedOverAll++;
        panelManager.SetLinesToNumber(linesCompletedOverAll);
        CheckForLevelUp();
        totalScore += fullLineScore[amount - 1];
        panelManager.ShowFlyingScore(fullLineScore[amount - 1], amount == 4 ? true : false);
    }

    void CheckForInput() {
        #region button input
        // ================= button control input ============================================================================================================
        if (Input.GetButtonUp(BUTTON_LEFT) || Input.GetButtonUp(BUTTON_RIGHT)) {
            keyRepeatStarted = false;
        } else if (Input.GetButton(BUTTON_LEFT) && Time.time > timeToRepeat || Input.GetButtonDown(BUTTON_LEFT)) {
            timeToRepeat = Time.time + (keyRepeatStarted ? keyRepeatRate : keyRepeatStart);
            keyRepeatStarted = true;
            MoveLeft();
        } else if (Input.GetButton(BUTTON_RIGHT) && Time.time > timeToRepeat || Input.GetButtonDown(BUTTON_RIGHT)) {
            timeToRepeat = Time.time + (keyRepeatStarted ? keyRepeatRate : keyRepeatStart);
            keyRepeatStarted = true;
            MoveRight();
        } else if (Input.GetButtonDown(BUTTON_ROTATE)) {
            Rotate();
        } else if (Input.GetButtonDown(BUTTON_DOWN)) {
            soundManager.PlayClipShapeDropDown();
        } else if (fastDropAllowed && Input.GetButton(BUTTON_DOWN) && Time.time> timeToDrop) {
            timeToDrop = Time.time + (fastDrop ? dropDownFastTime : dropDownNormalTime);
            fastDrop = true;
            MoveDown();
        } else if (Input.GetButtonUp(BUTTON_DOWN)) {
            fastDrop = false;
            fastDropAllowed = true;
        }
        #endregion
        #region touch input
        // ================= touch control input ============================================================================================================
        else if (dragDirection == Direction.left && Time.time > timeToNextDrag || swipeDirection == Direction.left && Time.time > timeToNextSwipe) {
            MoveLeft();
            timeToNextDrag = Time.time + minTimeToDrag;
            timeToNextSwipe = Time.time + minTimeToSwipe;
        } else if (dragDirection == Direction.right && Time.time > timeToNextDrag || swipeDirection == Direction.right && Time.time > timeToNextSwipe) {
            MoveRight();
            timeToNextDrag = Time.time + minTimeToDrag;
            timeToNextSwipe = Time.time + minTimeToSwipe;
        } else if (swipeDirection == Direction.up && Time.time > timeToNextSwipe || tapped) {
            Rotate();
            timeToNextSwipe = Time.time + minTimeToSwipe;
            tapped = false;
        } else if (fastDropAllowed && dragDirection == Direction.down && Time.time > timeToDrop) {
            fastDrop = true;
            timeToDrop = Time.time + (fastDrop ? dropDownFastTime : dropDownNormalTime);
            MoveDown();
        } else if (swipeDirection == Direction.down) {
            fastDrop = false;
            fastDropAllowed = true;
        } else if (Time.time > timeToDrop) {
            timeToDrop = Time.time + dropDownNormalTime;
            MoveDown();
        }
        dragDirection = Direction.none;
        swipeDirection = Direction.none;
        // ================= touch control input ============================================================================================================
        #endregion
        CheckForExtraButtons();
    }

    void Rotate() {
        activeShape.Rotate(rotationLeft ? Direction.left : Direction.right);
        soundManager.PlayClipShapeRotate();
        if (!board.ShapeInValidPosition(activeShape)) {
            activeShape.Rotate(rotationLeft ? Direction.right : Direction.left);
        }
    }

    void MoveLeft() {
        activeShape.MoveLeft();
        if (board.ShapeInValidPosition(activeShape)) {
            soundManager.PlayClipShapeMoveLeftRight();
        } else {
            activeShape.MoveRight();
        }
    }

    void MoveRight() {
        activeShape.MoveRight();
        if (board.ShapeInValidPosition(activeShape)) {
            soundManager.PlayClipShapeMoveLeftRight();
        } else {
            activeShape.MoveLeft();
        }
    }

    void MoveDown() {
        activeShape.MoveDown();
        if (!board.ShapeInValidPosition(activeShape)) {
            LandShape();
            fastDrop = false;
            fastDropAllowed = false;
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

    void LandShape() {
        activeShape.MoveUp();
        soundManager.PlayClipShapeLands();
        totalScore += shapeLandScore;
        gameOver = board.IsGameOver(activeShape);
        if (!gameOver) {
            board.StoreShapeInGrid(activeShape);
            activeShape = null;
            board.RemoveFullLines();
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

    void CheckForLevelUp() {
        if (linesCompletedOverAll % levelUpAfterCompletedLines == 0) {
            actualLevel++;
            panelManager.SetLevelToNumber(actualLevel);
            dropDownSpeed += speedIncreasePerLevel;
            dropDownNormalTime = 1f / dropDownSpeed;
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
