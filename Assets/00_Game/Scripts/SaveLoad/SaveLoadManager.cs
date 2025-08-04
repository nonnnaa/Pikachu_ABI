using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class LevelProgressData
{
    public int levelId;
    public bool isUnlock;
    public int stars;
}

[System.Serializable]
public class LevelProgression
{
    public List<LevelProgressData> levels = new List<LevelProgressData>();
}

[System.Serializable]
public class UserSettings
{
    public float musicBGVolume = 1f;
    public float musicVFXVolume = 1f;
}

public class SaveLoadManager : SingletonMono<SaveLoadManager>
{
    private string progressionFilePath;
    private string settingsFilePath;

    private void Awake()
    {
        progressionFilePath = Path.Combine(Application.persistentDataPath, "progression.json");
        settingsFilePath = Path.Combine(Application.persistentDataPath, "settings.json");
    }

    // Save Level Progression
    public void SaveLevelProgress(LevelProgression progression)
    {
        string json = JsonUtility.ToJson(progression, true);
        File.WriteAllText(progressionFilePath, json);
        Debug.Log($"Progress saved to: {progressionFilePath}");
    }

    // Load Level Progression
    public LevelProgression LoadLevelProgress()
    {
        if (File.Exists(progressionFilePath))
        {
            string json = File.ReadAllText(progressionFilePath);
            LevelProgression progression = JsonUtility.FromJson<LevelProgression>(json);
            return progression;
        }
        else
        {
            Debug.Log("No progression file found, creating new progression.");
            LevelProgression newProgress = new LevelProgression();
            SaveLevelProgress(newProgress); // Save file mặc định
            return newProgress;
        }
    }

    // Save User Settings
    public void SaveSettings(UserSettings settings)
    {
        string json = JsonUtility.ToJson(settings, true);
        File.WriteAllText(settingsFilePath, json);
        Debug.Log($"User settings saved to: {settingsFilePath}");
    }

    // Load User Settings
    public UserSettings LoadSettings()
    {
        if (File.Exists(settingsFilePath))
        {
            string json = File.ReadAllText(settingsFilePath);
            return JsonUtility.FromJson<UserSettings>(json);
        }
        else
        {
            Debug.Log("No settings file found, creating default settings.");
            UserSettings defaultSettings = new UserSettings();
            SaveSettings(defaultSettings); // Save file mặc định
            return defaultSettings;
        }
    }
}
