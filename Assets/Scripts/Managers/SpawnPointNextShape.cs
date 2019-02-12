using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointNextShape : MonoBehaviour
{
    [SerializeField] Shape[] shapes = new Shape[] { };
    [SerializeField] Transform[] shapeParents = new Transform[] { };

    public void SetParent(Shape shape) {
        for (int i = 0; i < shapes.Length; i++) {
            if (shape.GetShapeType() == shapes[i].GetShapeType()) {
                shape.transform.parent = shapeParents[i];
                shape.transform.position = shapeParents[i].transform.position;
                shape.transform.localScale = Vector3.one;
            }
        }
    }

}
