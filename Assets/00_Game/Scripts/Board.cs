using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : SingletonMono<Board>
{
    #region Data
    public int row, column;
    public int maxSegment;
    #endregion
    
    #region Temp
    private List<List<Fruit>> currentFruit;
    private int _fruitCount;
    [SerializeField] private Fruit[] fruitsChoosed =  new Fruit[2];
    [Range(0, 1)]
    private int _currentIdFruitChoosed;
    [SerializeField] private Transform fruitsParent;
    private float _offsetX, _offsetY;
    private Vector2 boardOrigin;
    private Dictionary<Collider2D, Fruit> fruitDictionary;
    #endregion
    
    #region Functions
    void SpawnBoard()
    {
        currentFruit = new List<List<Fruit>>();
        for (int x = 0; x < column; x++)
        {
            currentFruit.Add(new List<Fruit>());
            for (int y = 0; y < row; y++)
            {
                if (x >= 1 && x <= column - 2  && y >= 1 && y <= row - 2)
                {
                    var fruit = Instantiate(GameManager.Instance.GetRandomFruit(), new Vector3(x + _offsetX, -(y + _offsetY)), Quaternion.identity);
                    fruit.transform.SetParent(fruitsParent);
                    fruit.coordinate =  new Vector2Int(y, x);
                    currentFruit[x].Add(fruit);
                }
                else
                {
                    var fruit = Instantiate(GameManager.Instance.GetEmptyTile(), new Vector3(x + _offsetX, -(y + _offsetY)), Quaternion.identity);
                    fruit.transform.SetParent(fruitsParent);
                    fruit.coordinate =  new Vector2Int(y, x);
                    currentFruit[x].Add(fruit);
                }
            }
        }
    }
    public void SetFruit(Fruit fruit)
    {
        for (int i=0 ;i<2; i++)
        {
            if (fruitsChoosed[i] == fruit)
            {
                fruitsChoosed[i] = null;
                return;
            }
        }
        if (!fruitsChoosed.Contains(fruit))
        {
            fruitsChoosed[_currentIdFruitChoosed]?.SetupFruit();
        }
        fruitsChoosed[_currentIdFruitChoosed] =  fruit;
        
        _currentIdFruitChoosed =  (_currentIdFruitChoosed + 1) % 2;
    }

    private List<Vector3> GetPositionList(List<Vector2Int> coordinates)
    {
        List<Vector3> positionList = new List<Vector3>();
        for (int i = 0; i < coordinates.Count; i++)
        {
            Vector3 position = currentFruit[coordinates[i].y][coordinates[i].x].transform.position;
            Vector3 newPos = new Vector3(position.x, position.y, -1);
            positionList.Add(newPos);
        }
        return positionList;
    }
    #endregion
    
    #region LifeCycle Messages

    void Start()
    {
        _offsetX = -(column / 2f) + 0.5f;
        _offsetY = -(row / 2f) + 0.5f;
        boardOrigin = new Vector2(
            -(column / 2f) + 0.5f,
            (row / 2f) - 0.5f
        );
        _currentIdFruitChoosed = 0;
        _fruitCount =  GameManager.Instance.fruits.Count;
        fruitDictionary = new Dictionary<Collider2D, Fruit>();
        SpawnBoard();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            
            Collider2D collider = hit.collider;
            if (collider != null)
            {
                if (fruitDictionary.ContainsKey(collider))
                {
                    fruitDictionary[collider].OnClick();
                }
                else
                {
                    Fruit fruitCPN = hit.collider.GetComponent<Fruit>();
                    if (fruitCPN != null)
                    {
                        fruitCPN.OnClick();
                        fruitDictionary[collider] = fruitCPN;
                    }
                    else
                    {
                        Debug.Log("Fruit not found");
                    }
                }
            }
            
            // handle if fruitsChoosed[0] = fruitsChoosed[1]

            if (fruitsChoosed[0] != null && fruitsChoosed[1] != null &&
                fruitsChoosed[0].nameType == fruitsChoosed[1].nameType)
            {
                var res = BFS_Solver.BFS(currentFruit, fruitsChoosed[0].coordinate, fruitsChoosed[1].coordinate,
                    maxSegment);
                if (res.Item1)
                {
                    fruitsChoosed[0].gameObject.SetActive(false);
                    fruitsChoosed[1].gameObject.SetActive(false);
                    fruitsChoosed[0] = null;
                    fruitsChoosed[1] = null;
                    PathLineDrawer.Instance.DrawPath(GetPositionList(res.Item2), 1);
                }
            }
        }
    }
    #endregion
    
}
