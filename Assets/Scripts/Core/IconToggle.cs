using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class IconToggle : MonoBehaviour {

    [SerializeField] Sprite iconTrue = null;
    [SerializeField] Sprite iconFalse =null;
    [SerializeField] bool defualtIconState = true;
    Image image;

    void Awake() {
        image = GetComponent<Image>();
        image.sprite = defualtIconState ? iconTrue : iconFalse;
    }

    public void ToggleIcon(bool status) {
        image.sprite = status ? iconTrue : iconFalse;
    }
}
