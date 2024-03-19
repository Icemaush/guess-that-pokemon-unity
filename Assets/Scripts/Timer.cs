using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public static Timer instance;

    private float timer;
    [SerializeField] private TextMeshProUGUI timerLabel;
    private bool gameInProgress;

    public void Awake()
    {
        if (instance != null)
        {
            instance.timer = timer;
        }
    }

    void Update()
    {
        if (gameInProgress)
        {
            timer += Time.deltaTime;
            UpdateTimerDisplay(timer);
        }
    }

    public void StartTimer()
    {
        timer = 0f;
        gameInProgress = true;
    }

    public void StopTimer()
    {
        gameInProgress = false;
    }

    public void UpdateTimerDisplay(float time)
    {
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);
        float milliseconds = Mathf.FloorToInt(time * 1000);
        string msString = milliseconds.ToString();
        string msFormatted = "0";

        if (msString.Length == 3)
        {
            msFormatted = msString.Substring(0, 1);
        } else if (msString.Length >= 4)
        {
            msFormatted = msString.Substring(msString.Length - 3, 1);
        }

        string currentTime = string.Format("{00:00}:{1:00}.{02:00}", minutes, seconds, msFormatted);

        timerLabel.text = currentTime;
    }
}
