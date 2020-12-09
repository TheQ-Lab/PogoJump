using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiLives : MonoBehaviour
{
    public int maxLives = 3;
    private GameObject[] lives;

    private void Awake()
    {
        lives = new GameObject[maxLives];
        for (int i = 0; i < maxLives; i++)
        {
            lives[i] = transform.GetChild(i).gameObject;
        }
    }
    void Start()
    {
        
    }

    public void SetLives(int newLives)
    {
        //int clampedLives = Mathf.Min(newLives, maxLives);
        int clampedLives = Mathf.Clamp(newLives, 0, maxLives);
        for (int i=0; i<maxLives; i++)
        {
            if (i+1<=clampedLives) { 
                lives[i].SetActive(true);
            }
            else { 
                lives[i].SetActive(false);
            }
        }
    }
}
