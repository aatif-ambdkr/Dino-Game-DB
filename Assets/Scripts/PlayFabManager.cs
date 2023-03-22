using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;  

public class PlayFabManager : MonoBehaviour
{
    public static PlayFabManager Instance { get; private set; }
    public GameObject rowPrefab;
    public Transform rowsParent;
    public static string customId = MainMenu.Instance.UsernameInpt.text;

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

    public static void Login()
    {
        var request = new LoginWithCustomIDRequest 
        { 
            CustomId = customId, 
            CreateAccount = true,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams 
            { 
                GetPlayerProfile = true 
            } 
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnSuccess, OnError);
    }
    private static void OnSuccess(LoginResult result)
    {
        Debug.Log("Successful login/account create!");
        string name = null;
        if (result.InfoResultPayload.PlayerProfile != null)
            name = result.InfoResultPayload.PlayerProfile.DisplayName;
        if (name == null)
        {
            MainMenu.Instance.Username.SetActive(true);
        }
        else
        {
            MainMenu.Instance.LoadNewLevel("Level1", MainMenu.Instance.delayBeforeLoading);
        }
    }
    
    public static void SaveName()
    {
        customId = MainMenu.Instance.UsernameInpt.text;
        Login();
    }

    public static void SubmitNameButton()
    {
        var request = new UpdateUserTitleDisplayNameRequest 
        { 
            DisplayName = customId
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdate, OnError);
    }
    private static void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Display name set!");
        MainMenu.Instance.LoadNewLevel("Level1", MainMenu.Instance.delayBeforeLoading);
    }
    private static void OnError(PlayFabError error)
    {
        Debug.Log("Error while logging in/creating a new account");
        Debug.Log(error.GenerateErrorReport());
    }

    public void SendLeaderboard(float hiscore)
    {
        GameManager.Instance.UpdateHiscore();
        // Send the score to the leaderboard
        var request = new UpdatePlayerStatisticsRequest 
        { 
            Statistics = new List<StatisticUpdate> 
            { 
                new StatisticUpdate 
                { 
                    StatisticName = "HighScore", 
                    Value = (int)hiscore 
                } 
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdate, OnError);
    }
    void OnLeaderboardUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Sucessfull leaderboard sent");
    }

    public void GetLeaderboard()
    {
        // Get the leaderboard
        var request = new GetLeaderboardRequest
        {
            StatisticName = "HighScore",
            StartPosition = 0,
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGet, OnError);
    }
    void OnLeaderboardGet(GetLeaderboardResult result)
    {
        foreach (Transform child in rowsParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var player in result.Leaderboard)
        {
            GameObject newGb = Instantiate(rowPrefab, rowsParent);
            TextMeshProUGUI[] texts = newGb.GetComponentsInChildren<TextMeshProUGUI>();
            texts[0].text = (player.Position + 1).ToString();
            texts[1].text = player.DisplayName;
            texts[2].text = player.StatValue.ToString();
            Debug.Log((player.Position + 1) + " " + player.PlayFabId + " " + player.StatValue);
        }
    }
}

