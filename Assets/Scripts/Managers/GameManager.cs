using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    [SerializeField] Board gameBoard;
    [SerializeField] Spawner spawner;

    Shape spawnedShape = null;

    // Start is called before the first frame update
    void Start()
    {
        spawnedShape = spawner.SpawnShape();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
