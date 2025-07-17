using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Level Data", menuName = "Scriptable Object Data/Level Data")]
public class LevelData : ScriptableObject
{
   public int levelId;
   public int row, column;
   public List<TileRow> boardData;
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

   public override string ToString()
   {
      if (column == null || column.Count == 0)
         return "[]";

      return "[" + string.Join(", ", column) + "]";
   }
}
