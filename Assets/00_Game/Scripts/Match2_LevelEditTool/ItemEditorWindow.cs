using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
public class LevelDataEditorWindow : EditorWindow
{
    #region Temp variables
    private LevelData currentData;
    private int row, column, level;
    private List<TileRow> boardData;
    private bool isShowDebugBoard;
    #endregion
    [MenuItem("Tools/Level Data Editor")]
    public static void ShowWindow()
    {
        GetWindow<LevelDataEditorWindow>("Level Data Editor");
    }
    private void OnGUI()
    {
        
        // Draw label
        GUILayout.Label("Level Data Editor", EditorStyles.boldLabel);
        
        
        // get new Data from UI (If changed and set to Temp variables)
        var newData = (LevelData)EditorGUILayout.ObjectField("Level Asset", currentData, typeof(LevelData), false);
        if (newData != currentData && newData != null)
        {
            currentData = newData;
            // Sync data
            level = newData.levelId;
            row = newData.row;
            column = newData.column;
            boardData  = newData.boardData;
        }

        EditorGUILayout.Space();

        
        // set data
        level = EditorGUILayout.IntField("Level ID", level);
        column = EditorGUILayout.IntField("Column", column);
        row = EditorGUILayout.IntField("Row", row);
        
        isShowDebugBoard = EditorGUILayout.Toggle("ShowDebugBoard", isShowDebugBoard);
        
        
        // handle button Gen board onclick event
        if (GUILayout.Button("Generate Board"))
        {
            boardData = GenBoard(row, column, currentData.boardData);
            if(isShowDebugBoard) ShowDebugBoard(boardData);
        }
        
        // Draw board
        DrawBoard(boardData);
        
        // Savw button handle
        if (GUILayout.Button("Save"))
        {
            currentData.levelId = level;
            currentData.row = row;
            currentData.column = column;
            currentData.boardData = GenBoard(row, column, currentData.boardData);
            if(isShowDebugBoard) ShowDebugBoard(boardData);
            EditorUtility.SetDirty(currentData);
            AssetDatabase.SaveAssets();
            Debug.Log("Level saved!");
        }
    }
    private List<TileRow> GenBoard(int newRow, int newColumn, List<TileRow> oldBoard)
    {
        if (oldBoard == null)
        {
            oldBoard = new List<TileRow>();
        }
        
        List<TileRow> newBoard = new List<TileRow>();
        int oldRow = oldBoard.Count;
        
        for (int i = 0; i < newRow; i++)
        {
            var rowTmp = new TileRow();
            rowTmp.column = new List<TileJson>();
            if (i < oldRow && oldBoard[i] != null)
            {
                int sizeColumn = oldBoard[i].column != null ? oldBoard[i].column.Count : 0;
                for (int j = 0; j < newColumn; j++)
                {
                    // toan tu 3 ngoi thay the if else
                    TileJson tile = (j < sizeColumn) ? oldBoard[i].column[j] : new TileJson(i, j, 0);
                    rowTmp.column.Add(tile);
                }
            }
            else
            {
                for(int j = 0; j < newColumn; j++)
                {
                    rowTmp.column.Add(new TileJson(i, j, 0));
                }
            }
            newBoard.Add(rowTmp);
        }
        Debug.Log("Board generated.");
        return newBoard;
    }
    void ShowDebugBoard(List<TileRow> board)
    {
        string result = "Board:\n";

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
}
