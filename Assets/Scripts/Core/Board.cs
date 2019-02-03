using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] int height = 30;
    [SerializeField] int width = 10;

    Transform[,] grid;

    void Awake() {
        grid = new Transform[width, height];
    }

    bool IsWithinBoard(int x, int y) {
        return ((x >= 0 && x < width) && (y >= 0 && y < height));
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
    }

}
