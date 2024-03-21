using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UseToggleFromButton : MonoBehaviour
{
    public Toggle toggle;

    public Color colorToggleOn;

    public Color colorToggleOff;

    // Este método se asignará al evento "onClick" del botón en el editor de Unity
    public void CambiarValorToggle()
    {
        // Verificar si el toggle está activo o no y cambiar su valor
        toggle.isOn = !toggle.isOn;
        if (toggle.isOn)
        {
            toggle.targetGraphic.color = colorToggleOn;
        }
        else
        {
            toggle.targetGraphic.color = colorToggleOff;
        }
    }
}
