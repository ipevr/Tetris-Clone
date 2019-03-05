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
    [SerializeField] float timeToWaitAfterLineDeleted = 0.2f;
    [SerializeField] Transform destroyBlockParticle = null;

    SoundManager soundManager;
    Transform[,] grid;

    public delegate void OnLineDeleted(int amountOfDeletedLinesAtOnce);
    public static event OnLineDeleted OnLineDeletedEvent;

    void Awake() {
        grid = new Transform[width, absoluteHeight];
    }

    void Start() {
        soundManager = FindObjectOfType<SoundManager>();
    }

    public bool ShapeInValidPosition(Shape shape) {
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

    public void RemoveFullLines() {
        StartCoroutine(RemoveFullLinesStepByStep());
    }

    public void PutShapesToLayername(string name) {
        SpriteRenderer[] renderers = transform.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < renderers.Length; i++) {
            renderers[i].sortingLayerName = name;
        }
    }

    bool IsWithinBoard(int x, int y) {
        return ((x >= 0 && x < width) && (y >= 0 && y < absoluteHeight));
    }

    bool IsInFreeGrid(int x, int y) {
        return grid[x, y] == null;
    }

    bool LineIsComplete(int y) {
        for (int x = 0; x < width; x++) {
            if (grid[x, y] == null) {
                return false;
            }
        }
        PlayParticleEffectAt(y);
        return true;
    }

    void DeleteLine(int y) {
        for (int x = 0; x < width; x++) {
            if (grid[x, y] != null) {
                Destroy(grid[x, y].gameObject);
                grid[x, y] = null;
            }
        }
    }

    void PlayParticleEffectAt(int y) {
        for (int x = 0; x < width; x++) {
            Transform particle = Instantiate(destroyBlockParticle, new Vector3(x, y), Quaternion.identity);
            ParticleSystem particleSystem = particle.GetComponent<ParticleSystem>();
            particle.GetComponent<ParticleController>().SetSubParticleColor(0, grid[x, y].GetComponent<SpriteRenderer>().color);
            particleSystem.Play();
            StartCoroutine(DestroyParticleSystemWhenPlayed(particle));
        }
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

    IEnumerator RemoveFullLinesStepByStep() {
        int amountOfDeletedLines = 0;
        for (int y = 0; y < absoluteHeight; y++) {
            if (LineIsComplete(y)) {
                DeleteLine(y);
                y--;
                soundManager.PlayFullLinesClip(amountOfDeletedLines);
                amountOfDeletedLines++;
                yield return new WaitForSeconds(timeToWaitAfterLineDeleted);
                MoveDownAllLinesFrom(y + 1);
                OnLineDeletedEvent(amountOfDeletedLines);
            }
        }
        yield return new WaitForEndOfFrame();
    }

    IEnumerator DestroyParticleSystemWhenPlayed(Transform particle) {
        while (particle.GetComponent<ParticleSystem>().isPlaying) {
            yield return new WaitForEndOfFrame();
        }
        Destroy(particle.gameObject);
    }

}
