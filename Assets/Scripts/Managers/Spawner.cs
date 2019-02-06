using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    [SerializeField] Shape[] shapes = new Shape[] { };
    [SerializeField] Transform spawnPoint = null;
    [SerializeField] Transform spawnPointNextShape = null;
    [SerializeField] float zoomFactorNextShape = .8f;

    int shapesSpawned = 0;
    Shape nextShape = null;
    Shape actualShape = null;

    Shape GetRandomShape() {
        return shapes[Random.Range(0, shapes.Length)];
    }

    public Shape SpawnShape() {
        if (shapesSpawned == 0) {
            actualShape = Instantiate(GetRandomShape(), Vector3Int.RoundToInt(spawnPoint.position), Quaternion.identity);
        } else {
            actualShape = nextShape;
            actualShape.transform.localScale = Vector3.one;
            actualShape.transform.position = spawnPoint.position;
        }
        nextShape = Instantiate(GetRandomShape(), Vector3Int.RoundToInt(spawnPointNextShape.position), Quaternion.identity);
        nextShape.transform.localScale = new Vector3(zoomFactorNextShape, zoomFactorNextShape);
        shapesSpawned++;
        return actualShape;
    }

}
