using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GenerationManager : MonoBehaviour
{
    public static GenerationManager instance;

    public string currentGeneration;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void SelectGeneration(string generation)
    {
        currentGeneration = generation;

        SceneManager.LoadScene(2);
    }
}
