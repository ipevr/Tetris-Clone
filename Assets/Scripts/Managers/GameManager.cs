using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    const string BUTTON_ROTATE = "Rotate";
    const string BUTTON_RIGHT = "MoveRight";
    const string BUTTON_LEFT = "MoveLeft";
    const string BUTTON_DOWN = "MoveDown";

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
    [Header("Sounds")]
    [SerializeField] AudioClip shapeLandsClip = null;
    [SerializeField] AudioClip fullLineClip = null;
    [SerializeField] AudioClip fourFullLinesClip = null;
    [SerializeField] AudioClip gameOverClip = null;

    Shape activeShape = null;
    AudioSource audioSource = null;
    float timeToDrop = 0f;
    int linesCompletedOverAll = 0;
    bool gameOver = false;
    bool fastDropRequired = false;
    float timeToNextKey = 0;
    float timeToRepeat = 0;
    bool keyRepeatStarted = false;
    int actualLevel = 1;
    int totalScore = 0;

    void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    void Update() {
        if (!gameOver) {
            if (!activeShape) {
                activeShape = spawner.SpawnShape();
            } else {
                DropDownOverTime();
                CheckForInput();
            }
        }
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
        audioSource.PlayOneShot(shapeLandsClip);
        totalScore += shapeLandScore;
        gameOver = board.IsGameOver(activeShape);
        if (!gameOver) {
            board.StoreShapeInGrid(activeShape);
            activeShape = spawner.SpawnShape();
            CheckForFullLines();
        } else {
            panelManager.ActivatePanelGameOver();
            audioSource.PlayOneShot(gameOverClip);
        }
        panelManager.SetTotalScore(totalScore);
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
        audioSource.PlayOneShot(linesCompleted < 4 ? fullLineClip : fourFullLinesClip);
        totalScore += fullLineScore[linesCompleted - 1];
        panelManager.ShowFlyingScore(fullLineScore[linesCompleted - 1]);
    }

    void CheckForLevelUp() {
        if (linesCompletedOverAll % levelUpAfterCompletedLines == 0) {
            actualLevel++;
            panelManager.SetLevelToNumber(actualLevel);
            dropDownNormalSpeed *= 1f - actualLevel * speedIncreaseFactorPerLevel;
        }
    }
}
