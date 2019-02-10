using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    [SerializeField] Shape[] shapes = new Shape[] { };
    [SerializeField] Transform spawnPoint = null;

    int shapesSpawned = 0;
    Shape nextShape = null;
    Shape actualShape = null;
    SpawnPointNextShape spawnPointNextShape = null;

    void Start() {
        spawnPointNextShape = FindObjectOfType<SpawnPointNextShape>();
    }

    Shape GetRandomShape() {
        return shapes[Random.Range(0, shapes.Length)];
    }

    public Shape SpawnShape() {
        if (shapesSpawned == 0) {
            actualShape = Instantiate(GetRandomShape(), Vector3Int.RoundToInt(spawnPoint.position), Quaternion.identity, spawnPoint.transform);
        } else {
            actualShape = nextShape;
            TransferActualShapeToGameBoard();
        }
        nextShape = Instantiate(GetRandomShape());
        spawnPointNextShape.SetParent(nextShape);
        shapesSpawned++;
        return actualShape;
    }

    void TransferActualShapeToGameBoard() {
        actualShape.transform.parent = spawnPoint;
        actualShape.transform.position = spawnPoint.position;
        actualShape.transform.localScale = Vector3.one;
    }
}
