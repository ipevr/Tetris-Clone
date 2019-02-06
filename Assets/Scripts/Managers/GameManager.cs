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
    [SerializeField] float dropDownSpeed = .5f;
    [SerializeField] float dropDownFastSpeed = .05f;
    [SerializeField] float keyRepeatStart = .5f;
    [SerializeField] float keyRepeatRate = .25f;
    [Header("Level Up")]
    [SerializeField] int levelUpAfterCompletedLines = 10;
    [SerializeField] float speedIncreasePerLevel = 0.9f;

    Shape activeShape = null;
    float timeToDrop = 0f;
    float originalDropDownSpeed = 0f;
    int linesCompletedOverAll = 0;
    bool gameOver = false;
    bool fastDropRequired = false;
    float timeToNextKey = 0;
    float timeToRepeat = 0;
    bool keyRepeatStarted = false;
    int actualLevel = 1;
    bool levelIncreased = true;

    void Start() {
        originalDropDownSpeed = dropDownSpeed;
    }

    void Update() {
        if (!gameOver) {
            if (!activeShape) {
                activeShape = spawner.SpawnShape();
            } else {
                DropDownOverTime(fastDropRequired ? dropDownFastSpeed : dropDownSpeed);
                CheckForInput();
            }
        }
    }

    void CheckForFullLines() {
        int linesCompleted = board.RemoveFullLines();
        if (linesCompleted > 0) {
            for (int i = 1; i <= linesCompleted; i++) {
                linesCompletedOverAll++;
                panelManager.SetLinesToNumber(linesCompletedOverAll);
                if (linesCompletedOverAll % levelUpAfterCompletedLines == 0) {
                    actualLevel++;
                    panelManager.SetLevelToNumber(actualLevel);
                    dropDownSpeed *= speedIncreasePerLevel;
                    originalDropDownSpeed = dropDownSpeed;
                }
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
            DropDownOverTime(dropDownFastSpeed);
        }
        if (Input.GetButtonUp(BUTTON_LEFT) || Input.GetButtonUp(BUTTON_RIGHT)) {
            keyRepeatStarted = false;
        }
    }

    void DropDownOverTime(float interval) {
        if (Time.time > timeToDrop) {
            activeShape.MoveDown();
            timeToDrop = Time.time + interval;
            SpawnNewShapeWhenPlaced();
        }
    }

    void SpawnNewShapeWhenPlaced() {
        if (!board.HasShapeValidPosition(activeShape)) {
            fastDropRequired = false;
            activeShape.MoveUp();
            gameOver = board.IsGameOver(activeShape);
            if (!gameOver) {
                board.StoreShapeInGrid(activeShape);
                activeShape = spawner.SpawnShape();
                dropDownSpeed = originalDropDownSpeed;
                CheckForFullLines();
            } else {
                panelManager.ActivatePanelGameOver();
            }

        }
    }
}
