using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : SingletonMono<BoardManager>
{
    private int maxSegment = 3; // Rule Game
    
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
    // Score
    private int currentConnected;
    private int maxConnect;
    private int maxScore;
    private int connectScore;
    private int currentScore;
    #endregion
    
    public int GetCurrentScore() => currentScore;
    
    public event Action<int> UpdateScore;
    
    #region LifeCycle Messages
    private void Start()
    {
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
                    currentScore += Random.Range(connectScore, connectScore * 2);
                    UpdateScore?.Invoke(currentScore);
                    currentConnected++;
                    if (currentConnected >= maxConnect)
                    {
                        GameManager.Instance.EndLevel();
                        GameManager.Instance.WinGame();
                    }
                }
            }
            else
            {
                //Debug.Log("Fruits IS InValid");
            }
        }
    }
    #endregion
    
    #region Functions
    
    // (xPos, yPos, x, y) -> tuong ung (vi tri x, vi tri y, toa do x, toa do y) , vi tri : vi tri tren scene, toa do : toa so tren mang 2 chieu (so nguyen >= 0)
    List<(float, float, int , int)> RandomFruitsPosition(List<List<int>> boardValue)
    {
        List<(float, float, int, int)> listPos = new List<(float, float, int, int)>();
        int rowTmp = boardValue.Count;
        int columnTmp = boardValue[0].Count;
        
        // Debug.Log(rowTmp + " " +  columnTmp);
        for (int y = 0; y < rowTmp; y++)
        {
            for (int x = 0; x < columnTmp; x++)
            {
                //Debug.Log(IsValidCoordinate(x, y, columnTmp, rowTmp) + " " +  IsNormalFruit((TileNameType)boardValue[y][x]));
                if (boardValue[y][x] == -1)
                {
                    listPos.Add((x + _offsetX, -(y + _offsetY), x, y));
                } 
            }
        }
        return listPos;
    }
    
    // (xPos, yPos, x, y) -> tuong ung (vi tri x, vi tri y, toa do x, toa do y) , vi tri : vi tri tren scene, toa do : toa so tren mang 2 chieu (so nguyen >= 0)
    List<(float, float, int , int)> RandomSpecialFruitsPosition(List<List<int>> boardValue)
    {
        List<(float, float, int, int)> listPos = new List<(float, float, int, int)>();
        int rowTmp = boardValue.Count;
        int columnTmp = boardValue[0].Count;
        
        // Debug.Log(rowTmp + " " +  columnTmp);
        for (int y = 0; y < rowTmp; y++)
        {
            for (int x = 0; x < columnTmp; x++)
            {
                //Debug.Log(IsValidCoordinate(x, y, columnTmp, rowTmp) + " " +  IsNormalFruit((TileNameType)boardValue[y][x]));
                if (boardValue[y][x] != -1)
                {
                    listPos.Add((x + _offsetX, -(y + _offsetY), x, y));
                } 
            }
        }
        return listPos;
    }
    
    void OnInit()
    {
        currentConnected = 0;
        currentIdFruitChosen = 0;
        currentScore = 0;
        connectScore = Random.Range(10, 15);
        maxScore = LevelManager.Instance.GetCurrentLevelData().targetScore;
        canInteractive = true;
        fruitDictionary = new Dictionary<Collider2D, Fruit>(); 
        currentFruitBoard = new List<List<Fruit>>();
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
        
        List<(float, float, int, int)> coordinatesRandom = RandomFruitsPosition(fruitBoardValue);
        for (int i = 0; i < row; i++)
        {
            List<Fruit> fruits = new List<Fruit>();
            for (int j = 0; j < column; j++)
            {
                fruits.Add(null);
            }
            currentFruitBoard.Add(fruits);
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
                //Debug.Log(id1);
                (float xPos1, float yPos1, int x1, int y1) = coordinatesRandom[id1];
                var fruit1 = Instantiate(GetFruit(type), new Vector3(xPos1, yPos1, 0), Quaternion.identity);
                fruit1.transform.SetParent(board);
                fruit1.coordinate =  new Vector2Int(x1, y1);
                currentFruitBoard[y1][x1] =  fruit1;
                coordinatesRandom.RemoveAt(id1);
                fruitCount--;
                coordinatesSize--;
                
                // Spawn fruit2
                int id2 = Random.Range(0, coordinatesSize);
                //Debug.Log(id2);
                (float xPos2, float yPos2, int x2, int y2) = coordinatesRandom[id2];
                var fruit2 = Instantiate(GetFruit(type), new Vector3(xPos2, yPos2, 0), Quaternion.identity);
                fruit2.transform.SetParent(board);
                fruit2.coordinate =  new Vector2Int(x2, y2);
                currentFruitBoard[y2][x2] =  fruit2;
                coordinatesRandom.RemoveAt(id2);
                fruitCount--;
                coordinatesSize--;
            }
        }
        // Spawn Other Fruit Not from Random
        List<(float, float, int, int)> coordinatesSpecial = RandomSpecialFruitsPosition(fruitBoardValue);
        foreach (var item in coordinatesSpecial)
        {
            (float xPos, float yPos, int x, int y) = item;
            var fruit = Instantiate(GetFruit((TileNameType)fruitBoardValue[y][x]), new Vector3(xPos, yPos, 0), Quaternion.identity);
            fruit.transform.SetParent(board);
            fruit.coordinate =  new Vector2Int(x, y);
            currentFruitBoard[y][x] =  fruit;
        }

        int cnt = 0;
        for (int i = 0; i < fruitBoardValue.Count; i++)
        {
            for (int j = 0; j < fruitBoardValue[i].Count; j++)
            {
                if (IsNormalFruit((TileNameType)fruitBoardValue[i][j]))
                {
                    cnt++;
                }
            }
        }

        maxConnect = cnt / 2;
        Debug.Log("MAX_CONNECT : " + maxConnect);
    }
    
    // Get Fruit From Value Of Data
    Fruit GetFruit(TileNameType type)
    {
        return fruitPrefabDictionary[type];
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
    
    // Tao delay time de fruit vs line bien mat cung thoi gian (cung 0.3s)
    IEnumerator DelayDeactiveFruit(Fruit fruit)
    {
        fruit.SetInteractive(false);
        yield return new  WaitForSeconds(0.3f);
        if (fruit == null || fruit.gameObject == null)
            yield break;
        fruit.gameObject.SetActive(false);
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
               && IsNormalFruit(fruit1.nameType)
               && IsNormalFruit(fruit2.nameType)
               && fruitsChosen[0].nameType == fruitsChosen[1].nameType;
    }
    
    // Xoa board
    private void Despawn()
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
}
