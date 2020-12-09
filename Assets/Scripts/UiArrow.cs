using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // ADDITIONAL

public class UiArrow : MonoBehaviour
{
    public enum ArrowColors{ standard, poweredUp };

    public Color32 StandardColor, PoweredUpColor;
    private Slider slider;

    private void Awake()
    {
        slider = this.gameObject.GetComponent<Slider>();
    }

    private void Start()
    {
        SetColor(ArrowColors.standard);
    }

    public void SetMaxPower(float maxPower)
    {
        slider.maxValue = maxPower;
    }

    public void SetColor(ArrowColors color)
    {
        if (color == ArrowColors.standard)
            this.transform.Find("Fill").GetComponent<Image>().color = StandardColor;
        else if (color == ArrowColors.poweredUp)
            this.transform.Find("Fill").GetComponent<Image>().color = PoweredUpColor;
    }

    public void Activate()
    {
        this.gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        this.gameObject.SetActive(false);
    }

    public void SetAngle(float angle)
    {
        this.gameObject.transform.rotation = Quaternion.Euler(0, 0, 270f + angle);
    }

    public void SetPower(float power)
    {
        slider.value = power;
    }
}
