using TMPro;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private int levelId;
    [SerializeField] private LevelData levelData;
    [SerializeField] private bool isUnlock;
    public bool IsUnlock => isUnlock;
    [SerializeField] private int stars;
    [SerializeField] private SpriteRenderer[] starsSprite;
    [SerializeField] private SpriteRenderer levelSpriteRenderer;
    [SerializeField] private GameObject lockLevel;
    [SerializeField] private GameObject number;
    [SerializeField] private TextMeshProUGUI textNumber;

    private void Awake()
    {
        if (levelSpriteRenderer == null)
        {
            levelSpriteRenderer = GetComponent<SpriteRenderer>();
        }
        starsSprite ??= GetComponentsInChildren<SpriteRenderer>(); // ??= tuong duong check null
    }

    void Start()
    {
        if (levelData == null)
        {
            levelData = LevelManager.Instance.GetLevelDataByID(levelId);
        }
        OnInit();
    }

    public void OnClick()
    {
        //Debug.Log($"Click : {levelId}");
        if (UIManager.Instance.IsUIOpened<CanvasPrePlay>())
        {
            UIManager.Instance.CloseUI<CanvasPrePlay>(0f);
        }
        else
        {
            UIManager.Instance.OpenUI<CanvasPrePlay>();
        }
    }
    void OnInit()
    {
        // Hien thi sao
        for (int i = 0; i < stars; i++)
        {
            starsSprite[i].gameObject.SetActive(true);
        }
        
        // Hien thi lock image va number tren level
        lockLevel.SetActive(!isUnlock);
        number.SetActive(isUnlock);
        textNumber.text = levelId.ToString();
    }
    
    public LevelData GetLevelData()
    {
        return levelData;
    }
    public int GetLevelID() => levelId;
}
