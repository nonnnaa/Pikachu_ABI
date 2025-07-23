using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


public class LevelDataEditorWindow : EditorWindow
{
    #region Temp variables
    private int row, column, levelId;
    private int targetScore;
    private int star1Time, star2Time, star3Time;
    private int totalFruitCanRandom;
    private int currentFruitCanRandom;
    private float totalTime;
    private bool isShowDebugBoard;
    private LevelData currentData;
    private Vector2 scrollPos;    
    private List<TileRow> boardData;
    private List<FruitCount> randomFruitsCount;
    #endregion

    [MenuItem("Tools/Level Data Editor")]
    public static void ShowWindow()
    {
        GetWindow<LevelDataEditorWindow>("Level Data Editor");
    }
    // Draw Editor
    private void OnGUI()
    {
        // Bắt đầu scroll view
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        
        // Draw label
        GUILayout.Label("Level Data Editor", EditorStyles.boldLabel);
        
        
        // get new Data from UI (If changed and set to Temp variables)
        LevelData newData = (LevelData)EditorGUILayout.ObjectField("Level Asset", currentData, typeof(LevelData), false);
        
        if (newData != currentData && newData != null)
        {
            UpdateDataDefault(newData);
            currentData = newData;
            // Sync data
            levelId = newData.levelId;
            row = newData.row; 
            column = newData.column;
            totalTime = newData.time;
            targetScore = newData.targetScore;
            star1Time = newData.starTimeGoals[0];
            star2Time = newData.starTimeGoals[1];
            star3Time = newData.starTimeGoals[2]; 
            
            // Khong gan truc tiep duoc vi khi co thay doi se thay doi luon data duoc tao ma chua can an luu (not save but saved)
            randomFruitsCount = new List<FruitCount>();
            foreach (var item in newData.randomFruitCounts)
            {
                randomFruitsCount.Add(new FruitCount
                {
                    fruitType = item.fruitType,
                    count = item.count
                });
            }
            //boardData  = newData.boardData;
            boardData = new List<TileRow>();
            foreach (var item in newData.boardData)
            {
                TileRow rowTmp = new TileRow();
                foreach (var tile in item.column)
                {
                    rowTmp.column.Add(new TileJson(tile.x, tile.y,tile.value));
                }
                boardData.Add(rowTmp);
            }
        }

        EditorGUILayout.Space();

        
        // set data
        levelId = EditorGUILayout.IntField("Level ID", levelId);
        column = EditorGUILayout.IntField("Column", column);
        row = EditorGUILayout.IntField("Row", row);
        totalTime = EditorGUILayout.FloatField("Time", totalTime);
        targetScore =  EditorGUILayout.IntField("Target Score", targetScore);
        star1Time =  EditorGUILayout.IntField("Star 1 Time", star1Time);
        star2Time =  EditorGUILayout.IntField("Star 2 Time", star2Time);
        star3Time =  EditorGUILayout.IntField("Star 3 Time", star3Time);
        isShowDebugBoard = EditorGUILayout.Toggle("ShowDebugBoard", isShowDebugBoard);
        
        // handle button Gen board onclick event
        if (GUILayout.Button("Generate BoardManager"))
        {
            if(row < 2 && column < 2)
            {
                Debug.LogError("Please check row and column");
            }
            else
            {
                boardData = GenBoard(row, column);
                if(isShowDebugBoard) ShowDebugBoard(boardData);
            }
        }


        // Gen Fruit Random Count
        currentFruitCanRandom = GetCurrentFruitCanRandom(randomFruitsCount);
        totalFruitCanRandom = GetTotalFruitCanRandom(boardData);
        DrawRandomFruitCounts(randomFruitsCount);
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical();
        GUILayout.Label("Total Fruit Slots: " + totalFruitCanRandom);
        GUILayout.Label("Current Assigned Fruits: " + currentFruitCanRandom);
        EditorGUILayout.EndVertical();
    
        
        // Draw board
        DrawBoard(boardData);
        
        // Savw button handle
        if (GUILayout.Button("Save") && IsValidBoardInfor())
        {
            currentData.levelId = levelId;
            currentData.row = row;
            currentData.column = column;
            currentData.time = totalTime;
            currentData.targetScore  =  targetScore;
            currentData.starTimeGoals[0] =  star1Time;
            currentData.starTimeGoals[1] =  star2Time;
            currentData.starTimeGoals[2] =  star3Time;
            currentData.randomFruitCounts =  randomFruitsCount;
            currentData.boardData = boardData;
            if(isShowDebugBoard) ShowDebugBoard(boardData);
            EditorUtility.SetDirty(currentData);
            AssetDatabase.SaveAssets();
            Debug.Log("Level saved!");
        }
        
        // Kết thúc scroll view
        EditorGUILayout.EndScrollView();
    }

