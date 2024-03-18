using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private string generation;

    private int maxPokemon = 151;
    [SerializeField] private int maxRounds = 20;
    [SerializeField] private int nextRoundDelay = 3;

    private List<int> pokemonIndexList = new List<int>();
    private List<int> pokemonAnswerIndexList = new List<int>();

    public int currentPokemonIndex;
    public string currentPokemonName;
    public string currentPokemonResourceName;
    private Pokemon currentPokemon;

    public string selectedAnswer;
    private GameObject selectedAnswerButton;
    private GameObject[] answerButtons;

    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private Pokemon pokemonPrefab;

    private GameObject gameScreen;
    private GameObject endScreen;

    private IEnumerable<string> pokemonFiles;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        gameScreen = GameObject.FindGameObjectWithTag("GameScreen");
        endScreen = GameObject.FindGameObjectWithTag("EndScreen");

        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            NewGame();
        }
    }

    #region Main Menu 

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }

    #endregion

    #region Game

    public void ReturnToHomeScreen()
    {
        SceneManager.LoadScene(0);
    }

    public void NewGame()
    {
        if (answerButtons == null)
        {
            answerButtons = GameObject.FindGameObjectsWithTag("AnswerButton");
        }

        if (pokemonFiles == null)
        {
            pokemonFiles = Directory.GetFiles("Assets/Resources/Pokemon/Gen1").Where(x => !x.Contains(".meta"));
        }
        
        scoreManager.ResetScores();
        GetNewPokemonIndexes();
        LoadPokemon();

        ShowGameScreen();
    }

    private void ShowGameScreen()
    {
        gameScreen.SetActive(true);
        endScreen.SetActive(false);
    }

    private void ShowEndScreen()
    {
        gameScreen.SetActive(false);
        endScreen.SetActive(true);
    }

    private void GetNewPokemonIndexes()
    {
        pokemonIndexList.Clear();
        pokemonAnswerIndexList.Clear();

        while (pokemonIndexList.Count < maxRounds)
        {
            int index = Random.Range(0, maxPokemon);

            if (!pokemonIndexList.Contains(index))
            {
                pokemonIndexList.Add(index);
            }
        }
    }

    private void LoadPokemon()
    {
        if (currentPokemon)
        {
            Destroy(currentPokemon.gameObject);
        }

        if (pokemonIndexList.Count == 0)
        {
            EndGame();
        } else
        {
            currentPokemonIndex = pokemonIndexList[0];
            pokemonIndexList.Remove(currentPokemonIndex);

            string selectedPokemonFileName = pokemonFiles.ElementAt(currentPokemonIndex);

            currentPokemonResourceName = GetPokemonResourceFromFile(selectedPokemonFileName);
            currentPokemonName = GetPokemonNameFromFile(selectedPokemonFileName);
            currentPokemon = Instantiate(pokemonPrefab);

            SetAnswers();
        }
    }

    private string GetPokemonResourceFromFile(string fileName)
    {
        return fileName.Replace("Assets/Resources/", "").Replace("\\", "/").Replace(".png", "");
    }

    private string GetPokemonNameFromFile(string fileName)
    {
        string[] nameArr = fileName.Split("/");
        return nameArr[nameArr.Length - 1].Replace(".png", "");
    }

    private string GetPokemonByIndex(int index)
    {
        return GetPokemonNameFromFile(pokemonFiles.ElementAt(index));
    }

    private void SetAnswers()
    {
        int[] answerIndexes = new int[4];
        int currentPokemonAnswerIndex = Random.Range(0, 4);

        answerIndexes[currentPokemonAnswerIndex] = currentPokemonIndex;

        for (int i = 0; i < answerIndexes.Length; i++)
        {
            if (answerIndexes[i] == 0)
            {
                int randomIndex = Random.Range(0, maxPokemon);

                if (answerIndexes.Contains(randomIndex))
                {
                    i--;
                }
                else
                {
                    answerIndexes[i] = randomIndex;
                }
            }
        }
        Debug.Log(answerButtons.Length);
        for (int i = 0; i < answerIndexes.Length; i++)
        {
            answerButtons[i].gameObject.GetComponent<Image>().color = Color.blue;
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = GetPokemonByIndex(answerIndexes[i]);
        }

        SetAnswerButtonsEnabled(true);
    }

    public void GuessPokemon()
    {
        SetAnswerButtonsEnabled(false);

        selectedAnswerButton = EventSystem.current.currentSelectedGameObject;
        selectedAnswer = selectedAnswerButton.GetComponentInChildren<TextMeshProUGUI>().text;

        currentPokemon.RevealImage();

        if (selectedAnswer == currentPokemonName)
        {
            selectedAnswerButton.GetComponent<Image>().color = AnswerButton.correctColor;
            scoreManager.UpdateScoreCorrectAnswer();
        }
        else
        {
            selectedAnswerButton.GetComponent<Image>().color = AnswerButton.incorrectColor;
            scoreManager.UpdateScoreIncorrectAnswer();
        }

        StartCoroutine(NextRoundAfterWaitTime());
    }

    private void SetAnswerButtonsEnabled(bool isEnabled)
    {
        foreach (GameObject answerButton in answerButtons)
        {
            answerButton.GetComponent<Button>().enabled = isEnabled;
        }
    }

    private IEnumerator NextRoundAfterWaitTime()
    {
        yield return new WaitForSeconds(nextRoundDelay);

        if (pokemonIndexList.Count == 0)
        {
            EndGame();
        }
        else
        {
            LoadPokemon();
        }
    }

    private void EndGame()
    {
        if (currentPokemon)
        {
            Destroy(currentPokemon.gameObject);
        }

        ShowEndScreen();

        GameObject scoreObj = GameObject.FindGameObjectWithTag("EndScore");
        GameObject maxChainObj = GameObject.FindGameObjectWithTag("EndMaxChain");
        GameObject answersObj = GameObject.FindGameObjectWithTag("EndAnswers");
        GameObject timeObj = GameObject.FindGameObjectWithTag("EndTime");

        scoreObj.GetComponent<TextMeshProUGUI>().text = scoreManager.score.ToString();
        maxChainObj.GetComponent<TextMeshProUGUI>().text = scoreManager.maxChain.ToString();
        answersObj.GetComponent<TextMeshProUGUI>().text = scoreManager.correctAnswerCount.ToString() + " / " + maxRounds;
        timeObj.GetComponent<TextMeshProUGUI>().text = "Coming soon!";
    }



    #endregion

}
