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
    [SerializeField] float dropDownInterval = .5f;
    [SerializeField] float dropDownFastInterval = .05f;
    [SerializeField] float keyRepeatRate = 0.25f;

    Shape activeShape = null;
    float timeToDrop = 0f;
    float originalDropDownInterval = 0f;
    int linesCompleted = 0;
    bool gameOver = false;
    bool fastDropRequired = false;
    float timeToNextKey = 0;

    void Start() {
        originalDropDownInterval = dropDownInterval;
    }

    void Update() {
        if (!gameOver) {
            if (!activeShape) {
                activeShape = spawner.SpawnShape();
            } else {
                DropDownOverTime(fastDropRequired ? dropDownFastInterval : dropDownInterval);
                CheckForInput();
            }
        }
    }

    void CheckForFullLines() {
        linesCompleted = board.RemoveFullLines();
    }

    void CheckForInput() {
        if (Input.GetButtonDown(BUTTON_ROTATE)) {
            timeToNextKey = Time.time + keyRepeatRate;
            activeShape.RotateLeft();
            if (!board.HasShapeValidPosition(activeShape)) {
                activeShape.RotateRight();
            }
        }
        if (Input.GetButton(BUTTON_LEFT) && Time.time > timeToNextKey || Input.GetButtonDown(BUTTON_LEFT)) {
            timeToNextKey = Time.time + keyRepeatRate;
            activeShape.MoveLeft();
            if (!board.HasShapeValidPosition(activeShape)) {
                activeShape.MoveRight();
            }
        }
        if (Input.GetButton(BUTTON_RIGHT) && Time.time > timeToNextKey || Input.GetButtonDown(BUTTON_RIGHT)) {
            timeToNextKey = Time.time + keyRepeatRate;
            activeShape.MoveRight();
            if (!board.HasShapeValidPosition(activeShape)) {
                activeShape.MoveLeft();
            }
        }
        if (Input.GetButtonDown(BUTTON_DOWN)) {
            timeToNextKey = Time.time + keyRepeatRate;
            timeToDrop = Time.time;
            fastDropRequired = true;
            DropDownOverTime(dropDownFastInterval);
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
                dropDownInterval = originalDropDownInterval;
                CheckForFullLines();
            } else {
                panelManager.ActivatePanelGameOver();
            }

        }
    }
}
