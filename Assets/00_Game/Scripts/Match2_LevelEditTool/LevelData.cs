using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjectData/LevelData")]
public class LevelData : ScriptableObject
{
   public int levelId;
   public int row, column;
   public float time;
   public int targetScore;
   public List<int> starTimeGoals; // time nay la de tru di khong phai cong sao => rule game max la 3 sao cu qua time[i] thi bi tru di 1 sao
   public List<FruitCount> randomFruitCounts;
   public List<TileRow> boardData;
   public List<List<int>> GetBoardValue()
   {
      List<List<int>> boardValue = new List<List<int>>();
      if (boardData != null)
      {
         int row = boardData.Count;
         int column = boardData[0].column.Count;
         for (int i = 0; i < row; i++)
         {
            List<int> rowTmp = new List<int>();
            for (int j = 0; j < column; j++)
            {
               rowTmp.Add(boardData[i].column[j].value);
            }
            boardValue.Add(rowTmp);
         }
      }
      return boardValue;
   }
}


// Tile Infor in Editor
[Serializable]
public class TileJson
{
   public int x, y, value;
   
   public TileJson(int x, int y, int value)
   {
      this.x = x;
      this.y = y;
      this.value = value;
   }
   public override string ToString()
   {
      return $"({x},{y}:{value})";
   }
}

// Row 
[Serializable]
public class TileRow
{
   public List<TileJson> column;

   public TileRow()
   {
      column = new List<TileJson>();
   }
   public override string ToString()
   {
      if (column == null || column.Count == 0)
         return "[]";

      return "[" + string.Join(", ", column) + "]";
   }
}

[Serializable]
public struct FruitCount
{
   public TileNameType fruitType;
   public int count;

   public FruitCount(TileNameType fruitType, int count)
   {
      this.fruitType = fruitType;
      this.count = count;
   }
}
