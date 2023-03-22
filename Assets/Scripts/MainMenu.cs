using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance { get; private set; }
    public GameObject InfoScene;
    public GameObject Tutorial;
    public GameObject Username;
    public TMP_InputField UsernameInpt;
    public GameObject Message;
    public CanvasGroup LoadScreen;
    public float delayBeforeLoading = 1f;
    
    public void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void Info()
    {
        InfoScene.SetActive(true);
    }

    public void Tutrl()
    {
        Tutorial.SetActive(true);
    }

    public void TutorialBack()
    {
        Tutorial.SetActive(false);
    }

    public void InfoBack()
    {
        InfoScene.SetActive(false);
    }

    public void Back()
    {
        LoadNewLevel("MainMenu", delayBeforeLoading);
    }
    
    public void QuitButton()
    {
        Application.Quit();
    }

   private IEnumerator ShowTransition(float delay)
    {
        // Get the initial alpha value of the black screen
        float initialAlpha = LoadScreen.alpha;

        // Show the black screen
        LoadScreen.alpha = 0f;
        
        // Animate the fade-in effect using a CanvasGroup component
        CanvasGroup canvasGroup = LoadScreen.GetComponent<CanvasGroup>();
        float t = 0f;
        while (t < delay)
        {
            canvasGroup.alpha = Mathf.Lerp(initialAlpha, 1f, t / delay);
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    private IEnumerator LoadLevelWithDelay(string levelName, float delay)
    {
        // Wait for the delay
        yield return new WaitForSecondsRealtime(delay);

        // Load the new level
        SceneManager.LoadScene(levelName);

        // Resume the game
        Time.timeScale = 1f;
    }

    public void LoadNewLevel(string levelName, float delay)
    {
        // Pause the game
        Time.timeScale = 0f;

        // Show the black screen
        StartCoroutine(ShowTransition(delay));

        // Load the new level after the delay
        StartCoroutine(LoadLevelWithDelay(levelName, delay));
    }

    public void Next()
    {
        Tutorial.SetActive(false);
        Message.SetActive(true);
    }
    public void Note()
    {
       Message.SetActive(false);
       Username.SetActive(true); 
    }

    public void Submit()
    {
        PlayFabManager.SaveName();
    }

    public void Play()
    {
        PlayFabManager.SubmitNameButton();
    }
   
}