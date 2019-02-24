using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchController : MonoBehaviour {

    [SerializeField] int minDragDistance = 100;
    [SerializeField] int minSwipeDistance = 200;
    [SerializeField] float tapTimeWindow = 0.1f;
    [SerializeField] Text diagnosticText1 = null; 
    [SerializeField] Text diagnosticText2 = null;
    [SerializeField] bool useDiagnostic = false;

    public delegate void TouchEventHandler (Vector2 swipe);
    public static event TouchEventHandler DragEvent;
    public static event TouchEventHandler SwipeEvent;
    public static event TouchEventHandler TapEvent;

    Vector2 touchMovement;
    float tapTimeMax = 0;

    void Start() {
        Diagnostic("", "");
    }
     
    void Update() {
        if (Input.touchCount > 0) {
            Touch touch = Input.touches[0];
            if (touch.phase == TouchPhase.Began) {
                touchMovement = Vector2.zero;
                tapTimeMax = Time.time + tapTimeWindow;
                Diagnostic("", "");
            } else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) {
                touchMovement += touch.deltaPosition;
                if (touchMovement.magnitude > minDragDistance) {
                    OnDrag();
                    Diagnostic("Drag detected", touchMovement.ToString() + " " + DragSwipeDiagnostic(touchMovement));
                }
            } else if (touch.phase == TouchPhase.Ended) {
                if (touchMovement.magnitude > minSwipeDistance) {
                    OnSwipe();
                    Diagnostic("Swipe detected", touchMovement.ToString() + " " + DragSwipeDiagnostic(touchMovement));
                } else if (Time.time <= tapTimeMax) {
                    OnTap();
                    Diagnostic("Tap detected", touchMovement.ToString() + " " + DragSwipeDiagnostic(touchMovement));
                }
            }
        }
    }

    void OnDrag() {
        if (DragEvent != null) {
            DragEvent(touchMovement);
        }
    }

    void OnSwipe() {
        if (SwipeEvent != null) {
            SwipeEvent(touchMovement);
        }
    }

    void OnTap() {
        if (TapEvent != null) {
            TapEvent(touchMovement);
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

    string DragSwipeDiagnostic(Vector2 swipeMovement) {
        string direction = "";
        bool isHorizontalSwipe = Mathf.Abs(swipeMovement.x) > Mathf.Abs(swipeMovement.y);
        if (isHorizontalSwipe) {
            direction = (swipeMovement.x >= 0) ? "right" : "left";
        } else {
            direction = (swipeMovement.y >= 0) ? "up" : "down";
        }
        return direction;
    }
}
