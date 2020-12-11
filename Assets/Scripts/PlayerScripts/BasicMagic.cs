using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMagic : MonoBehaviour
{
    
    public void UseBasicMagic()
    {

    }

    private void CalculateHightPoint()
    {
        // Den Hochpunkt berechnen
        // x = (startx + zielx) / 2
        // y = (starty + ziely) / 2
        // x = (starty + ziely) / 2
        // => Mittelpunkt zwischen start und angeklickte stelle
        // x + wurfhöhe

        // P1 = startpunkt
        // P2 = Mittelpunkt
        // P3 = Angeklickter Punkt
        // => Benzier Curve
    }
}
