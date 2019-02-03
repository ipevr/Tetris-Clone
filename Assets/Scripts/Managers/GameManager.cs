using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    [SerializeField] Board board = null;
    [SerializeField] Spawner spawner = null;
    [SerializeField] PanelManager panelManager = null;
    [SerializeField] float dropDownInterval = .5f;

    Shape activeShape = null;
    float timeToDrop = 0f;
    float originalDropDownInterval = 0f;
    int linesCompleted = 0;
    bool gameOver = false;

    void Start() {
        originalDropDownInterval = dropDownInterval;
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

    void CheckForFullLines() {
        linesCompleted = board.RemoveFullLines();
    }

    void CheckForInput() {
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            activeShape.RotateLeft();
            if (!board.HasShapeValidPosition(activeShape)) {
                activeShape.RotateRight();
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            activeShape.MoveLeft();
            if (!board.HasShapeValidPosition(activeShape)) {
                activeShape.MoveRight();
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            activeShape.MoveRight();
            if (!board.HasShapeValidPosition(activeShape)) {
                activeShape.MoveLeft();
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            dropDownInterval = 0;
            timeToDrop = Time.time;
            DropDownOverTime();
        }
    }

    void DropDownOverTime() {
        if (Time.time > timeToDrop) {
            activeShape.MoveDown();
            timeToDrop = Time.time + dropDownInterval;
            SpawnNewShapeWhenPlaced();
        }
    }

    void SpawnNewShapeWhenPlaced() {
        if (!board.HasShapeValidPosition(activeShape)) {
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
