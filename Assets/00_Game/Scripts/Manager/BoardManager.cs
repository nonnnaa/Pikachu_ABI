using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : SingletonMono<BoardManager>
{
    private const int maxSegment = 3;
    
    #region Temp
    private int row, column;
    private List<List<Fruit>> currentFruitBoard;
    [SerializeField] private Fruit[] fruitsChosen =  new Fruit[2];
    [Range(0, 1)] private int currentIdFruitChosen;
    private float _offsetX, _offsetY;
    [SerializeField] private Transform board;
    private Fruit[] fruitprefabs;
    private Dictionary<TileNameType, Fruit> fruitPrefabDictionary;
    private Dictionary<Collider2D, Fruit> fruitDictionary;
    private bool canInteractive;
    #endregion
    
    #region LifeCycle Messages
    private void Start()
    {
        currentIdFruitChosen = 0;
        canInteractive = true;
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
        GameManager.Instance.OnGamePause += () =>
        {
            canInteractive = false;
        };
        GameManager.Instance.OnResumeGame += () =>
        {
            canInteractive = true;
        };
    }
    private void Update()
    {
        // Xu ly va cham vao Fruits
        if (Input.GetMouseButtonDown(0))
        {
            if (!canInteractive) return;
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
                        //Debug.Log("Fruit not found");
                    }
                }
            }
            
            // handle if fruitsChosen[0] = fruitsChosen[1]

            if (CheckFruitValidToConnect(fruitsChosen[0], fruitsChosen[1]))
            {
                var res = BFS_Solver.BFS(currentFruitBoard, fruitsChosen[0].coordinate, fruitsChosen[1].coordinate,
                    maxSegment);
                if (res.Item1)
                {
                    StartCoroutine(DelayDeactiveFruit(fruitsChosen[0]));
                    StartCoroutine(DelayDeactiveFruit(fruitsChosen[1]));
                    fruitsChosen[0] = null;
                    fruitsChosen[1] = null;
                    // Ve Line noi 2 diem
                    PathLineDrawer.Instance.DrawPath(GetPositionList(res.Item2));
                }
            }
        }
    }
    #endregion
    
    #region Functions

    List<(float, float)> RandomFruitsPosition(List<List<int>> boardValue)
    {
        List<(float, float)> listPos = new List<(float, float)>();
        for (int x = 0; x < column; x++)
        {
            currentFruitBoard.Add(new List<Fruit>());
            for (int y = 0; y < row; y++)
            {
                if (IsValidCoordinate(x, y, column, row) && IsNormalFruit((TileNameType)boardValue[x][y]))
                {
                    listPos.Add((x + _offsetX, -(y + _offsetY)));
                }
            }
        }
        return listPos;
    }

    
    // Check ngoai tru canh cua Grid (board)
    bool IsValidCoordinate(int x, int y, int columnGrid, int rowGrid)
    {
        return x >= 1 && x <= columnGrid - 2 && y >= 1 && y <= rowGrid - 2;
    }
    
    
    // Tao bang
    public void GenerateBoard()
    {
        LevelData levelData = LevelManager.Instance.GetCurrentLevelData();
        List<List<int>> fruitBoardValue = levelData.GetBoardValue();
        canInteractive = true;
        column = levelData.column;
        row = levelData.row;
        
        // Tinh toa do goc tren cung ben trai trung voi goc ma tran 2 chieu
        _offsetX = -(column / 2f) + 0.5f;
        _offsetY = -(row / 2f) + 0.5f;
        
        // for (int x = 0; x < column; x++)
        // {
        //     currentFruitBoard.Add(new List<Fruit>());
        //     for (int y = 0; y < row; y++)
        //     {
        //         if (x >= 1 && x <= column - 2  && y >= 1 && y <= row - 2)
        //         {
        //             //var fruit = Instantiate(GetRandomFruit(), new Vector3(x + _offsetX, -(y + _offsetY)), Quaternion.identity);
        //             var fruit = Instantiate(GetFruit(fruitBoardValue[x][y]), new Vector3(x + _offsetX, -(y + _offsetY)), Quaternion.identity);
        //             fruit.transform.SetParent(board);
        //             fruit.coordinate =  new Vector2Int(y, x);
        //             currentFruitBoard[x].Add(fruit);
        //         }
        //         else
        //         {
        //             var fruit = Instantiate(GetEmptyTile(), new Vector3(x + _offsetX, -(y + _offsetY)), Quaternion.identity);
        //             fruit.transform.SetParent(board);
        //             fruit.coordinate =  new Vector2Int(y, x);
        //             currentFruitBoard[x].Add(fruit);
        //         }
        //     }
        // }
        List<(float, float)> coordinates = RandomFruitsPosition(fruitBoardValue);

        while (coordinates.Count > 0 && coordinates.Count % 2 == 0)
        {
            int id1 = Random.Range(0, coordinates.Count);
            (float x, float y) = coordinates[id1];
            // var fruit = Instantiate(GetRandomFruit(), new Vector3(x + _offsetX, -(y + _offsetY)), Quaternion.identity);
            //              var fruit = Instantiate(GetFruit(fruitBoardValue[x][y]), new Vector3(x + _offsetX, -(y + _offsetY)), Quaternion.identity);
            //              fruit.transform.SetParent(board);
            //              fruit.coordinate =  new Vector2Int(y, x);
            //              currentFruitBoard[x].Add(fruit);
            
        }
        
    }
    
    // Get Fruit From Value Of Data

    Fruit GetFruit(int value)
    {
        switch (value)
        {
            case 0:
                return GetRandomFruit();
            case 100:
                // Get Block
                return fruitPrefabDictionary[(TileNameType)100]; // ep kieu tu int -> enum BlockTile = 100 
            case 101:
                // Get Ice
                return fruitPrefabDictionary[(TileNameType)101];
            case 102:
                // Get Booster Bom
                return fruitPrefabDictionary[(TileNameType)102];
        }
        return GetEmptyTile();
    }
    
    // Xac dinh Fruit Hien tai 
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
            fruitsChosen[currentIdFruitChosen]?.SetupFruit();
        }
        fruitsChosen[currentIdFruitChosen] =  fruit;
        
        currentIdFruitChosen =  (currentIdFruitChosen + 1) % 2;
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
        while (!IsNormalFruit(type))
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

    // Tao delay time de fruit vs line bien mat cung thoi gian (cung 0.3s)
    IEnumerator DelayDeactiveFruit(Fruit fruit)
    {
        yield return new  WaitForSeconds(0.3f);
        fruit?.gameObject.SetActive(false);
    }

    bool IsNormalFruit(TileNameType fruitType)
    {
        return fruitType != TileNameType.TileEmpty && fruitType != TileNameType.Block &&  fruitType != TileNameType.Ice;
    }
    
    bool CheckFruitValidToConnect(Fruit fruit1, Fruit fruit2)
    {
        return fruit1 != null 
               && fruit2 != null
               && IsNormalFruit(fruit1.nameType)
               && IsNormalFruit(fruit2.nameType)
               && fruitsChosen[0].nameType == fruitsChosen[1].nameType;
    }
    #endregion
}
