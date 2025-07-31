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
    private Dictionary<int,Level> levelMapping;
    private bool canInteractive;
    public int CurrentLevelID => currentLevelID;
    public LevelData GetCurrentLevelData()
    {
        return currentLevelData;
    }
    
    private void Awake()
    {
        LoadAllLevelData();
        GetAllLevelInMap();
        //OnInit();
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
        return levelDataMapping[id]; ;
    }
    public Level GetCurrentLevel() => levelMapping[currentLevelID];
    
    private void Update()
    {
        // Xu ly va cham vao Level
        if (Input.GetMouseButtonDown(0))
        {
            if (!canInteractive) return;
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            Collider2D collider = hit.collider;
            if (collider != null)
            {
                if (levelColliderMapping.ContainsKey(collider))
                {
                    currentLevelID = levelColliderMapping[collider].GetLevelID();
                    currentLevelData = levelColliderMapping[collider].GetLevelData();
                    levelColliderMapping[collider]?.OnClick();
                }
                else
                {
                    Level level = hit.collider.GetComponent<Level>();
                    if (level != null)
                    {
                        levelColliderMapping[collider] = level;
                        currentLevelID = levelColliderMapping[collider].GetLevelID();
                        currentLevelData = levelColliderMapping[collider].GetLevelData();
                        level.OnClick(); 
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
