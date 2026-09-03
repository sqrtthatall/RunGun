using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance;

    [SerializeField] private string baseUrl = "http://localhost:5000";

    // Сохраняем имя авторизованного игрока
    public string CurrentUsername { get; private set; }

    // DTO под монеты и смерти
    [Serializable] public class AuthRequest { public string username; public string password; }
    [Serializable] public class StatsRequest { public string username; public int coins_earned; public int deaths_count; }

    [Serializable] 
    public class LoginResponse 
    { 
        public string status; 
        public int coins; 
        public int deaths; 
        public string message; 
    }

    [Serializable] 
    public class LeaderboardEntry 
    { 
        public string username; 
        public int coins; 
        public int deaths; 
    }

    [Serializable] 
    public class LeaderboardResponse 
    { 
        public string status; 
        public List<LeaderboardEntry> leaderboard; 
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Регистрация
    public void Register(string user, string pass, Action onSuccess = null, Action<string> onError = null)
    {
        StartCoroutine(RegisterCoroutine(user, pass, onSuccess, onError));
    }

    // Вход
    public void Login(string user, string pass, Action<LoginResponse> onSuccess = null, Action<string> onError = null)
    {
        StartCoroutine(LoginCoroutine(user, pass, onSuccess, onError));
    }

    // Отправка прогресса: сколько собрано монет и сколько смертей за раунд
    public void SendProgress(int coinsEarned, int deathsCount, Action onSuccess = null, Action<string> onError = null)
    {
        if (string.IsNullOrEmpty(CurrentUsername))
        {
            Debug.LogWarning("NetworkManager: Сначала нужно войти в аккаунт!");
            onError?.Invoke("Not logged in");
            return;
        }
        StartCoroutine(SendProgressCoroutine(CurrentUsername, coinsEarned, deathsCount, onSuccess, onError));
    }

    // Запрос лидерборда
    public void GetLeaderboard(Action<List<LeaderboardEntry>> onSuccess = null, Action<string> onError = null)
    {
        StartCoroutine(GetLeaderboardCoroutine(onSuccess, onError));
    }

    // --- Корутины ---

    private IEnumerator RegisterCoroutine(string user, string pass, Action onSuccess, Action<string> onError)
    {
        string json = JsonUtility.ToJson(new AuthRequest { username = user, password = pass });
        using (UnityWebRequest req = CreateJsonPost(baseUrl + "/register", json))
        {
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Register Success: " + req.downloadHandler.text);
                onSuccess?.Invoke();
            }
            else
            {
                Debug.LogError("Register Error: " + req.downloadHandler.text);
                onError?.Invoke(req.downloadHandler.text);
            }
        }
    }

    private IEnumerator LoginCoroutine(string user, string pass, Action<LoginResponse> onSuccess, Action<string> onError)
    {
        string json = JsonUtility.ToJson(new AuthRequest { username = user, password = pass });
        using (UnityWebRequest req = CreateJsonPost(baseUrl + "/login", json))
        {
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                LoginResponse res = JsonUtility.FromJson<LoginResponse>(req.downloadHandler.text);
                if (res.status == "success")
                {
                    CurrentUsername = user;
                    Debug.Log($"Logged in as {user}. Coins: {res.coins}, Deaths: {res.deaths}");
                    onSuccess?.Invoke(res);
                }
                else
                {
                    Debug.LogError("Login Failed: " + res.message);
                    onError?.Invoke(res.message);
                }
            }
            else
            {
                Debug.LogError("Login Error: " + req.error);
                onError?.Invoke(req.error);
            }
        }
    }

    private IEnumerator SendProgressCoroutine(string user, int coins, int deaths, Action onSuccess, Action<string> onError)
    {
        string json = JsonUtility.ToJson(new StatsRequest 
        { 
            username = user, 
            coins_earned = coins, 
            deaths_count = deaths 
        });

        using (UnityWebRequest req = CreateJsonPost(baseUrl + "/update_stats", json))
        {
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Stats Synced: " + req.downloadHandler.text);
                onSuccess?.Invoke();
            }
            else
            {
                Debug.LogError("Stats Error: " + req.error);
                onError?.Invoke(req.error);
            }
        }
    }

    private IEnumerator GetLeaderboardCoroutine(Action<List<LeaderboardEntry>> onSuccess, Action<string> onError)
    {
        using (UnityWebRequest req = UnityWebRequest.Get(baseUrl + "/leaderboard"))
        {
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                LeaderboardResponse res = JsonUtility.FromJson<LeaderboardResponse>(req.downloadHandler.text);
                onSuccess?.Invoke(res.leaderboard);
            }
            else
            {
                Debug.LogError("Leaderboard Error: " + req.error);
                onError?.Invoke(req.error);
            }
        }
    }

    private UnityWebRequest CreateJsonPost(string url, string jsonBody)
    {
        UnityWebRequest req = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        return req;
    }
}
