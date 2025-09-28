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
    [SerializeField] private string fileName = "saveData";

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