using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour {

    [SerializeField] Slider swipeDistanceSlider = null;
    [SerializeField] Slider dragDistanceSlider = null;

    TouchController touchController = null;
    GameManager gameManager = null;
    bool initialized = false;

    void Start() {
        touchController = FindObjectOfType<TouchController>().GetComponent<TouchController>();
        gameManager = FindObjectOfType<GameManager>().GetComponent<GameManager>();
        InitializePanel();
    }

    public void UpdatePanel() {
        if (initialized) {
            touchController.GetSetMinSwipeDistance = (int)swipeDistanceSlider.value;
            touchController.GetSetMinDragDistance = (int)dragDistanceSlider.value;
        }
    }

    private void InitializePanel() {
        swipeDistanceSlider.minValue = touchController.GetAbsoluteMinSwipeDistance;
        swipeDistanceSlider.maxValue = touchController.GetAbsoluteMaxSwipeDistance;
        swipeDistanceSlider.value = touchController.GetSetMinSwipeDistance;
        dragDistanceSlider.minValue = touchController.GetAbsoluteMinDragDistance;
        dragDistanceSlider.maxValue = touchController.GetAbsoluteMaxDragDistance;
        dragDistanceSlider.value = touchController.GetSetMinDragDistance;
        initialized = true;
    }
}
