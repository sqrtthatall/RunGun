using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LeaderBoard : MonoBehaviour
{
    [Header("Server")]
    [SerializeField] private string leaderboardUrl = "http://127.0.0.1:5000/leaderboard";

    [Header("UI References")]
    [SerializeField] private TMP_Text displayTMPText;
    [SerializeField] private Button backButton;
    [SerializeField] private Button refreshButton;

    [Header("Scenes")]
    [SerializeField] private string sceneName = "Login";

    [Serializable]
    public class UserEntry
    {
        public string username;
        public int coins;
        public int deaths;
    }

    [Serializable]
    public class LeaderboardResponse
    {
        public string status;
        public List<UserEntry> leaderboard;
    }

    private void Awake()
    {
        if (backButton != null)
            backButton.onClick.AddListener(OnBackClicked);

        if (refreshButton != null)
            refreshButton.onClick.AddListener(LoadLeaderboard);
    }

    private void Start()
    {
        LoadLeaderboard();
    }

    public void LoadLeaderboard()
    {
        StartCoroutine(GetLeaderboardData());
    }

    public void OnBackClicked()
    {
        Debug.Log("Переход на сцену: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator GetLeaderboardData()
    {
        if (displayTMPText != null)
            displayTMPText.text = "Загрузка...";

        using (UnityWebRequest request = UnityWebRequest.Get(leaderboardUrl))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                if (displayTMPText != null)
                    displayTMPText.text = "Ошибка загрузки данных";
                yield break;
            }

            string json = request.downloadHandler.text;
            LeaderboardResponse data = JsonUtility.FromJson<LeaderboardResponse>(json);

            if (data != null && data.leaderboard != null && data.leaderboard.Count > 0)
            {
                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < data.leaderboard.Count; i++)
                {
                    UserEntry user = data.leaderboard[i];
                    builder.AppendLine($"{i + 1}. {user.username} — {user.coins} монет");
                }

                if (displayTMPText != null)
                    displayTMPText.text = builder.ToString();
            }
            else
            {
                if (displayTMPText != null)
                    displayTMPText.text = "Таблица лидеров пуста";
            }
        }
    }
}