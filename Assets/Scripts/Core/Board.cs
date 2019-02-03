using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] Transform shapeCollector = null;
    [SerializeField] int absoluteHeight = 30;
    [SerializeField] int height = 22;
    [SerializeField] int width = 10;

    Transform[,] grid;

    void Awake() {
        grid = new Transform[width, absoluteHeight];
    }

    bool IsWithinBoard(int x, int y) {
        return ((x >= 0 && x < width) && (y >= 0 && y < absoluteHeight));
    }

    bool IsInFreeGrid(int x, int y) {
        return grid[x, y] == null;
    }

    void DeleteLine(int lineNumber) {
        for (int x = 0; x < width; x++) {
            Destroy(grid[x, lineNumber].gameObject);
            grid[x, lineNumber] = null;
        }
        MoveDownShapesAboveLine(lineNumber);
    }

    void MoveDownShapesAboveLine(int lineNumber) {
        for (int y = lineNumber; y < absoluteHeight - 1; y++) {
            for (int x = 0; x < width; x++) {
                grid[x, y] = grid[x, y + 1];
                if (grid[x, y]) {
                    grid[x, y + 1].position = new Vector3Int(x, y, (int)grid[x, y].position.z);
                }
            }
        }
    }

    public bool HasShapeValidPosition(Shape shape) {
        foreach(Transform child in shape.transform) {
            Vector2Int pos = Vector2Int.RoundToInt(child.position);
            if (!IsWithinBoard(pos.x, pos.y) || !IsInFreeGrid(pos.x, pos.y)) {
                return false;
            }
        }
        return true;
    }

    public void StoreShapeInGrid(Shape shape) {
        foreach (Transform child in shape.transform) {
            Vector2Int pos = Vector2Int.RoundToInt(child.position);
            grid[pos.x, pos.y] = child;
        }
        while (shape.transform.childCount > 0) {
            shape.transform.GetChild(shape.transform.childCount - 1).SetParent(shapeCollector);
        }
        Destroy(shape.gameObject);
    }

    public bool IsGameOver(Shape shape) {
        foreach (Transform child in shape.transform) {
            Vector2Int pos = Vector2Int.RoundToInt(child.position);
            if (pos.y >= height) {
                return true;
            }
        }
        return false;
    }

    public int RemoveFullLines() {
        bool fullLine = false;
        int numberOfFullLines = 0;
        for (int y = 0; y < absoluteHeight; y++) {
            fullLine = true;
            for (int x = 0; x < width; x++) {
                if (grid[x, y] == null) {
                    fullLine = false;
                }
            }
            if (fullLine) {
                Debug.Log("Full line detected");
                numberOfFullLines++;
                DeleteLine(y);
                y--;
            }
        }
        return numberOfFullLines;
    }

}
