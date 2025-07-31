using System;
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
    private float offsetX, offsetY;
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
        LoadAllFruits();
        GameManager.Instance.OnLevelStart += () =>
        {
            board.gameObject.SetActive(true);
            OnInit();
            GenerateBoard();
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
    private Collider2D colliderCache;
    private void Update()
    {
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
            if (Utilities.IsFruitsValidToConnect(fruit1, fruit2))
            {
                ConnectFruits();
            }
        }
    }
    #endregion
    
    #region Functions
    private Vector2Int cacheV2;
    (List<(Vector3, Vector2Int)> ,List<(Vector3, Vector2Int)>)  GetFruitInforFromData(List<List<int>> boardValue)
    {
        List<(Vector3, Vector2Int)> listPos1 = new List<(Vector3, Vector2Int)>();
        List<(Vector3, Vector2Int)> listPos2 = new List<(Vector3, Vector2Int)>();
        for (int y = 0; y < row; y++)
        {
            for (int x = 0; x < column; x++)
            {
                cacheV3.x = x + offsetX;
                cacheV3.y = -(y + offsetY);
                cacheV3.z = 0;
                cacheV2.x = x;
                cacheV2.y = y;
                if (boardValue[y][x] == -1)
                {
                    listPos1.Add((cacheV3, cacheV2));
                } 
                else
                {
                    listPos2.Add((cacheV3, cacheV2));
                }
            }
        }
        return (listPos1,  listPos2);
    }
    
    void OnInit()
    { ;
        currentScore = 0;
        connectScore = Random.Range(10, 15);
        canInteractive = true;
        levelData = LevelManager.Instance.GetCurrentLevelData();
        fruitBoardValue = levelData.GetBoardValue();
        canInteractive = true;
        column = levelData.column;
        row = levelData.row;
        // Tinh toa do goc tren cung ben trai trung voi goc ma tran 2 chieu
        offsetX = -(column / 2f) + 0.5f;
        offsetY = -(row / 2f) + 0.5f;
    }
    
    // Load Fruit tu Resources
    void LoadAllFruits()
    {
        Fruit[] fruitprefabs = Resources.LoadAll<Fruit>("Fruit");
        fruitPrefabDictionary = new Dictionary<TileNameType, Fruit>();
        for (int i=0 ; i<fruitprefabs.Length; i++)
        {
            fruitPrefabDictionary[fruitprefabs[i].NameType] = fruitprefabs[i];
        }
    }

    private Vector3 cacheV3;
    LevelData levelData;
    List<List<int>> fruitBoardValue  = new List<List<int>>();
    List<(Vector3, Vector2Int)> inforListCache1 = new List<(Vector3, Vector2Int)>();
    List<(Vector3, Vector2Int)> inforListCache2 = new List<(Vector3, Vector2Int)>();
    List<FruitCount> fruitCounts =  new List<FruitCount>();
    // Tao bang
    public void GenerateBoard()
    {
        ResizeCurrentBoard();
        inforListCache1.Clear();
        inforListCache2.Clear();
        (inforListCache1, inforListCache2) = GetFruitInforFromData(fruitBoardValue); ;
        fruitCounts = levelData.randomFruitCounts;
        foreach (var fruitCountTmp in fruitCounts)
        {
            TileNameType type = fruitCountTmp.fruitType;
            int fruitCount = fruitCountTmp.count;
            int coordinatesSize = inforListCache1.Count;
            while (fruitCount > 0 && inforListCache1.Count % 2 == 0 && inforListCache1.Count > 0)
            {
                // Spawn fruit1
                id1 = Random.Range(0, coordinatesSize);
                (cacheV3, cacheV2) = inforListCache1[id1];
                SpawnFruit(type, cacheV2, cacheV3, true);
                inforListCache1.RemoveAt(id1);
                fruitCount--;
                coordinatesSize--;
                
                // Spawn fruit2
                id2 = Random.Range(0, coordinatesSize);
                (cacheV3, cacheV2) = inforListCache1[id2];
                SpawnFruit(type, cacheV2, cacheV3, true);
                inforListCache1.RemoveAt(id2);
                fruitCount--;
                coordinatesSize--;
            }
        }
        // Spawn Other Fruit Not from Random ;
        foreach (var item in inforListCache2)
        {
            (cacheV3, cacheV2) = item;
            SpawnFruit((TileNameType)fruitBoardValue[cacheV2.y][cacheV2.x], cacheV2, cacheV3, false);
        }
        currentCoupleCanConnect = GetTilesCanMatch();
    }
    // Spawn Fruit
    private void SpawnFruit(TileNameType fruitType, Vector2Int coordinates, Vector3 position, bool isRandomFruit)
    {
        fruitTmp = Instantiate(GetFruit(fruitType), position, Quaternion.identity);
        fruitTmp.transform.SetParent(board);
        fruitTmp.SetCoordinate(coordinates);
        currentFruitBoard[coordinates.y][coordinates.x] =  fruitTmp;
        if(isRandomFruit) currentActiveFruits.Add(coordinates,fruitTmp);
    }
    // Resize CurrentBoardFruit
    private void ResizeCurrentBoard()
    {
        currentFruitBoard.Clear();
        for (int i = 0; i < row; i++)
        {
            List<Fruit> fruits = new List<Fruit>();
            for (int j = 0; j < column; j++)
            {
                fruits.Add(null);
            }
            currentFruitBoard.Add(fruits);
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
    private void Despawn()
    {
        currentActiveFruits.Clear();
        fruitDictionary.Clear();
        currentCoupleCanConnect.Clear();
        foreach (var item in currentFruitBoard)
        {
            foreach (var fruit in item)
            {
                Destroy(fruit.gameObject);
            }
        }
        currentFruitBoard.Clear();
    }
    HashSet<(Vector2Int, Vector2Int)> currentCoupleCache = new HashSet<(Vector2Int, Vector2Int)>();
    private HashSet<(Vector2Int, Vector2Int)> GetTilesCanMatch()
    {
        currentCoupleCache.Clear();
        for (int i1=0 ; i1<row; i1++)
        {
            for (int j1=0 ; j1<column; j1++)
            {
                bool isFound = false;
                for (int i2 = 0; i2 < row; i2++)
                {
                    for (int j2 = 0; j2 < column; j2++)
                    {
                        if (!Utilities.IsFruitsValidToConnect(currentFruitBoard[i1][j1], currentFruitBoard[i2][j2])
                            ||!currentActiveFruits.ContainsKey(new Vector2Int(j2, i2))
                            ||currentCoupleCache.Contains((new Vector2Int(i1, j1), new Vector2Int(i2, j2)))
                            ||currentCoupleCache.Contains((new Vector2Int(i2, j2), new Vector2Int(i1, j1))))
                        {
                            continue;
                        }
                        (bool check, List<Vector2Int> path) = BFS_Solver.BFS(currentFruitBoard,
                            currentFruitBoard[i1][j1].Coordinate, currentFruitBoard[i2][j2].Coordinate, maxSegment);
                        if (check)
                        { 
                            currentCoupleCache.Add((new Vector2Int(j1, i1), new Vector2Int(j2, i2)));
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
        return currentCoupleCache;
    }
    void RemoveFruitFromCurrentCoupleCanConnect(Vector2Int coordinate1, Vector2Int coordinate2)
    {
        currentCoupleCanConnect.Remove((coordinate1, coordinate2));
        currentCoupleCanConnect.Remove((coordinate2, coordinate1));
    }
    void UpdateCurrentActiveFruits (Fruit fr1, Fruit fr2)
    {
        currentActiveFruits.Remove(fr1.Coordinate);
        currentActiveFruits.Remove(fr2.Coordinate);
    }
    private Fruit fruit1Tmp, fruit2Tmp;
    private int id1, id2;
    void ShuffleBoardWithActiveFruits()
    {
        List<Fruit> fruitList = currentActiveFruits.Values.ToList();
        while (fruitList.Count > 0)
        {
            
            id1 =  Random.Range(0, fruitList.Count);
            fruit1Tmp = fruitList[id1];
            fruitList.RemoveAt(id1);
            
            id2 = Random.Range(0, fruitList.Count);
            fruit2Tmp = fruitList[id2];
            fruitList.RemoveAt(id2);
            
            //Debug.Log("Swap" + fruit1Tmp.NameType + " : " + fruit2Tmp.NameType);
            
            Utilities.SwapFruitData(fruit1Tmp, fruit2Tmp);
        }
    }
    private (Vector2Int, Vector2Int) GetSuggest()
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
        ShuffleBoardWithActiveFruits();
        currentCoupleCanConnect = GetTilesCanMatch();
    }

    private void ConnectFruits()
    {
        var res = BFS_Solver.BFS(currentFruitBoard, fruit1.Coordinate, fruit2.Coordinate,
            maxSegment);
        if (res.Item1)
        {
            fruit1.OnConnected();
            fruit2.OnConnected();
            RemoveFruitFromCurrentCoupleCanConnect(fruit1.Coordinate, fruit2.Coordinate);
            UpdateCurrentActiveFruits(fruit1, fruit2);
            currentCoupleCanConnect = GetTilesCanMatch();
            fruit1 = null;
            fruit2 = null;
            PathLineDrawer.Instance.DrawPath(GetPositionList(res.Item2));
            SoundManager.Instance.SetMusicVFX(SoundVFXType.SoundGetPoint);
            currentScore += Random.Range(connectScore, connectScore * 2);
            UpdateScore?.Invoke(currentScore);
                    
            if (currentActiveFruits.Count <= 0)
            {
                GameManager.Instance.EndLevel();
                GameManager.Instance.WinGame();
            }
            Debug.Log(currentActiveFruits.Count);
        }
    }
    public void Suggest()
    {
        (Vector2Int key1, Vector2Int key2) = GetSuggest();
        if (key1 == Vector2Int.zero)
        {
            Debug.Log("NO PAIR OF FRUITS TO CONNECT.");
            return;
        }
        fruit1 = currentActiveFruits[key1];
        fruit2 = currentActiveFruits[key2];
        if (fruit1 != null)
        {
            fruit1.OnSelected();
        }
        if (fruit2 != null)
        {
            fruit2.OnSelected();
        }
        ConnectFruits();
    }
    private void OnFruitSelected(Fruit fruit)
    {
        if (fruit1 != null) fruit1.OnDeselected();
        if (fruit2 != null) fruit2.OnDeselected(); 
        if (fruit != null)
        {
            if (fruit1 == null) fruit1 = fruit;
            else if (fruit2 == null) fruit2 = fruit;
            else
            {
                fruit1 = fruit2;
                fruit2 = fruit;
            }
            fruit.OnSelected();
        }
    }
    #endregion
}
