using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : SingletonMono<BoardManager>
{
    #region Data
    [SerializeField] private int row, column;
    [SerializeField] public int maxSegment;
    #endregion
    
    #region Temp
    private List<List<Fruit>> currentFruitBoard;
    [SerializeField] private Fruit[] fruitsChosen =  new Fruit[2];
    [Range(0, 1)]
    private int _currentIdFruitChosen;
    [SerializeField] private Transform board;
    private float _offsetX, _offsetY;
    private Dictionary<Collider2D, Fruit> fruitDictionary;
    private Fruit[] fruitprefabs;
    private Dictionary<TileNameType, Fruit> fruitPrefabDictionary;
    #endregion
    
    #region Functions
    // Tao bang
    public void GenerateBoard()
    {
        LevelData levelData = LevelManager.Instance.GetCurrentLevelData();
        column = levelData.column;
        row = levelData.row;
        
        // Tinh toa do goc tren cung ben trai trung voi goc ma tran 2 chieu
        _offsetX = -(column / 2f) + 0.5f;
        _offsetY = -(row / 2f) + 0.5f;
        for (int x = 0; x < column; x++)
        {
            currentFruitBoard.Add(new List<Fruit>());
            for (int y = 0; y < row; y++)
            {
                if (x >= 1 && x <= column - 2  && y >= 1 && y <= row - 2)
                {
                    var fruit = Instantiate(GetRandomFruit(), new Vector3(x + _offsetX, -(y + _offsetY)), Quaternion.identity);
                    fruit.transform.SetParent(board);
                    fruit.coordinate =  new Vector2Int(y, x);
                    currentFruitBoard[x].Add(fruit);
                }
                else
                {
                    var fruit = Instantiate(GetEmptyTile(), new Vector3(x + _offsetX, -(y + _offsetY)), Quaternion.identity);
                    fruit.transform.SetParent(board);
                    fruit.coordinate =  new Vector2Int(y, x);
                    currentFruitBoard[x].Add(fruit);
                }
            }
        }
    }
    public void SetFruit(Fruit fruit)
    {
        for (int i=0 ;i<2; i++)
        {
            if (fruitsChosen[i] == fruit)
            {
                fruitsChosen[i] = null;
                return;
            }
        }
        if (!fruitsChosen.Contains(fruit))
        {
            fruitsChosen[_currentIdFruitChosen]?.SetupFruit();
        }
        fruitsChosen[_currentIdFruitChosen] =  fruit;
        
        _currentIdFruitChosen =  (_currentIdFruitChosen + 1) % 2;
    }

    // Chuyen coordinate trong ma tran 2 chieu sang toa do thuc te
    private List<Vector3> GetPositionList(List<Vector2Int> coordinates)
    {
        List<Vector3> positionList = new List<Vector3>();
        for (int i = 0; i < coordinates.Count; i++)
        {
            Vector3 position = currentFruitBoard[coordinates[i].y][coordinates[i].x].transform.position;
            Vector3 newPos = new Vector3(position.x, position.y, -1);
            positionList.Add(newPos);
        }
        return positionList;
    }
    
    
    // Load Fruit tu Resources
    void LoadAllFruits()
    {
        fruitprefabs = Resources.LoadAll<Fruit>("Fruit");
        fruitPrefabDictionary = new Dictionary<TileNameType, Fruit>();
        for (int i=0 ; i<fruitprefabs.Length; i++)
        {
            fruitPrefabDictionary[fruitprefabs[i].nameType] = fruitprefabs[i];
        }
    }

    
    // Get Fruit != Empty
    Fruit GetRandomFruit()
    {
        int id = Random.Range(0, fruitprefabs.Length);
        TileNameType type = fruitPrefabDictionary.Keys.ToArray()[id];
        while (type == TileNameType.TileEmpty)
        {
            id = Random.Range(0, fruitsChosen.Length);
            type = fruitPrefabDictionary.Keys.ToArray()[id];
        }
        return fruitPrefabDictionary[type];
    }

    // Get Empty Fruit
    Fruit GetEmptyTile() => fruitPrefabDictionary[TileNameType.TileEmpty];

    // Xoa board
    private void ClearBoard()
    {
        foreach (var fruitList in currentFruitBoard)
        {
            foreach (var fruit in fruitList)
            {
                Destroy(fruit.gameObject);
            }
        }
        currentFruitBoard.Clear();
        fruitDictionary.Clear();
    }
    
    #endregion
    
    #region LifeCycle Messages

    private void Start()
    {
        _currentIdFruitChosen = 0;
        fruitDictionary = new Dictionary<Collider2D, Fruit>(); 
        currentFruitBoard = new List<List<Fruit>>();
        LoadAllFruits();
        GameManager.Instance.OnLevelStart += GenerateBoard;
        GameManager.Instance.OnLevelStart += () =>
        {
            board.gameObject.SetActive(true);
        };
        GameManager.Instance.OnLevelEnd += () =>
        {
            board.gameObject.SetActive(false);
            ClearBoard();
        };
    }
    private void Update()
    {
        // Xu ly va cham vao Fruits
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
            
            // handle if fruitsChosen[0] = fruitsChosen[1]

            if (fruitsChosen[0] != null && fruitsChosen[1] != null &&
                fruitsChosen[0].nameType == fruitsChosen[1].nameType)
            {
                var res = BFS_Solver.BFS(currentFruitBoard, fruitsChosen[0].coordinate, fruitsChosen[1].coordinate,
                    maxSegment);
                if (res.Item1)
                {
                    fruitsChosen[0].gameObject.SetActive(false);
                    fruitsChosen[1].gameObject.SetActive(false);
                    fruitsChosen[0] = null;
                    fruitsChosen[1] = null;
                    // Ve Line noi 2 diem
                    PathLineDrawer.Instance.DrawPath(GetPositionList(res.Item2));
                }
            }
        }
    }
    #endregion
}
