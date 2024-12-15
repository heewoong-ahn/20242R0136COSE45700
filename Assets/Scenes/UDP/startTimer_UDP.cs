using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class startTimer_UDP : MonoBehaviour
{
    public GameData gameData;
    public TextMeshProUGUI timerText;
    private float timeRemaining;
    private bool timerStarted = false; 

    void Start()
    {
        //timerText.text = "READY";
        //Invoke("StartTimer", 1f);
    }

    void checkTimer()
    {
        if (gameData.gameTime <= 94f)
        {
            timerText.text = "READY";
            StartTimer();
        }
    }
    void StartTimer()
    {
        //timerText.text = "3";
        timerStarted = true;
    }

    void Update()
    {
        if (!timerStarted)
        {
            checkTimer();
        }

        timeRemaining = gameData.gameTime - 90f;

        if (timerStarted && timeRemaining > 0)
        {
            //timeRemaining -= Time.deltaTime;
            UpdateTimerDisplay();
        }
        else if (timeRemaining <= 0)
        {
            timerText.text = "Fight!";
            Invoke("DestroyTimer", 1f);
        }
    }

    void DestroyTimer()
    {
        Destroy(gameObject);
    }

    void UpdateTimerDisplay()
    {
        if(timeRemaining>0)
        {
            timerText.text = Mathf.CeilToInt(timeRemaining).ToString();
        }
    }
}