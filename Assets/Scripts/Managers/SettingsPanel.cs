using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour {

    [SerializeField] Slider swipeDistanceSlider = null;
    [SerializeField] Slider dragDistanceSlider = null;
    [SerializeField] Slider dragSpeedSlider = null;

    TouchController touchController = null;
    GameManager gameManager = null;
    PanelManager panelManager = null;
    bool initialized = false;

    void Start() {
        touchController = FindObjectOfType<TouchController>().GetComponent<TouchController>();
        gameManager = FindObjectOfType<GameManager>().GetComponent<GameManager>();
        panelManager = FindObjectOfType<PanelManager>().GetComponent<PanelManager>();
        InitializePanel();
    }

    public void UpdateMinSwipeDistance() {
        if (initialized) {
            touchController.GetSetMinSwipeDistance = (int)swipeDistanceSlider.value;
        }
    }

    public void UpdateMinDragDistance() {
        if (initialized) {
            touchController.GetSetMinDragDistance = (int)dragDistanceSlider.value;
        }
    }

    public void UpdateDragSpeed() {
        if (initialized) {
            gameManager.GetSetMinDragTime = dragSpeedSlider.value;
        }
    }

    public void ResetHighscore() {
        gameManager.ResetHighscore();
        panelManager.UpdateHighScore();
    }

    private void InitializePanel() {
        swipeDistanceSlider.minValue = touchController.GetAbsoluteMinSwipeDistance;
        swipeDistanceSlider.maxValue = touchController.GetAbsoluteMaxSwipeDistance;
        swipeDistanceSlider.value = touchController.GetSetMinSwipeDistance;
        dragDistanceSlider.minValue = touchController.GetAbsoluteMinDragDistance;
        dragDistanceSlider.maxValue = touchController.GetAbsoluteMaxDragDistance;
        dragDistanceSlider.value = touchController.GetSetMinDragDistance;
        dragSpeedSlider.minValue = gameManager.GetAbsoluteMinDragTime;
        dragSpeedSlider.maxValue = gameManager.GetAbsoluteMaxDragTime;
        dragSpeedSlider.value = gameManager.GetSetMinDragTime;
        initialized = true;
    }
}
