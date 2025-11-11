using System.Collections.Generic;
using UnityEngine;

public class LevelManager : SingletonMono<LevelManager>
{

    [SerializeField] private string levelDataPath = "LevelData";
    private Dictionary<int,LevelData> levelDataMapping = new Dictionary<int, LevelData>();
    [SerializeField] private Transform levelParent;
    
    private int currentLevelID;
    private Dictionary<Collider2D, Level> levelColliderMapping;
    private LevelData currentLevelData;
    private Level currentLevel;
    private Dictionary<int,Level> levelMapping;
    private bool canInteractive;
    public int CurrentLevelID => currentLevelID;
    
    private LevelProgression progression;
    
    private void LoadProgression()
    {
        progression = SaveLoadManager.Instance.LoadLevelProgress();
        if (progression.levels.Count == 0)
        {
            foreach (var kvp in levelMapping)
            {
                var data = new LevelProgressData
                {
                    levelId = kvp.Key,
                    isUnlock = (kvp.Key == 1),
                    stars = 0
                };
                progression.levels.Add(data);
            }
            SaveProgression(); // Save lại file khởi tạo
        }
    }
    public bool HasLevel(int levelId) => levelMapping.ContainsKey(levelId);
    public Level GetLevel(int levelId) => levelMapping[levelId];

    private void ApplyProgressionToLevels()
    {
        foreach (var data in progression.levels)
        {
            if (levelMapping.ContainsKey(data.levelId))
            {
                levelMapping[data.levelId].SetUnlock(data.isUnlock);
                levelMapping[data.levelId].SetStars(data.stars);
            }
        }
    }

    public void SaveProgression()
    {
        // Cập nhật lại progression từ Level instances trong scene
        foreach (var data in progression.levels)
        {
            if (levelMapping.TryGetValue(data.levelId, out Level level))
            {
                data.isUnlock = level.IsUnlock;
                data.stars = level.GetStars();
            }
        }
        SaveLoadManager.Instance.SaveLevelProgress(progression);
    }
    public LevelData GetCurrentLevelData()
    {
        return currentLevelData;
    }
    
    private void Awake()
    {
        LoadAllLevelData();
        GetAllLevelInMap(); ;
        LoadProgression(); // <-- PHẢI gọi chỗ này
    
        ApplyProgressionToLevels();
        levelColliderMapping =  new Dictionary<Collider2D, Level>();
        GameManager.Instance.OnLevelStart += () =>
        {
            levelParent.gameObject.SetActive(false);
            canInteractive = false;
        };
        GameManager.Instance.OnMainMenu += () =>
        {
            levelParent.gameObject.SetActive(true);
            canInteractive = true;
            
        };
        GameManager.Instance.OnGameWin += OnGameWinHandler;
    }
    private void OnGameWinHandler()
    {
        currentLevel.SetStars(TimeManager.Instance.CurrentStar);
        Debug.Log(currentLevel.GetStars());
        int nextLevelId = currentLevel.GetLevelID() + 1;
        if (HasLevel(nextLevelId))
        {
            var nextLevel = GetLevel(nextLevelId);
            if (!nextLevel.IsUnlock)
            {
                nextLevel.SetUnlock(true);
            }
        }
        SaveProgression();
    }
    void LoadAllLevelData()
    {
        var levelDatas = Resources.LoadAll<LevelData>(levelDataPath);
        foreach (var levelData in levelDatas)
        {
            levelDataMapping[levelData.levelId] = levelData;
        }
    }
    void GetAllLevelInMap()
    {
        Level[] levels = levelParent.transform.GetComponentsInChildren<Level>();
        levelMapping = new Dictionary<int, Level>();
        for (int i = 0; i < levels.Length; i++)
        {
            levelMapping[levels[i].GetLevelID()] = levels[i];
        }
    }
    public void LoadLevel(int index)
    {
        currentLevelID = index;
        currentLevelData = GetLevelDataByID(currentLevelID);
    }
    public LevelData GetLevelDataByID(int id)
    {
        return levelDataMapping[id]; 
    }
    public Level GetCurrentLevel() => levelMapping[currentLevelID];
    private Collider2D colliderCache;
    private void Update()
    {
        // Xu ly va cham vao Level
        if (Input.GetMouseButtonDown(0))
        {
            if (!canInteractive) return;
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            colliderCache = hit.collider;
            if (colliderCache != null)
            {
                if (levelColliderMapping.ContainsKey(colliderCache))
                {
                    currentLevelID = levelColliderMapping[colliderCache].GetLevelID();
                    currentLevelData = levelColliderMapping[colliderCache].GetLevelData();
                    levelColliderMapping[colliderCache]?.OnClick();
                    currentLevel = levelColliderMapping[colliderCache];
                }
                else
                {
                    currentLevel = hit.collider.GetComponent<Level>();
                    if (currentLevel != null)
                    {
                        levelColliderMapping[colliderCache] = currentLevel;
                        currentLevelID = levelColliderMapping[colliderCache].GetLevelID();
                        currentLevelData = levelColliderMapping[colliderCache].GetLevelData();
                        currentLevel.OnClick(); 
                    }
                    else
                    {
                        Debug.Log("Level not found");
                    }
                }
            }
        }
    }
}
