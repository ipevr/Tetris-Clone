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
        if (!shape) {
            return false;
        }
        foreach (Transform child in shape.transform) {
            if (child.position.y >= height) {
                return true;
            }
        }
        return false;
    }

    public int RemoveFullLines() {
        int numberOfFullLines = 0;
        for (int y = 0; y < absoluteHeight; y++) {
            if (LineIsComplete(y)) {
                numberOfFullLines++;
                DeleteLine(y);
                y--;
            }
        }
        return numberOfFullLines;
    }

    bool LineIsComplete(int y) {
        for (int x = 0; x < width; x++) {
            if (grid[x, y] == null) {
                return false;
            }
        }
        return true;
    }

    void DeleteLine(int y) {
        for (int x = 0; x < width; x++) {
            if (grid[x, y] != null) {
                Destroy(grid[x, y].gameObject);
                grid[x, y] = null;
            }
        }
        MoveDownAllLinesFrom(y + 1);
    }

    void MoveDownAllLinesFrom(int lineNumber) {
        for (int y = lineNumber; y < absoluteHeight; y++) {
            MoveDownLine(y);
        }
    }

    void MoveDownLine(int y) {
        for (int x = 0; x < width; x++) {
            if (grid[x, y]) {
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;
                grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }
}
