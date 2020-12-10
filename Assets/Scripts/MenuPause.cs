using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPause : MonoBehaviour
{
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
}
