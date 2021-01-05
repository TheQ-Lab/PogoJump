using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuPause : MonoBehaviour
{
    private void Start()
    {
        transform.Find("VolumeSlider").GetComponent<Slider>().value = GameManager.Instance.VolumeMultiplier;
    }

    public void OnClickContinue()
    {
        Debug.Log("CONTINUE GAME");
        GameManager.Instance.SetGameplayPause(false);
    }

    public void OnClickRestart()
    {
        Debug.Log("RESTART GAME");
        GameManager.Instance.ResetGame();
    }

    public void OnClickQuit()
    {
        Debug.Log("QUIT GAME");
        Application.Quit();
    }

    public void OnValueChangedVolume()
    {
        float value = transform.Find("VolumeSlider").GetComponent<Slider>().value;
        Debug.Log("Volume multiplier changed to " + value);
        GameManager.Instance.VolumeMultiplier = value;
    }
}
