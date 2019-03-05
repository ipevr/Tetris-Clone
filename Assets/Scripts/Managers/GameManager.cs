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
    const string BUTTON_PARK = "Park";
    const string BUTTON_PLAY_AGAIN = "PlayAgain";
    const string BUTTON_EXIT_GAME = "ExitGame";
    const string BUTTON_PAUSE_GAME = "PauseGame";
    const string BUTTON_TOGGLE_ROTATION = "ToggleRotation";
    const string BUTTON_TOGGLE_MUSIC = "ToggleMusic";
    const string BUTTON_TOGGLE_SOUND = "ToggleSound";
    const float ABSOLUTE_MIN_DRAG_TIME = .01f;
    const float ABSOLUTE_MAX_DRAG_TIME = .5f;
    const float ABSOLUTE_MIN_SWIPE_TIME = .05f;
    const float ABSOLUTE_MAX_SWIPE_TIME = 1f;

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
    [Range(ABSOLUTE_MIN_DRAG_TIME, ABSOLUTE_MAX_DRAG_TIME)]
    [SerializeField] float minTimeToDrag = 0.15f;
    [Range(ABSOLUTE_MIN_SWIPE_TIME, ABSOLUTE_MAX_SWIPE_TIME)]
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
    Ghost ghostShape = null;
    SoundManager soundManager;
    float timeToDrop = 0;
    float dropDownNormalTime = 0;
    float dropDownFastTime = 0;
    float dropDownSpeed = 0;
    float timeToNextDrag = 0;
    float timeToNextSwipe = 0;
    float timeToRepeat = 0;
    int actualLevel = 1;
    int linesCompletedOverAll = 0;
    int totalScore = 0;
    bool gameOver = false;
    bool paused = false;
    bool rotationLeft = true;
    bool keyRepeatStarted = false;
    bool fastDropAllowed = true;
    bool tapped = false;
    Direction dragDirection = Direction.none;
    Direction swipeDirection = Direction.none;

    public float GetAbsoluteMinDragTime => ABSOLUTE_MIN_DRAG_TIME;
    public float GetAbsoluteMaxDragTime => ABSOLUTE_MAX_DRAG_TIME;
    public float GetAbsoluteMinSwipeTime => ABSOLUTE_MIN_SWIPE_TIME;
    public float GetAbsoluteMaxSwipeTime => ABSOLUTE_MAX_SWIPE_TIME;
    public float GetSetMinDragTime {
        get { return minTimeToDrag; }
        set { minTimeToDrag = value; }
    }
    public float GetSetMinSwipeTime {
        get { return minTimeToSwipe; }
        set { minTimeToSwipe = value; }
    }
    
    void Start() {
        soundManager = FindObjectOfType<SoundManager>();
        ghostShape = GetComponent<Ghost>();
        dropDownNormalTime = 1f / startDropDownSpeed;
        dropDownFastTime = 1f / dropDownFastSpeed;
        dropDownSpeed = startDropDownSpeed;
        if (resetHighScore) {
            ResetHighscore();
        }
        if (backGroundMusicMutedOnStart) {
            soundManager.MuteBackGroundMusic(true);
        }
    }

    void Update() {
        if (!gameOver && !paused) {
            if (!activeShape) {
                activeShape = spawner.SpawnShape();
                ghostShape.DrawGhost(activeShape, board);
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
        StartCoroutine(TogglePauseAfterFrames(paused ? 1 : 0));
    }

    public void ResetHighscore() {
        ScoreManager.ResetHighScore();
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

    public void ParkShape() {
        activeShape = spawner.SwitchActualAndParkShapes();
        ghostShape.DrawGhost(activeShape, board);
    }

    void DragHandler(Vector2 swipe) {
        dragDirection = GetDirection(swipe);
    }

    void SwipeHandler(Vector2 swipe) {
        swipeDirection= GetDirection(swipe);
    }

    void TapHandler(Vector2 tap) {
        if (!paused) {
            tapped = true;
        }
    }

    void OnLineDeletedHandler(int amount) {
        linesCompletedOverAll++;
        panelManager.SetLinesToNumber(linesCompletedOverAll);
        CheckForLevelUp();
        totalScore += fullLineScore[amount - 1];
        panelManager.ShowFlyingScore(fullLineScore[amount - 1], amount == 4 ? true : false);
        if (activeShape) {
            ghostShape.DrawGhost(activeShape, board);
        }
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
        } else if (Input.GetButtonDown(BUTTON_DOWN) || (fastDropAllowed && Input.GetButton(BUTTON_DOWN) && Time.time> timeToDrop)) {
            timeToDrop = Time.time + dropDownFastTime;
            MoveDown();
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
        } else if (fastDropAllowed && dragDirection == Direction.down) {
            timeToDrop = Time.time + dropDownFastTime;
            MoveDown();
        } else if (swipeDirection == Direction.down) {
            fastDropAllowed = true;
        } else if (Time.time > timeToDrop) {
            timeToDrop = Time.time + dropDownNormalTime;
            MoveDown();
        }
        dragDirection = Direction.none;
        swipeDirection = Direction.none;
        // ================= touch control input ============================================================================================================
        #endregion
        if (Input.GetButtonDown(BUTTON_DOWN) || swipeDirection == Direction.down) {
            soundManager.PlayClipShapeDropDown();
        }
        if (Input.GetButtonUp(BUTTON_DOWN)) {
            fastDropAllowed = true;
        }
        CheckForExtraButtons();
    }

    void Rotate() {
        activeShape.Rotate(rotationLeft ? Direction.left : Direction.right);
        soundManager.PlayClipShapeRotate();
        if (!board.ShapeInValidPosition(activeShape)) {
            activeShape.Rotate(rotationLeft ? Direction.right : Direction.left);
        } else {
            ghostShape.DrawGhost(activeShape, board);
        }
    }

    void MoveLeft() {
        activeShape.MoveLeft();
        if (board.ShapeInValidPosition(activeShape)) {
            soundManager.PlayClipShapeMoveLeftRight();
            ghostShape.DrawGhost(activeShape, board);
        } else {
            activeShape.MoveRight();
        }
    }

    void MoveRight() {
        activeShape.MoveRight();
        if (board.ShapeInValidPosition(activeShape)) {
            soundManager.PlayClipShapeMoveLeftRight();
            ghostShape.DrawGhost(activeShape, board);
        } else {
            activeShape.MoveLeft();
        }
    }

    void MoveDown() {
        activeShape.MoveDown();
        if (!board.ShapeInValidPosition(activeShape)) {
            LandShape();
            if (Input.GetButton(BUTTON_DOWN) || dragDirection == Direction.down) {
                fastDropAllowed = false;
            }
        }
    }

    void CheckForExtraButtons() {
        if (Input.GetButtonDown(BUTTON_PAUSE_GAME)) {
            PauseResumeGame();
        } else if (Input.GetButtonDown(BUTTON_TOGGLE_ROTATION)) {
            ToggleRotation();
        } else if (Input.GetButtonDown(BUTTON_TOGGLE_MUSIC)) {
            soundManager.ToggleBackgroundMusic();
        } else if (Input.GetButtonDown(BUTTON_TOGGLE_SOUND)) {
            soundManager.ToggleSound();
        } else if (Input.GetButtonDown(BUTTON_PARK)) {
            ParkShape();
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

    IEnumerator TogglePauseAfterFrames(int frames) {
        // Coroutine for unpause the game to prevent a race condition with the tap event from TouchController --> lets the shape rotate on unpausing the game
        for (int i = 0; i < frames; i++) {
            yield return new WaitForEndOfFrame();
        }
        paused = !paused;
        if (!paused) {
            ghostShape.DrawGhost(activeShape, board);
        }
        panelManager.ShowPausedPanel(paused);
        soundManager.PlayBackgroundMusic(!paused);
        yield return null; 
    }
}
