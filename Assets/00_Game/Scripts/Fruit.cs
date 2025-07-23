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
    Strawbery = 7,
    
    
    Block = 100,
    Ice = 101,
}
public class Fruit : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Vector2Int coordinate;
    public TileNameType nameType;
    private Animator animator;
    public bool isChoosen;
    private bool isInteractive;

    public void SetInteractive(bool value)
    {
        isInteractive = value;
    }
    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();
        isChoosen = false;
        isInteractive = true;
    }
    public void SetupFruit()
    {
        isChoosen = !isChoosen;
        if (animator.runtimeAnimatorController != null)
        {
            animator.SetBool(Constant.FruitAnim.isChosen, isChoosen);
        }
    }
    
    public void OnClick()
    {
        if (!isInteractive) return;
        SetupFruit();
        BoardManager.Instance.SetFruit(this);
    }
}
