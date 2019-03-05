using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    [SerializeField] Shape[] shapes = new Shape[] { };
    [SerializeField] Transform spawnPoint = null;
    [SerializeField] DisplayShapeIcon displayShapeNextIcon = null;
    [SerializeField] DisplayShapeIcon displayShapeHoldIcon = null;

    int shapesSpawned = 0;
    Shape nextShape = null;
    Shape actualShape = null;
    Shape parkShape = null;

    public Shape SpawnShape() {
        if (shapesSpawned == 0) {
            actualShape = Instantiate(GetRandomShape(), Vector3Int.RoundToInt(spawnPoint.position), Quaternion.identity, spawnPoint.transform);
        } else {
            actualShape = nextShape;
            PlayShape(actualShape, Vector3.zero);
        }
        nextShape = Instantiate(GetRandomShape());
        displayShapeNextIcon.SetParent(nextShape);
        shapesSpawned++;
        return actualShape;
    }

    public Shape SwitchActualAndParkShapes() {
        if (!parkShape) {
            parkShape = actualShape;
            actualShape = SpawnShape();
        } else {
            Shape helperShape = actualShape;
            Vector3 actualPosition = actualShape.transform.localPosition;
            actualShape = parkShape;
            parkShape = helperShape;
            PlayShape(actualShape, actualPosition);
        }
        parkShape.transform.rotation = Quaternion.identity;
        displayShapeHoldIcon.SetParent(parkShape);
        return actualShape;
    }

    Shape GetRandomShape() {
        return shapes[UnityEngine.Random.Range(0, shapes.Length)];
    }

    void PlayShape(Shape shape, Vector3 position) {
        shape.transform.parent = spawnPoint;
        shape.transform.localPosition = position;
        shape.transform.localScale = Vector3.one;
    }

}
