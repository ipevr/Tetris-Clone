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

}
