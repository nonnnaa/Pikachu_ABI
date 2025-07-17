using UnityEngine;

public enum TileNameType
{
    Apple = 0,
    Banana = 1,
    Blueberry = 2,
    Grapes = 3,
    Orange = 4,
    Pear = 5,
    Strawberry = 6,
    TileEmpty = 100,
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
        Board.Instance.SetFruit(this);
    }

}
