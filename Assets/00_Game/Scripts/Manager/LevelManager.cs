using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : SingletonMono<LevelManager>
{

    [SerializeField] private string levelDataPath = "LevelData";
    [SerializeField] private LevelData[] levelDatas;
    [SerializeField] private Transform levelParent;
    
    private int currentLevelID;
    private Dictionary<Collider2D, Level> levelDataMapping;
    private LevelData currentLevelData;
    private Dictionary<int,Level> levelMapping;
    
    public int CurrentLevelID => currentLevelID;
    public LevelData GetCurrentLevelData()
    {
        return currentLevelData;
    }
    
    private void Start()
    {
        LoadAllLevelData();
        GetAllLevelInMap();
        //OnInit();
        levelDataMapping =  new Dictionary<Collider2D, Level>();
        GameManager.Instance.OnLevelStart += () =>
        {
            levelParent.gameObject.SetActive(false);
        };
        GameManager.Instance.OnMainMenu += () =>
        {
            levelParent.gameObject.SetActive(true);
        };
    }
    
    void LoadAllLevelData()
    {
        levelDatas = Resources.LoadAll<LevelData>(levelDataPath);
    }
    void GetAllLevelInMap()
    {
        Level[] levels = levelParent.transform.GetComponentsInChildren<Level>();
        //Debug.Log(levels.Length);
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
        if (id > 0)
        {
            return levelDatas[id - 1];
        }
        return null;
    }
    public Level GetCurrentLevel() => levelMapping[currentLevelID];
    
    public Level GetLevelByLevelId(int id)
    {
        return levelMapping[id];
    }
    
    
    
    private void Update()
    {
        // Xu ly va cham vao Level
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            Collider2D collider = hit.collider;
            if (collider != null)
            {
                if (levelDataMapping.ContainsKey(collider))
                {
                    currentLevelID = levelDataMapping[collider].GetLevelID();
                    currentLevelData = levelDataMapping[collider].GetLevelData();
                    levelDataMapping[collider]?.OnClick();
                }
                else
                {
                    Level level = hit.collider.GetComponent<Level>();
                    if (level != null)
                    {
                        levelDataMapping[collider] = level;
                        currentLevelID = levelDataMapping[collider].GetLevelID();
                        currentLevelData = levelDataMapping[collider].GetLevelData();
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
