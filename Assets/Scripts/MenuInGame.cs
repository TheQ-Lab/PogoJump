using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuInGame : MonoBehaviour
{
    private Text scoreText;
    private int displayedScore = 0;

    private void Start()
    {
        scoreText = this.GetComponentInChildren<Text>();
    }

    private void Update()
    {
        if (GameManager.Instance.GetScore() != displayedScore)
        {
            UpdateDisplayedScore(GameManager.Instance.GetScore());
        }
    }

    private void UpdateDisplayedScore(int newScore)
    {
        scoreText.text = ((newScore - (newScore % 100)) / 100) + "," + (newScore % 100).ToString("D2") + "m";
        //scoreText.text = "" + newScore;
        displayedScore = newScore;
    }

    public void OnClickRefresh()
    {
        Debug.Log("REFRESH???");
    }

    public void OnClickPause()
    {
        Debug.Log("PAUSE GAME");
        GameManager.Instance.SetGameplayPause(true);
    }
}
