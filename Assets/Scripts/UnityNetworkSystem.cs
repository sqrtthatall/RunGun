using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour
{
    private string baseUrl = "http://localhost:5000";

    [System.Serializable] public class AuthRequest { public string username; public string password; }
    [System.Serializable] public class StatsRequest { public string username; public int score; public float time; }

    [System.Serializable] public class LoginResponse { public string status; public int high_score; public float best_time; public string message; }
    [System.Serializable] public class LeaderboardEntry { public string username; public int score; }
    [System.Serializable] public class LeaderboardResponse { public string status; public List<LeaderboardEntry> leaderboard; }


    public void Register(string user, string pass) { StartCoroutine(RegisterCoroutine(user, pass)); }
    public void Login(string user, string pass) { StartCoroutine(LoginCoroutine(user, pass)); }
    public void SendStats(string user, int score, float time) { StartCoroutine(SendStatsCoroutine(user, score, time)); }
    public void GetLeaderboard() { StartCoroutine(GetLeaderboardCoroutine()); }



    private IEnumerator RegisterCoroutine(string user, string pass)
    {
        string json = JsonUtility.ToJson(new AuthRequest { username = user, password = pass });
        using (UnityWebRequest request = new UnityWebRequest(baseUrl + "/register", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
                Debug.Log("Register: " + request.downloadHandler.text);
            else
                Debug.LogError("Register Error: " + request.error);
        }
    }

    private IEnumerator LoginCoroutine(string user, string pass)
    {
        string json = JsonUtility.ToJson(new AuthRequest { username = user, password = pass });
        using (UnityWebRequest request = new UnityWebRequest(baseUrl + "/login", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                LoginResponse response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);
                if (response.status == "success")
                    Debug.Log($"Welcome! Your best score: {response.high_score}");
                else
                    Debug.LogError("Login failed: " + response.message);
            }
        }
    }

    private IEnumerator SendStatsCoroutine(string user, int score, float time)
    {
        string json = JsonUtility.ToJson(new StatsRequest { username = user, score = score, time = time });
        using (UnityWebRequest request = new UnityWebRequest(baseUrl + "/update_stats", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();
            Debug.Log("Stats updated: " + request.downloadHandler.text);
        }
    }

    private IEnumerator GetLeaderboardCoroutine()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(baseUrl + "/leaderboard"))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                LeaderboardResponse response = JsonUtility.FromJson<LeaderboardResponse>(request.downloadHandler.text);
                foreach (var entry in response.leaderboard)
                {
                    Debug.Log($"{entry.username}: {entry.score}");
                }
            }
        }
    }
}