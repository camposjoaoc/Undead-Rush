using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    private string savePath;
    private SaveData currentData;

    [Serializable]
    public class SaveData
    {
        public string playerName;
        public int highScoreKills;
        public float bestTime;
        public List<ScoreEntry> allScores = new();
    }

    [Serializable]
    public class ScoreEntry
    {
        public string playerName;
        public int kills;
        public float time;
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        savePath = Path.Combine(Application.persistentDataPath, "Undead_Rush_Save.json");
    }

    public void SetHighScore(int kills, string playerName, float time)
    {
        if (currentData == null) currentData = new SaveData();

        // Add current score
        currentData.allScores.Add(new ScoreEntry
        {
            playerName = playerName,
            kills = kills,
            time = time
        });

        // Update score if higher
        if (kills > currentData.highScoreKills)
        {
            currentData.playerName = playerName;
            currentData.highScoreKills = kills;
            currentData.bestTime = time;
        }

        Save();
    }

    private void Save()
    {
        string json = JsonUtility.ToJson(currentData, true);
        File.WriteAllText(savePath, json);
    }

    public void LoadData()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            currentData = JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            currentData = new SaveData();
        }
    }

    public SaveData GetData()
    {
        return currentData;
    }
}