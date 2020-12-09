using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuGameOver : MonoBehaviour
{
    public Text finalScore;
    private void Start()
    {
        GameObject.Find("InGameMenu").SetActive(false); //automatically disables InGameMenu on Activation
        int score = GameManager.Instance.GetScore();
        finalScore.text = "YOUR SCORE: " + ((score - (score % 100)) / 100) + "," + (score % 100).ToString("D2") + "m";
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
