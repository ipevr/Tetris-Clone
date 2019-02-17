using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchController : MonoBehaviour {

    [SerializeField] int minSwipeDistance = 20;
    [SerializeField] Text diagnosticText1 = null; 
    [SerializeField] Text diagnosticText2 = null;
    [SerializeField] bool useDiagnostic = false;

    public delegate void TouchEventHandler (Vector2 swipe);
    public static event TouchEventHandler SwipeEvent;
    public static event TouchEventHandler SwipeEndEvent;

    Vector2 touchMovement;

    void Start() {
        Diagnostic("", "");
    }
     
    void Update() {
        if (Input.touchCount > 0) {
            Touch touch = Input.touches[0];
            if (touch.phase == TouchPhase.Began) {
                touchMovement = Vector2.zero;
                Diagnostic("", "");
            } else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) {
                touchMovement += touch.deltaPosition;
                if (touchMovement.magnitude > minSwipeDistance) {
                    OnSwipe();
                    Diagnostic("Swipe detected", touchMovement.ToString() + " " + SwipeDiagnostic(touchMovement));
                }
            } else if (touch.phase == TouchPhase.Ended) {
                OnSwipeEnd();
            }
        }
    }

    void OnSwipe() {
        if (SwipeEvent != null) {
            SwipeEvent(touchMovement);
        }
    }

    void OnSwipeEnd() {
        if (SwipeEndEvent != null) {
            SwipeEndEvent(touchMovement);
        }
    }

    void Diagnostic(string text1, string text2) {
        diagnosticText1.gameObject.SetActive(useDiagnostic);
        diagnosticText2.gameObject.SetActive(useDiagnostic);
        if (diagnosticText1 && diagnosticText2) {
            diagnosticText1.text = text1;
            diagnosticText2.text = text2;
        }
    }

    string SwipeDiagnostic(Vector2 swipeMovement) {
        string direction = "";

        // horizontal
        if (Mathf.Abs(swipeMovement.x) > Mathf.Abs(swipeMovement.y)) {
            direction = (swipeMovement.x >= 0) ? "right" : "left";
        } else { //vertical
            direction = (swipeMovement.y >= 0) ? "up" : "down";
        }
        return direction;
    }
}
