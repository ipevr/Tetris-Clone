using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    [SerializeField] Color color = new Color(1, 1, 1, .2f);
    [SerializeField] SettingsPanel settingsPanel = null;

    Shape ghostShape = null;
    bool hitBottom = false;

    public void DrawGhost(Shape originalShape, Board gameBoard) {
        if (ghostShape) {
            Destroy(ghostShape.gameObject);
        }
        if (settingsPanel.ShowGhost()) {
            ghostShape = Instantiate(originalShape, originalShape.transform.position, originalShape.transform.rotation) as Shape;
            SpriteRenderer[] renderers = ghostShape.gameObject.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer renderer in renderers) {
                renderer.color = color;
            }
            hitBottom = false;
            while (!hitBottom) {
                ghostShape.MoveDown();
                if (!gameBoard.ShapeInValidPosition(ghostShape)) {
                    ghostShape.MoveUp();
                    hitBottom = true;
                }
            }
        }
    }
}
