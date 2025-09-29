using System;
using System.IO;
using UnityEngine;

[Serializable]
public struct SaveData
{
    public int highScore;
    public float survivalTime;
    public string userName;
}

public class SaveManager : MonoBehaviour
{
    [SerializeField] private SaveData data;
    [SerializeField] private string fileName;

    public int GetHighScore => data.highScore;

    string GetPath()
    {
        return Application.persistentDataPath + "/" + fileName + ".json";
    }

    public void LoadData()
    {
        if (File.Exists(GetPath()))
        {
            SaveGameFile();
            return;
        }

        string jsonFile = File.ReadAllText(GetPath());
        data = JsonUtility.FromJson<SaveData>(jsonFile);
    }

    public void SaveGameFile()
    {
        string jsonFile = JsonUtility.ToJson(data, true);

        File.WriteAllText(GetPath(), jsonFile);
    }

    public void SetHighScore(int someScore)
    {
        if (data.highScore > someScore) return;

        data.highScore = someScore;
        SaveGameFile();
    }
}