using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public GameManager gamemanager;
    public TextMeshProUGUI LevelText;
    public int loadNextLevelScore = 150;
    public GameObject retryButton;
    public CanvasGroup LoadScreen;
    public float delayBeforeLoading = 1f;

    private void Start()
    {
        LevelText.text = "Level " + SceneManager.GetActiveScene().buildIndex;
    }

    public void Retry()
    {
        LoadNewLevel("Level1", delayBeforeLoading); 
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

    public void Update()
    {
        int nextLevel = SceneManager.GetActiveScene().buildIndex + 1;

        if (gamemanager.score >= loadNextLevelScore)
        {
            LoadNewLevel("Level" + nextLevel, delayBeforeLoading);
        }
    }
}