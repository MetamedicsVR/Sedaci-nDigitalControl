using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UseToggleFromButton : MonoBehaviour
{
    public Toggle toggle;

    public Color colorToggleOn;

    public Color colorToggleOff;

    // Este m�todo se asignar� al evento "onClick" del bot�n en el editor de Unity
    public void CambiarValorToggle()
    {
        // Verificar si el toggle est� activo o no y cambiar su valor
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