    // Return Number Fruit Can Random From FruitCount
    int GetTotalFruitCanRandom(List<TileRow> board)
    {
        if (board == null || board.Count == 0)
        {
            return 0;
        }
        int res = 0;
        for (int x = 0; x < row; x++)
        {
            for (int y = 0; y < column; y++)
            {
                int value = boardData[x].column[y].value;
                if (value == -1)
                {
                    res++;
                }
            }
        }
        return res;
    }
    
    // Return Number of current Random Fruit 
    int GetCurrentFruitCanRandom(List<FruitCount> fruitCounts)
    {
        if (fruitCounts == null || fruitCounts.Count == 0)
        {
            return 0;
        }
        int res = 0;
        foreach (var fruitCount in fruitCounts)
        {
            res += fruitCount.count;
        }
        return res;
    }
    
    // Assign Board Value
    private List<TileRow> GenBoard(int newRow, int newColumn)
    {
        if (newColumn < 2 || newRow < 2)
        {
            Debug.LogError("Please check newColumn and newRow");
            return null;
        }
        List<TileRow> newBoard = new List<TileRow>();
;
        for (int i = 0; i < newRow; i++)
        {
            TileRow rowTmp = new TileRow();
            rowTmp.column = new List<TileJson>();
            for (int j = 0; j < newColumn; j++)
            {
                TileJson tile;
                if (i == 0 || j == 0 || i == newRow - 1 || j == newColumn - 1)
                {
                    tile = new TileJson(i, j, 0);
                }
                else
                {
                    tile = new TileJson(i, j, -1);
                }
                rowTmp.column.Add(tile);
            }
            newBoard.Add(rowTmp);
        }
        //Debug.Log("BoardManager generated.");
        return newBoard;
    }
    
    // Log Information Board
    void ShowDebugBoard(List<TileRow> board)
    {
        string result = "BoardManager:\n";

        for (int i = 0; i < board.Count; i++)
        {
            for (int j = 0; j < board[i].column.Count; j++)
            {
                result += board[i].column[j].value.ToString().PadLeft(3); 
            }
            result += "\n";
        }
        Debug.Log(result);
    }
    
