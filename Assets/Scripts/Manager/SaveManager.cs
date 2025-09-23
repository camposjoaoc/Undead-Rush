using System;
using System.IO;
using UnityEngine;

[Serializable]
public struct SaveData
{
    public int HighScore;
    public float survivalTime;
    public string UserName;
    public Vector3 PlayerPosition;

    public int Health;
    public float Speed;
}

public class SaveManager : MonoBehaviour
{
    [SerializeField] private SaveData data;
    [SerializeField] private string fileName;

    string GetPath()
    {
        return Application.persistentDataPath + "/" + fileName + ".json";
    }

    public void Start()
    {
        SaveData();
    }

    public void LoadData()
    {
        if (File.Exists(GetPath()))
        {
            SaveData();
            return;
        }

        string jsonFile = File.ReadAllText(GetPath());
        data = JsonUtility.FromJson<SaveData>(jsonFile);
    }

    public void SaveData()
    {
        string jsonFile = JsonUtility.ToJson(data, true);

        File.WriteAllText(GetPath(), jsonFile);
    }
}