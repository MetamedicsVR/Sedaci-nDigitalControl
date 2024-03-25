using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UseToggleFromButton : MonoBehaviour
{
    public Toggle toggle;

    public Color colorToggleOn;
    public Color colorToggleOff;

    private void Start()
    {
        toggle.onValueChanged.AddListener(UpdateColor);
        UpdateColor(toggle.isOn);
    }

    public void CambiarValorToggle()
    {
        toggle.isOn = !toggle.isOn;
    }

    public void UpdateColor(bool isOn)
    {
        if (isOn)
        {
            toggle.targetGraphic.color = colorToggleOn;
        }
        else
        {
            toggle.targetGraphic.color = colorToggleOff;
        }
    }
}
