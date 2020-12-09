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

    public void OnClickQuit()
    {
        Debug.Log("QUIT GAME");
        Application.Quit();
    }
}
