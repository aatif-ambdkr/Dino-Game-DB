using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public float initialGameSpeed = 5f;
    public float gameSpeedIncrease = 0.1f;
    public float gameSpeed { get; private set; }

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI hiscoreText;
    public TextMeshProUGUI gameOverText;
    public Button retryButton;
    public GameObject LBPanel;
    public Button leaderboard;

    public GameObject pausePanel;

    private Player player;
    private Spawner spawner;

    private bool isPaused = false; 

    public float score;
    private float hiscoreValue; 
    
    private void Awake()
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
    
    private void OnDestroy()
    {
        if (Instance == this) 
        {
            Instance = null;
        }
    }
    
    private void Start()
    {
        player = FindObjectOfType<Player>();
        spawner = FindObjectOfType<Spawner>();
        
        NewGame();
    }
    
    public void NewGame()
    {
        Obstacle[] obstacles = FindObjectsOfType<Obstacle>();
        
        foreach (var obstacle in obstacles) 
        {
            Destroy(obstacle.gameObject);
        }
        score = 0f;
        gameSpeed = initialGameSpeed;
        enabled = true;
        
        player.gameObject.SetActive(true);
        spawner.gameObject.SetActive(true);
        gameOverText.gameObject.SetActive(false);
        retryButton.gameObject.SetActive(false);
        leaderboard.gameObject.SetActive(false);
        LBPanel.SetActive(false);
        pausePanel.SetActive(false); 
        
        UpdateHiscore();
    }
    
    public void GameOver()
    {
        gameSpeed = 0f;
        enabled = false;
        player.gameObject.SetActive(false);
        spawner.gameObject.SetActive(false);
        gameOverText.gameObject.SetActive(true);
        retryButton.gameObject.SetActive(true);
        leaderboard.gameObject.SetActive(true);
        hiscoreValue = Mathf.FloorToInt(PlayerPrefs.GetFloat("hiscore", 0));
        PlayFabManager.Instance.SendLeaderboard(hiscoreValue);
        
        UpdateHiscore();
    }
    
    private void Update()
    {
        if (!isPaused) 
        {
            gameSpeed += gameSpeedIncrease * Time.deltaTime;
            score += gameSpeed * Time.deltaTime;
            scoreText.text = Mathf.FloorToInt(score).ToString("D5");
        }
        
        UpdateHiscore();
        
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            TogglePause(); 
        }
    }
    
    public void UpdateHiscore()
    {
        float hiscore = PlayerPrefs.GetFloat("hiscore", 0);
        
        if (score > hiscore)
        {
            hiscore = score;
            PlayerPrefs.SetFloat("hiscore", hiscore);
        }
        
        hiscoreText.text = Mathf.FloorToInt(hiscore).ToString("D5");
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteAll();
    }
 
    public void TogglePause() 
    {
        isPaused = !isPaused;
        
        if (isPaused)
        {
            Time.timeScale = 0f; 
            pausePanel.SetActive(true); 
        }
        else
        {
            Time.timeScale = 1f; 
            pausePanel.SetActive(false); 
        }
    }

    public void GetLeaderboard()
    {
        PlayFabManager.Instance.GetLeaderboard();
        LBPanel.SetActive(true);
    }

    public void CloseLeaderboard()
    {
        LBPanel.SetActive(false);
    }

    public void QuitGame() 
    {
        Application.Quit(); 
    }
}