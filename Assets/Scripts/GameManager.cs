using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    [Tooltip("For safe Distribution to other Scripts through this accessible Static NOT WORK ON RELOAD")]
    public GameObject PlumberLink;

    [Header("Internals, public for ease of access from other scrips or debugging")]
    public bool IsGameplayActive = true;
    public bool IsGameOver = false;
    public int ScoreMultiplyer = 100;

    private MenuGameOver menuGameOver;

    private int score = 0;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if(Instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //plumber = GameObject.Find("Plumber").GetComponent<Plumber>();
        menuGameOver = Resources.FindObjectsOfTypeAll<MenuGameOver>()[0];
    }


    public void SetGameplayPause(bool isPause)
    {
        if (isPause)
        {
            IsGameplayActive = false;
            Time.timeScale = 0f;
        }
        else
        {
            IsGameplayActive = true;
            Time.timeScale = 1f;
        }
    }

    public void GameOver()
    {
        SetGameplayPause(true);
        IsGameOver = true;
        if(menuGameOver == null) { menuGameOver = Resources.FindObjectsOfTypeAll<MenuGameOver>()[0]; }
        menuGameOver.gameObject.SetActive(true);
        
    }

    public void ResetGame()
    {
        score = 0;
        IsGameOver = false;
        SetGameplayPause(false);
        UnityEngine.SceneManagement.SceneManager.LoadScene("main");
    }

    public void SetScore(int newScore)
    {
        score = newScore;

    }

    public int GetScore()
    {
        return score;
    }
}
