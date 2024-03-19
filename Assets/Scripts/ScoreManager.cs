using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    [SerializeField] public int baseCorrectAnswerScore = 100;
    [SerializeField] public int baseIncorrectAnswerScore = -50;

    public int score;
    public int chain;
    public int maxChain;
    public int correctAnswerCount;
    private bool previousAnswerCorrect;

    public GameObject scoreObj;
    public GameObject chainObj;
    public GameObject timerObj;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void ResetScores()
    {
        score = 0; 
        chain = 0;
        maxChain = 0;
        correctAnswerCount = 0;
        previousAnswerCorrect = false;

        UpdateScores();
    }

    public void UpdateScores()
    {
        scoreObj.GetComponent<TextMeshProUGUI>().text = score.ToString();
        chainObj.GetComponent<TextMeshProUGUI>().text = chain.ToString();
    }

    public void UpdateScoreCorrectAnswer()
    {
        correctAnswerCount++;

        if (previousAnswerCorrect)
        {
            chain++;
        }

        if (chain > maxChain)
        {
            maxChain++;
        }

        if (chain > 0)
        {
            score += baseCorrectAnswerScore * chain;
        }
        else
        {
            score += baseCorrectAnswerScore;
        }

        previousAnswerCorrect = true;

        UpdateScores();
    }

    public void UpdateScoreIncorrectAnswer()
    {
        chain = 0;
        score += baseIncorrectAnswerScore;
        previousAnswerCorrect = false;

        UpdateScores();
    }
}
