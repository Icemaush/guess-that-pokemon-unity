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

    [SerializeField] private GameObject scoreObj;
    [SerializeField] private GameObject chainObj;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void ResetScores()
    {
        Debug.Log("resetting scores");
        score = 0; 
        chain = 0;
        maxChain = 0;
        correctAnswerCount = 0;
        previousAnswerCorrect = false;

        UpdateScores();
    }

    public void UpdateScores()
    {
        Debug.Log("updating scores");
        scoreObj.GetComponent<TextMeshProUGUI>().text = score.ToString();
        chainObj.GetComponent<TextMeshProUGUI>().text = chain.ToString();
        Debug.Log("scores updated");
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
