using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : SingletonMono<BoardManager>
{
    private int maxSegment = 3; // Rule Game
    [SerializeField] private Transform board;
    #region Temp
    private int row, column;
    private List<List<Fruit>> currentFruitBoard = new List<List<Fruit>>();
    private Dictionary<Vector2Int, Fruit> currentActiveFruits = new Dictionary<Vector2Int, Fruit>();
    private HashSet<(Vector2Int, Vector2Int)> currentCoupleCanConnect =  new HashSet<(Vector2Int, Vector2Int)>();
    private Fruit fruit1, fruit2;
    private float _offsetX, _offsetY;
    private Fruit[] fruitprefabs;
    private Dictionary<TileNameType, Fruit> fruitPrefabDictionary = new Dictionary<TileNameType, Fruit>();
    private Dictionary<Collider2D, Fruit> fruitDictionary = new Dictionary<Collider2D, Fruit>();
    private bool canInteractive;
    // Score
    private int connectScore;
    private int currentScore;
    #endregion
    public int GetCurrentScore() => currentScore;
    public event Action<int> UpdateScore;
    #region LifeCycle Messages
    private void Awake()
    {
        GameManager.Instance.OnLevelStart += () =>
        {
            board.gameObject.SetActive(true);
            OnInit();
            GenerateBoard();
            currentCoupleCanConnect = GetTilesCanMatch(currentFruitBoard, maxSegment);
        };
        GameManager.Instance.OnLevelEnd += () =>
        {
            board.gameObject.SetActive(false);
            Despawn();
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


    private Fruit fruitTmp;
    private Vector2 mousePosTmp;
    private RaycastHit2D hit;
    private Collider2D collider;
    private void Update()
    {
        // Xu ly va cham vao Fruits
        if (Input.GetMouseButtonDown(0))
        {
            if (!canInteractive) return;

            mousePosTmp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            hit = Physics2D.Raycast(mousePosTmp, Vector2.zero);
        
            Collider2D collider = hit.collider;
            if (collider != null)
            {
                if (fruitDictionary.ContainsKey(collider))
                {
                    OnFruitSelected(fruitDictionary[collider]);
                }
                else
                {
                    fruitTmp = hit.collider.GetComponent<Fruit>();
                    OnFruitSelected(fruitTmp);
                    fruitDictionary[collider] = fruitTmp;
                }
            }
            // handle if fruitsChosen[0].type = fruitsChosen[1].type
            if (CheckFruitValidToConnect(fruit1, fruit2))
            {
                ConnectFruits();
            }
            else
            {
                //Debug.Log("Fruits IS InValid");
            }
        }
    }
    #endregion
    
    #region Functions

    private Vector2Int cacheV2;
    List<(Vector3, Vector2Int)> RandomFruitsPosition(List<List<int>> boardValue)
    {
        List<(Vector3 ,Vector2Int)> listPos = new List<(Vector3, Vector2Int)>();
        int rowTmp = boardValue.Count;
        int columnTmp = boardValue[0].Count;
        
        for (int y = 0; y < rowTmp; y++)
        {
            for (int x = 0; x < columnTmp; x++)
            {
                if (boardValue[y][x] == -1)
                {
                    cacheV3.x = x + _offsetX;
                    cacheV3.y = -(y + _offsetY);
                    cacheV3.z = 0;
                    cacheV2.x = x;
                    cacheV2.y = y;
                    listPos.Add((cacheV3, cacheV2));
                } 
            }
        }
        return listPos;
    }
    
    List<(Vector3, Vector2Int)> RandomSpecialFruitsPosition(List<List<int>> boardValue)
    {
        List<(Vector3, Vector2Int)> listPos = new List<(Vector3, Vector2Int)>();
        int rowTmp = boardValue.Count;
        int columnTmp = boardValue[0].Count;
        
        for (int y = 0; y < rowTmp; y++)
        {
            for (int x = 0; x < columnTmp; x++)
            {
                if (boardValue[y][x] != -1)
                {
                    cacheV3.x = x + _offsetX;
                    cacheV3.y = -(y + _offsetY);
                    cacheV3.z = 0;
                    cacheV2.x = x;
                    cacheV2.y = y;
                    listPos.Add((cacheV3, cacheV2));
                } 
            }
        }
        return listPos;
    }
    
    void OnInit()
    { ;
        currentScore = 0;
        connectScore = Random.Range(10, 15);
        canInteractive = true;
        LoadAllFruits();
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

    private Vector3 cacheV3;
    LevelData levelData;
    List<List<int>> fruitBoardValue  = new List<List<int>>();
    // Tao bang
    public void GenerateBoard()
    {
        levelData = LevelManager.Instance.GetCurrentLevelData();
        fruitBoardValue = levelData.GetBoardValue();
        canInteractive = true;
        column = levelData.column;
        row = levelData.row;
        
        // Tinh toa do goc tren cung ben trai trung voi goc ma tran 2 chieu
        _offsetX = -(column / 2f) + 0.5f;
        _offsetY = -(row / 2f) + 0.5f;
        
        List<(Vector3, Vector2Int)> coordinatesRandom = RandomFruitsPosition(fruitBoardValue);
        for (int i = 0; i < row; i++)
        {
            List<Fruit> fruits = new List<Fruit>();
            for (int j = 0; j < column; j++)
            {
                fruits.Add(null);
            }
            currentFruitBoard.Add(fruits);
        }
        
        
        
        //currentFruitBoard = new List<List<Fruit>>();
        for (int i = 0; i < row; i++)
        {
            List<Fruit> rowTmp = new List<Fruit>();
            for (int j = 0; j < column; j++)
            {
                rowTmp.Add(null);
            }
            currentFruitBoard.Add(rowTmp);
        }
        
        
        // Spanw Random Fruits => Sinh theo cap
        List<FruitCount> fruitCounts = levelData.randomFruitCounts;
        foreach (var fruitCountTmp in fruitCounts)
        {
            TileNameType type = fruitCountTmp.fruitType;
            int fruitCount = fruitCountTmp.count;
            int coordinatesSize = coordinatesRandom.Count;
            while (fruitCount > 0 && coordinatesRandom.Count % 2 == 0 && coordinatesRandom.Count > 0)
            {
                // Spawn fruit1
                int id1 = Random.Range(0, coordinatesSize);
                //(float xPos1, float yPos1, int x1, int y1) 
                (cacheV3, cacheV2) = coordinatesRandom[id1];
                fruit1Tmp = Instantiate(GetFruit(type), cacheV3, Quaternion.identity);
                fruit1Tmp.transform.SetParent(board);
                fruit1Tmp.coordinate =  cacheV2;
                currentFruitBoard[cacheV2.y][cacheV2.x] =  fruit1Tmp;
                currentActiveFruits.Add(cacheV2, fruit1Tmp);
                coordinatesRandom.RemoveAt(id1);
                fruitCount--;
                coordinatesSize--;
                
                // Spawn fruit2
                int id2 = Random.Range(0, coordinatesSize);
                (cacheV3, cacheV2) = coordinatesRandom[id2];
                fruit2Tmp = Instantiate(GetFruit(type), cacheV3, Quaternion.identity);
                fruit2Tmp.transform.SetParent(board);
                fruit2Tmp.coordinate =  cacheV2;
                currentFruitBoard[cacheV2.y][cacheV2.x] =  fruit2Tmp;
                currentActiveFruits.Add(cacheV2,fruit2Tmp);
                coordinatesRandom.RemoveAt(id2);
                fruitCount--;
                coordinatesSize--;
            }
        }
        // Spawn Other Fruit Not from Random
        List<(Vector3, Vector2Int)> coordinatesSpecial = RandomSpecialFruitsPosition(fruitBoardValue);
        foreach (var item in coordinatesSpecial)
        {
            (cacheV3, cacheV2) = item;
            fruitTmp = Instantiate(GetFruit((TileNameType)fruitBoardValue[cacheV2.y][cacheV2.x]), cacheV3, Quaternion.identity);
            fruitTmp.transform.SetParent(board);
            fruitTmp.coordinate =  cacheV2;
            currentFruitBoard[cacheV2.y][cacheV2.x] =  fruitTmp;
        }
    }
    
    // Get Fruit From Value Of Data
    Fruit GetFruit(TileNameType type)
    {
        return fruitPrefabDictionary[type];
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
    
    // Check Fruit can Random
    bool IsNormalFruit(TileNameType fruitType)
    {
        return fruitType != TileNameType.TileEmpty && fruitType != TileNameType.Block &&  fruitType != TileNameType.Ice;
    }
    
    //Check Condition to Connect
    bool CheckFruitValidToConnect(Fruit fruit1, Fruit fruit2)
    {
        return fruit1 != null 
               && fruit2 != null
               && fruit1.gameObject.activeInHierarchy
               && fruit2.gameObject.activeInHierarchy
               && IsNormalFruit(fruit1.nameType)
               && IsNormalFruit(fruit2.nameType)
               && fruit1.coordinate != fruit2.coordinate
               && fruit1.nameType == fruit2.nameType;
    }
    
    // Xoa board
    private void Despawn()
    {
        currentActiveFruits.Clear();
        currentFruitBoard.Clear();
        fruitDictionary.Clear();
        currentCoupleCanConnect.Clear();
    }
    
    private HashSet<(Vector2Int, Vector2Int)> GetTilesCanMatch(List<List<Fruit>> currentBoard, int valueSegment)
    {
        if (currentBoard == null)
        {
            Debug.Log("CurrentBoard is null");
            return null;
        }
        HashSet<(Vector2Int, Vector2Int)> result = new HashSet<(Vector2Int, Vector2Int)>();
        
        List<List<Fruit>> fruitBoard = currentBoard;
        
        int rowBoard = fruitBoard.Count;
        int columnBoard = fruitBoard[1].Count;
        for (int i1=0 ; i1<rowBoard; i1++)
        {
            for (int j1=0 ; j1<columnBoard; j1++)
            {
                bool isFound = false;
                for (int i2 = 0; i2 < rowBoard; i2++)
                {
                    for (int j2 = 0; j2 < columnBoard; j2++)
                    {
                        if (!CheckFruitValidToConnect(currentBoard[i1][j1], currentBoard[i2][j2]) 
                            || result.Contains((new Vector2Int(i1, j1), new Vector2Int(i2, j2)))
                            || result.Contains((new Vector2Int(i2, j2), new Vector2Int(i1, j1))))
                        {
                            continue;
                        }
                        (bool check, List<Vector2Int> path) = BFS_Solver.BFS(currentBoard,
                              fruitBoard[i1][j1].coordinate, fruitBoard[i2][j2].coordinate, valueSegment);
                        if (check)
                        { 
                            result.Add((new Vector2Int(j1, i1), new Vector2Int(j2, i2)));
                            isFound = true;
                            break;
                        }
                    }
                    if (isFound)
                    { 
                        break;
                    }
                }
            }
        }
        return result;
    }

    void UpdateCurrentCoupleCanConnect(Vector2Int coordinate1, Vector2Int coordinate2)
    {
        if (currentCoupleCanConnect == null)
        {
            Debug.Log("CurrentCoupleCanConnect is null");
            return;
        }
        if (currentCoupleCanConnect.Contains((coordinate1, coordinate2)))
        {
            currentCoupleCanConnect.Remove((coordinate1, coordinate2));
        }
        if (currentCoupleCanConnect.Contains((coordinate2, coordinate1)))
        {
            currentCoupleCanConnect.Remove((coordinate2, coordinate1));
        }
    }

    void UpdateCurrentActiveFruits(Dictionary<Vector2Int, Fruit> hashSet, Fruit fr1, Fruit fr2)
    {
        if (hashSet == null)
        {
            Debug.Log("hashSet is null");
            return;
        }
        hashSet.Remove(fr1.coordinate);
        hashSet.Remove(fr2.coordinate);
    }

    void ConnectFruits()
    {
        var res = BFS_Solver.BFS(currentFruitBoard, fruit1.coordinate, fruit2.coordinate,
            maxSegment);
        if (res.Item1)
        {
            fruit1.OnConnected();
            fruit2.OnConnected();
            UpdateCurrentCoupleCanConnect(fruit1.coordinate, fruit2.coordinate);
            UpdateCurrentActiveFruits(currentActiveFruits, fruit1, fruit2);
            currentCoupleCanConnect = GetTilesCanMatch(currentFruitBoard, maxSegment);
            fruit1 = null;
            fruit2 = null;
            
            // Ve Line noi 2 diem
            PathLineDrawer.Instance.DrawPath(GetPositionList(res.Item2));
            SoundManager.Instance.SetMusicVFX(SoundVFXType.SoundGetPoint);
                    
            currentScore += Random.Range(connectScore, connectScore * 2);
            UpdateScore?.Invoke(currentScore);
                    
            if (currentActiveFruits.Count <= 0)
            {
                GameManager.Instance.EndLevel();
                GameManager.Instance.WinGame();
            }
        }
    }


    private Fruit fruit1Tmp, fruit2Tmp;
    private int id1, id2;
    void ShuffleBoardWithActiveFruits(Dictionary<Vector2Int, Fruit> currentFruitActives)
    {
        List<Fruit> fruitList = currentFruitActives.Values.ToList();
        while (fruitList.Count > 0 &&  fruitList.Count % 2 == 0)
        {
            id1 =  Random.Range(0, fruitList.Count);
            fruit1Tmp = fruitList[id1];
            fruitList.RemoveAt(id1);
            
            
            id2 = Random.Range(0, fruitList.Count);
            fruit2Tmp = fruitList[id2];
            fruitList.RemoveAt(id2);
            
            SwapFruitData(fruit1Tmp, fruit2Tmp);
        }
    }
    
    void SwapFruitData(Fruit fruitA, Fruit fruitB)
    {
        // Swap sprite
        (fruitA.spriteRenderer.sprite, fruitB.spriteRenderer.sprite) = (fruitB.spriteRenderer.sprite, fruitA.spriteRenderer.sprite);
        // Swap nameType
        (fruitA.nameType, fruitB.nameType) = (fruitB.nameType, fruitA.nameType);
    }
    
    
    public (Vector2Int, Vector2Int) GetSuggest()
    {
        (Vector2Int, Vector2Int)  result = (new Vector2Int(), new Vector2Int());
        foreach (var couple in currentCoupleCanConnect)
        {
            result = couple;
            break;
        }
        return result;
    }
    
    
    // Call From BoosterBase
    public void Shuffle()
    {
        ShuffleBoardWithActiveFruits(currentActiveFruits);
        currentCoupleCanConnect = GetTilesCanMatch(currentFruitBoard, maxSegment);
    }

    public void Suggest()
    {
        (Vector2Int key1, Vector2Int key2) = GetSuggest();
        if (key1 == key2 && key2 == Vector2Int.zero)
        {
            Debug.Log("NO PAIR OF FRUITS TO CONNECT.");
            return;
        }
            
        if (currentActiveFruits == null)
        {
            Debug.Log("currentActiveFruits is null");
            return;
        }
        Debug.Log(key1 + ", " + key2); 
        fruit1Tmp = currentActiveFruits[key1];
        fruit2Tmp = currentActiveFruits[key2];
        if (fruit1Tmp != null)
        {
            fruit1Tmp.OnSelected();
        }
        if (fruit2Tmp != null)
        {
            fruit2Tmp.OnSelected();
        }
        ConnectFruits();
    }

    void OnFruitSelected(Fruit fruit)
    {
        if (fruit1 != null)
        {
            fruit1.OnDeselected();
        }
        if (fruit2 != null)
        {
            fruit2.OnDeselected();
        }

        if (fruit != null)
        {
            if (fruit1 == null)
            {
                fruit1 = fruit;
            }
            else if (fruit2 == null)
            {
                fruit2 = fruit;
            }
            else
            {
                fruit1 = fruit2;
                fruit2 = fruit;
            }
            fruit.OnSelected();
        }
        else
        {
            //Debug.Log("No Fruit Selected");
        }
    }
    #endregion
}
