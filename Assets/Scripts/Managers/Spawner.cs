using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    [SerializeField] Shape[] shapes;
    [SerializeField] Transform spawnPoint;

    Shape GetRandomShape() {
        return shapes[Random.Range(0, shapes.Length)];
    }

    public Shape SpawnShape() {
        return Instantiate(GetRandomShape(), Vector3Int.RoundToInt(spawnPoint.position), Quaternion.identity);
    }

}
