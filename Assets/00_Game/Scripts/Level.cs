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
        if (levelData == null)
        {
            levelData = LevelManager.Instance.GetLevelDataByID(levelId);
        }
    }

    void Start()
    {
        OnInit();
    }

    public void OnClick()
    {
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
        for (int i = 0; i < 3; i++)
        {
            starsSprite[i].gameObject.SetActive(i < stars);
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
    
    public void SetUnlock(bool unlock)
    {
        isUnlock = unlock;
        lockLevel.SetActive(!isUnlock);
        number.SetActive(isUnlock);
    }

    public void SetStars(int starCount)
    {
        if (starCount < stars) return;
        stars = starCount;
        for (int i = 0; i < 3; i++)
        {
            starsSprite[i].gameObject.SetActive(i <  stars);
        }
    }
    public int GetStars() => stars;
}
