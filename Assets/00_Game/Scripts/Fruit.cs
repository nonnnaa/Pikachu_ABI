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
}
public class Fruit : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Vector2Int coordinate;
    public TileNameType nameType;
    private Animator animator;
    public bool isChoosen;
    
    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();
        isChoosen = false;
    }
    public void SetupFruit()
    {
        isChoosen = !isChoosen;
        if (animator != null)
        {
            animator.SetBool(Constant.FruitAnim.isChosen, isChoosen);
        }
    }
    public void OnClick()
    {
        SetupFruit();
        BoardManager.Instance.SetFruit(this);
    }

}
