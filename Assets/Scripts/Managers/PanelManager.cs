using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    [SerializeField] GameObject panelGameOver;

    void Start() {
        panelGameOver.SetActive(false);
    }

    public void ActivatePanelGameOver() {
        panelGameOver.SetActive(true);
    }
}
