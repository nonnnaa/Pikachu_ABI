using System.Collections;
using UnityEngine;

public enum TileNameType
{
    TileEmpty = 0,
    Apple = 1,
    Banana = 2,
    Blueberry = 3,
    Grapes = 4,
    Orange = 5,
    Pear = 6,
    Strawberry = 7,
    ChestNut = 8,
    Cherry = 9,
    CandyRed1 = 10,
    CandyRed2 = 11,
    CandyRed3 = 12,
    CandyRed4 = 13,
    CandyBlue1 = 14,
    CandyBlue2 = 15,
    CandyBlue3 = 16,
    CandyBlue4 = 17,
    CandyGreen1 = 18,
    CandyGreen2 = 19,
    CandyGreen3 = 20,
    CandyGreen4 = 21,
    CandyYellow1 = 22,
    CandyYellow2 = 23,
    CandyYellow3 = 24,
    CandyYellow4 = 25,
    CandyPurple1 = 26,
    CandyPurple2 = 27,
    CandyPurple3 = 28,
    CandyPurple4 = 29,
    CandyOrange1 = 30,
    CandyOrange2 = 31,
    CandyOrange3 = 32,
    CandyOrange4 = 33,
    Block = 100,
    Ice = 101,
}
public class Fruit : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform outlineTransform;
    [SerializeField] private TileNameType nameType;
    [SerializeField] private Animator animator;
    private Vector2Int coordinate;
    private bool isInteractive;
    
    // Property để truy cập trực tiếp SpriteRenderer
    public SpriteRenderer SpriteRenderer => spriteRenderer;

    
    public RuntimeAnimatorController GetAnimatorController() => animator.runtimeAnimatorController;

    public void SetAnimatorController(RuntimeAnimatorController controller)
    {
        animator.runtimeAnimatorController = controller;
    }
    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }
    public Sprite GetSprite() => spriteRenderer.sprite;
    public TileNameType NameType => nameType;

    public void SetNameType(TileNameType inNameType)
    {
        nameType = inNameType;
    }
    
    // Hàm debug để kiểm tra sprite
    public void DebugSpriteInfo()
    {
        if (spriteRenderer != null)
        {
            Debug.Log($"Fruit {nameType}: Sprite = {spriteRenderer.sprite?.name}, SortingOrder = {spriteRenderer.sortingOrder}, Enabled = {spriteRenderer.enabled}");
        }
        else
        {
            Debug.LogWarning($"Fruit {nameType}: SpriteRenderer is null!");
        }
    }
    
    public Vector2Int Coordinate => coordinate;
    public Vector2Int SetCoordinate(Vector2Int value) => coordinate = value;
    private void Awake()
    {
        OnInit();
    }
    private void OnInit()
    {
        isInteractive = true;
    }
    public void OnSelected()
    {
        if (NameType == TileNameType.TileEmpty) return;
        outlineTransform.gameObject.SetActive(true);
        if (animator != null)
        {
            animator.SetTrigger(Constant.FruitAnim.select);
        }
    }
    public void OnConnected()
    {
        if (NameType == TileNameType.TileEmpty) return;
        isInteractive = false;
        if (animator != null)
        {
            animator.ResetTrigger(Constant.FruitAnim.connect);
            animator.SetTrigger(Constant.FruitAnim.connect);
        }
        ShowAnimConnect();
    }
    public void OnDeselected()
    {
        if (NameType == TileNameType.TileEmpty) return;
        outlineTransform.gameObject.SetActive(false);
        if (animator != null)
        {
            animator.SetTrigger(Constant.FruitAnim.deSelect);
        }
    }
    
    public void ShowAnimConnect()
    {
        if (NameType == TileNameType.TileEmpty) return;
        StartCoroutine(DelayAnim());
    }
    
    IEnumerator DelayAnim()
    {
        yield return new  WaitForSeconds(0.3f);
        gameObject.SetActive(false); 
    }
    
}
