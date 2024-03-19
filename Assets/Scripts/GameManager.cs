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

    private string currentGeneration;
    private int maxGen1Pokemon = 151;
    private int maxGen2Pokemon = 100;
    private int maxPokemon;
    [SerializeField] private int maxRounds = 20;
    [SerializeField] private int nextRoundDelay = 3;

    private List<int> pokemonIndexList = new List<int>();
    private List<int> pokemonAnswerIndexList = new List<int>();

    public int currentPokemonIndex;
    public string currentPokemonName;
    public string currentPokemonImageResourceName;
    public string currentPokemonAudioResourceName;
    private Pokemon currentPokemon;

    public string selectedAnswer;
    private GameObject selectedAnswerButton;
    private GameObject[] answerButtons;

    public Pokemon pokemonPrefab;

    private ScoreManager scoreManager;
    private SoundManager soundManager;
    private GenerationManager generationManager;


    public GameObject gameScreen;
    public GameObject endScreen;
    public GameObject scoreObj;
    public GameObject maxChainObj;
    public GameObject answersObj;
    public GameObject timeObj;
    public Timer timer;

    private IEnumerable<string> pokemonImageFiles;
    private IEnumerable<string> pokemonAudioFiles;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        scoreManager = FindAnyObjectByType<ScoreManager>();
        soundManager = FindAnyObjectByType<SoundManager>();
        generationManager = FindAnyObjectByType<GenerationManager>();

        if (generationManager)
        {
            currentGeneration = generationManager.currentGeneration;

            if (currentGeneration == "Gen1")
            {
                maxPokemon = maxGen1Pokemon;
                
            } else if (currentGeneration == "Gen2")
            {
                maxPokemon = maxGen2Pokemon;
            } else
            {
                maxPokemon = maxGen1Pokemon + maxGen2Pokemon;
            }

            if (currentGeneration == "All")
            {
                IEnumerable<string> pokemonGen1ImageFiles = Directory.GetFiles("Assets/Resources/Pokemon/Images/Gen1").Where(x => !x.Contains(".meta"));
                IEnumerable<string> pokemonGen2ImageFiles = Directory.GetFiles("Assets/Resources/Pokemon/Images/Gen2").Where(x => !x.Contains(".meta"));
                pokemonImageFiles = pokemonGen1ImageFiles.Union(pokemonGen2ImageFiles);

                IEnumerable<string> pokemonGen1AudioFiles = Directory.GetFiles("Assets/Resources/Pokemon/Cries/Gen1").Where(x => !x.Contains(".meta"));
                IEnumerable<string> pokemonGen2AudioFiles = Directory.GetFiles("Assets/Resources/Pokemon/Cries/Gen2").Where(x => !x.Contains(".meta"));
                pokemonAudioFiles = pokemonGen1AudioFiles.Union(pokemonGen2AudioFiles);
            } else
            {
                pokemonImageFiles = Directory.GetFiles("Assets/Resources/Pokemon/Images/" + currentGeneration).Where(x => !x.Contains(".meta"));
                pokemonAudioFiles = Directory.GetFiles("Assets/Resources/Pokemon/Cries/" + currentGeneration).Where(x => !x.Contains(".meta"));
            }
        }

        if (SceneManager.GetActiveScene().buildIndex == 2) 
        {
            answerButtons = GameObject.FindGameObjectsWithTag("AnswerButton");
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
        scoreManager.ResetScores();

        GetNewPokemonIndexes();
        LoadPokemon();
        ShowGameScreen();
        timer.StartTimer();
        
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

            string selectedPokemonImageFileName = pokemonImageFiles.ElementAt(currentPokemonIndex);
            currentPokemonImageResourceName = GetPokemonResourceFromFile(selectedPokemonImageFileName, ".png");

            string selectedPokemonAudioFileName = pokemonAudioFiles.ElementAt(currentPokemonIndex);
            currentPokemonAudioResourceName = GetPokemonResourceFromFile(selectedPokemonAudioFileName, ".wav");

            currentPokemonName = GetPokemonNameFromFile(selectedPokemonImageFileName);
            currentPokemon = Instantiate(pokemonPrefab);

            SetAnswers();
        }
    }

    private string GetPokemonResourceFromFile(string fileName, string fileExtension)
    {
        return fileName.Replace("Assets/Resources/", "").Replace("\\", "/").Replace(fileExtension, "");
    }

    private string GetPokemonNameFromFile(string fileName)
    {
        string[] nameArr = fileName.Replace("\\", "/").Split("/");
        return nameArr[nameArr.Length - 1].Replace(".png", "");
    }

    private string GetPokemonByIndex(int index)
    {
        return GetPokemonNameFromFile(pokemonImageFiles.ElementAt(index));
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
        currentPokemon.PlayAudio();

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

        timer.StopTimer();

        ShowEndScreen();

        scoreObj.GetComponent<TextMeshProUGUI>().text = scoreManager.score.ToString();
        maxChainObj.GetComponent<TextMeshProUGUI>().text = scoreManager.maxChain.ToString();
        answersObj.GetComponent<TextMeshProUGUI>().text = scoreManager.correctAnswerCount.ToString() + " / " + maxRounds;
        timeObj.GetComponent<TextMeshProUGUI>().text = timer.GetComponentInChildren<TextMeshProUGUI>().text;
    }

    #endregion

}
