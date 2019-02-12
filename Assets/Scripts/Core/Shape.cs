using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShapeType { i, j, l, o, s, t, z}

public class Shape : MonoBehaviour {

    [SerializeField] bool canRotate = true;
    [SerializeField] ShapeType shapeType = ShapeType.i;

    void Move(Vector3 moveDirection) {
        transform.position += moveDirection;
    }

    public void MoveLeft() {
        Move(new Vector3(-1, 0, 0));
    }

    public void MoveRight() {
        Move(new Vector3(1, 0, 0));
    }

    public void MoveUp() {
        Move(new Vector3(0, 1, 0));
    }

    public void MoveDown() {
        Move(new Vector3(0, -1, 0));
    }

    public void RotateRight() {
        if (canRotate) {
            transform.Rotate(new Vector3(0, 0, -90));
        }
    }

    public void RotateLeft() {
        if (canRotate) {
            transform.Rotate(new Vector3(0, 0, 90));
        }
    }

    public ShapeType GetShapeType() {
        return shapeType;
    }
}