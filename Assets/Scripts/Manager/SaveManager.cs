using System;
using System.IO;
using UnityEngine;

[Serializable]
public struct SaveData
{
    public int highScoreKills;
    public float bestTime;
    public string playerName;
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    private string savePath;
    private SaveData currentData;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        savePath = Path.Combine(Application.persistentDataPath, "Undead_Rush_Save.json");
    }

    [System.Serializable]
    public class SaveData
    {
        public string playerName;
        public int highScoreKills;
        public float bestTime;
    }

    public void SetHighScore(int kills, string playerName, float time)
    {
        if (currentData == null) currentData = new SaveData();
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
    }

    public SaveData GetData()
    {
        return currentData;
    }
}