using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class SessionData
{
    public List<Vector3> to_apple = new();
    public List<Vector3> to_basket = new();
}

public class PositionLogger : MonoBehaviour
{
    [Header("Settings")]
    public GameObject target;
    public int intervalMilliseconds = 500;

    private Coroutine loggingCoroutine;
    private enum DataCategory { ToApple, ToBasket }
    private DataCategory currentCategory = DataCategory.ToApple;

    private List<SessionData> sessionLogs = new();
    private SessionData currentSession;

    private bool isLogging = false;

    void Start()
    {
        //StartLogging(); // Optional auto-start
    }

    public void StartLogging()
    {
        if (isLogging || target == null) return;

        CreateNewSession(); // Start with a fresh session
        loggingCoroutine = StartCoroutine(LogPosition());
        isLogging = true;
    }

    public void SwitchToApple()
    {
        CreateNewSession();
        currentCategory = DataCategory.ToApple;
        Debug.Log("[Logger] Switched to: to_apple");
    }

    public void SwitchToBasket()
    {
        currentCategory = DataCategory.ToBasket;
        Debug.Log("[Logger] Switched to: to_basket");
    }

    private void CreateNewSession()
    {
        currentSession = new SessionData();
        sessionLogs.Add(currentSession);
    }

    private IEnumerator LogPosition()
    {
        var waitTime = new WaitForSeconds(intervalMilliseconds / 1000f);

        while (true)
        {
            Vector3 pos = target.transform.position;

            if (currentCategory == DataCategory.ToApple)
            {
                currentSession.to_apple.Add(pos);
                Debug.Log($"[to_apple] {pos}");
            }
            else
            {
                currentSession.to_basket.Add(pos);
                Debug.Log($"[to_basket] {pos}");
            }

            yield return waitTime;
        }
    }

    private void OnApplicationQuit()
    {
        WriteDataToFile();
    }

    private void WriteDataToFile()
    {
        string path = Path.Combine(Application.persistentDataPath, "position_log.json");

        using StreamWriter writer = new StreamWriter(path, false);
        writer.WriteLine("[");
        for (int i = 0; i < sessionLogs.Count; i++)
        {
            var session = sessionLogs[i];
            writer.WriteLine("  {");
            writer.WriteLine("    \"to_apple\": [");
            foreach (var pos in session.to_apple)
                writer.WriteLine($"      \"{pos}\",");
            writer.WriteLine("    ],");

            writer.WriteLine("    \"to_basket\": [");
            foreach (var pos in session.to_basket)
                writer.WriteLine($"      \"{pos}\",");
            writer.WriteLine("    ]");

            writer.Write(i == sessionLogs.Count - 1 ? "  }" : "  },");
        }
        writer.WriteLine("\n]");
        Debug.Log($"[Logger] Position data written to: {path}");
    }
}
