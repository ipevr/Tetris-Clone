﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour
{
    [SerializeField] GameObject panelGameOver = null;
    [SerializeField] Text linesText = null;
    [SerializeField] Text levelText = null;

    void Start() {
        panelGameOver.SetActive(false);
        linesText.text = "0";
        levelText.text = "1";
    }

    public void ActivatePanelGameOver() {
        panelGameOver.SetActive(true);
    }

    public void SetLinesToNumber (int number) {
        linesText.text = number.ToString();
    }

    public void SetLevelToNumber (int number) {
        levelText.text = number.ToString();
    }
}