    // Ve bang
    void DrawBoard(List<TileRow> currentBoard)
    {
        if (currentBoard != null)
        {
            int currentRow = currentBoard.Count;
            int currentColumn  = currentBoard[0].column.Count;
            EditorGUILayout.Space();
        
            int width = 50;

            // Header Row
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("X" + "\\" + "Y", GUILayout.Width(width));
    
            for (int y = 0; y < currentColumn; y++)
                GUILayout.Label($"[{y}]", EditorStyles.boldLabel, GUILayout.Width(width));
            EditorGUILayout.EndHorizontal();
    
            for (int x = 0; x < currentRow; x++)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label($"[{x}]", EditorStyles.boldLabel, GUILayout.Width(width));
                
               
                for (int y = 0; y < currentColumn; y++)
                {
                    TileJson cell = currentBoard[x].column[y];
                    cell.value = EditorGUILayout.IntField(cell.value, GUILayout.Width(width));
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        //Debug.Log("board null");
    }
    
    // Show Random Fruit Count & TileNameType
    private void DrawRandomFruitCounts(List<FruitCount> randomFruitCounts)
    {
        if (randomFruitCounts == null)
            randomFruitCounts = new List<FruitCount>();
        
        EditorGUILayout.LabelField("Random Fruit Counts", EditorStyles.boldLabel);
        
        for (int i = 0; i < randomFruitCounts.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
        
            var item = randomFruitCounts[i];
            var fruitType = (TileNameType)EditorGUILayout.EnumPopup(item.fruitType);
            var count = EditorGUILayout.IntField(item.count);
        
            randomFruitCounts[i] = new FruitCount(fruitType, count);
        
            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                randomFruitCounts.RemoveAt(i);
                EditorGUILayout.EndHorizontal();
                break;
            }
        
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Add Fruit Count"))
        {
            randomFruitCounts.Add(new FruitCount(TileNameType.Apple, 1));
        }
    }
    
    // Check Board Information to Save
    bool IsValidBoardInfor()
    {
        if (currentData == null)
        {
            Debug.LogError("BoardData is null");
            return false;
        }

        if (levelId < 0)
        {
            Debug.LogError("LevelId is negative");
        }

        if ((row % 2 == 1 && column % 2 == 1)
            || row < 0 || column < 0)
        {
            Debug.LogError("Row and Column are InValid(Must % 2 = 0 & > 0)");
            return false;
        }

        if (totalTime <= 0)
        {
            if (star1Time > totalTime)
            {
                Debug.LogError("Invalid : star1Time <= totalTime");
                return false;
            }

            if (star1Time >= star2Time || star2Time >= star3Time)
            {
                Debug.LogError("Invalid : star1Time > star2Time > star3Time");
                return false;
            }

            Debug.LogError("Invalid Time : totalTime > 0");
            return false;
        }

        if (targetScore <= 0)
        {
            Debug.LogError("TargetScore must be greater than 0");
            return false;
        }

        int totalRandomFruit = 0;
        foreach (var fruitCount in randomFruitsCount)
        {
            if (fruitCount.count <= 0 || fruitCount.count % 2 == 1)
            {
                Debug.LogError($"Invalid Fruit Count (FruitCount > 0 & {fruitCount.fruitType.ToString()} % 2 = 0): " + fruitCount.count);
                return false;
            }
            totalRandomFruit += fruitCount.count;
        }
        
        int realCountFruitCanRandom = GetTotalFruitCanRandom(boardData);

        if (totalRandomFruit != realCountFruitCanRandom)
        {
            Debug.LogError("Invalid : totalRandomFruit != realCountFruitCanRandom" + ": " + totalRandomFruit + " != " +  realCountFruitCanRandom);
            return false;
        }
        return true;
    }
    
    // Update newData to Default value if newData Information InValid
    void UpdateDataDefault(LevelData data)
    {
        if (data == null)
        {
            Debug.LogError("Data is null");
            return;
        }
        if (data.time <= 3)
        {
            data.time = 3;
        }

        if (data.column == 0)
        {
            data.column = 2;
        }

        if (data.row == 0)
        {
            data.row = 2;
        }

        if (data.targetScore == 0)
        {
            data.targetScore = 100;
        }

        if (data.boardData == null || data.boardData.Count == 0)
        {
            data.boardData = new List<TileRow>();
            for (int i = 0; i < data.column; i++)
            {
                TileRow tileRow = new TileRow();
                for (int j = 0; j < data.row; j++)
                {
                    TileJson tile;
                    if (i == 0 || j == 0 || i == data.row - 1 || j == data.column - 1)
                    {
                        tile = new TileJson(i, j, 0);
                    }
                    else
                    {
                        tile = new TileJson(i, j, -1);
                    }
                    tileRow.column.Add(tile);
                }
                data.boardData.Add(tileRow);
            }
        }
        
        if (data.randomFruitCounts == null)
        {
            data.randomFruitCounts = new List<FruitCount>();
        }

        if (data.starTimeGoals == null || data.starTimeGoals.Count == 0)
        {
            data.starTimeGoals = new List<int>
            {
                3, 2, 1
            };
        }
    }
}
