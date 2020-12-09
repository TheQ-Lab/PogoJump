using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//MAYBE MAKE STATIC LATER
public class CameraControl : MonoBehaviour
{
    public GameObject follows;

    public bool ___CONSTANTS___ = false;

    

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float targetPosY = follows.transform.position.y;
        if (targetPosY > this.transform.position.y)
        {
            this.transform.position = this.transform.position + new Vector3(0f, 0.6f * (targetPosY - this.transform.position.y), 0f);
        }

        
    }

    private void FixedUpdate()
    {
        countScore();
    }

    private void countScore()
    {
        int num = (int)(this.transform.position.y * GameManager.Instance.ScoreMultiplyer /*always 100*/);
        if (num > 0) GameManager.Instance.SetScore(num);
    }
}
