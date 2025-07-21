using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int currentLevel;
    public int highScore;
    public bool soundOn;
    public float volume;
    public bool musicOn;
    public float musicVolume;
}

public class DataManager : SingletonMono<DataManager>
{
    private const string PlayerDataKey = "PLAYER_DATA";
    public PlayerData Data { get; private set; }

    private void Awake()
    {
        LoadData();
    }

    private void LoadData()
    {
        if (PlayerPrefs.HasKey(PlayerDataKey))
        {
            string json = PlayerPrefs.GetString(PlayerDataKey);
            Data = JsonUtility.FromJson<PlayerData>(json);
        }
        else
        {
            Data = new PlayerData()
            {
                currentLevel = 0,
                highScore = 0,
                soundOn = true,
                volume = 0f,
                musicOn = true,
                musicVolume = 0f
            };
        }
    }

    private void SaveData()
    {
        string json = JsonUtility.ToJson(Data);
        PlayerPrefs.SetString(PlayerDataKey, json);
        PlayerPrefs.Save();
    }

    private void ResetData()
    {
        PlayerPrefs.DeleteKey(PlayerDataKey);
        LoadData();
    }
}