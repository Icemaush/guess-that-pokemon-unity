using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public static Timer Instance;

    private float timer;
    [SerializeField] private TextMeshProUGUI timerLabel;
    private bool gameInProgress;

    public void Awake()
    {
        if (Instance != null)
        {
            Instance.timer = timer;
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
        //float milliseconds = Mathf.FloorToInt(time / 1000);

        string currentTime = string.Format("{00:00}:{1:00}", minutes, seconds);

        timerLabel.text = currentTime;
    }
}
