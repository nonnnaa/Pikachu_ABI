// using System.Collections.Generic;
// using UnityEngine;
//
// // Van de : Time limit, storage limit => not recommend
// // Y tuong tuong tu nhu BFS_Solve => dua vao maxSegment de limit duong di
//
//
//
//
//
//
// public class IDS_Solver : MonoBehaviour
// {
//     // Start call => IDS(grid, start, goal, limit, maxSegment);
//     
//     // Tinh theo toa do cua ma tran 2 chieu 
//     static readonly int[,] directions = {
//         {0, 1}, // up
//         {1, 0}, // right
//         {0, -1},// down
//         {-1, 0} // left
//     };
//     
//     // Duyet DFS theo limit va maxSegment
//     public static bool IDS(List<List<Fruit>> currentBoard, Vector2Int start, Vector2Int goal, int maxLimit, int targetSegments)
//     {
//         for (int depthLimit = 1; depthLimit <= maxLimit; depthLimit++)
//         {
//             var path = DLS(currentBoard, start, goal, depthLimit, targetSegments);
//             Debug.Log(depthLimit);
//             if (path != null)
//             {
//                 Debug.Log($"Number of straight segments: {CountStraightSegments(path)}");
//                 return true;
//             }
//         }
//         return false;
//     }
//
//     #region Test
//
//     
//
//     
//     // Chay DFS voi do sau cu the ( = limit)
//     public static List<Vector2Int> DLS(List<List<Fruit>> currentBoard, Vector2Int start, Vector2Int goal, int limit, int targetSegments)
//     {
//         var stack = new Stack<Vector2Int>();
//         var depthMap = new Dictionary<Vector2Int, int>();
//         var parentMap = new Dictionary<Vector2Int, Vector2Int?>();
//     
//         int gridY = currentBoard.Count;
//         if (gridY <= 0)
//         {
//             Debug.LogError($"Grid height is invalid: {gridY}");
//             return null;
//         }
//     
//         int gridX = currentBoard[0].Count;
//     
//         stack.Push(start);
//         depthMap[start] = 0;
//         parentMap[start] = null;
//     
//         int cnt = 0;
//         
//         while (stack.Count > 0 && cnt ++ < 900)
//         {
//             Vector2Int current = stack.Pop();
//             int currentDepth = depthMap[current];
//     
//             if (current == goal)
//             {
//                 var path = ReconstructPath(parentMap, goal);
//                 if (CountStraightSegments(path) <= targetSegments)
//                 {
//                     //PrintPath(path);
//                     return path;
//                 }
//                 continue; // Không thỏa số đoạn, tiếp tục nhánh khác
//             }
//     
//             if (currentDepth >= limit)
//                 continue;
//     
//             for (int i = 0; i < 4; i++)
//             {
//                 int newX = current.x + directions[i, 0];
//                 int newY = current.y + directions[i, 1];
//     
//                 
//                 if (!IsInBounds(gridX, gridY, newX, newY))
//                     continue;
//                 
//                 Vector2Int next = new Vector2Int(newX, newY);
//     
//                 Fruit cell = currentBoard[newY][newX];
//     
//                 if (cell == null)
//                 {
//                     Debug.LogWarning($"Cell at ({newX}, {newY}) is null.");
//                     continue;
//                 }
//     
//                 Debug.Log($"Checking ({newX},{newY}) | active: {cell.gameObject.activeSelf}, type: {cell.nameType}");
//     
//                 bool isGoal = next == goal;
//                 bool isInactive = !cell.gameObject.activeSelf;
//                 bool isEmpty = cell.nameType == TileNameType.TileEmpty;
//     
//                 //Debug.Log($"{next} => isGoal : {isGoal}, isInactive : {isInactive}, isEmpty : {isEmpty}");
//                 
//                 
//                 
//                 
//                 if (isGoal || isInactive || isEmpty)
//                 {
//                     if (!depthMap.ContainsKey(next) || depthMap[next] >= currentDepth + 1)
//                     {
//                         depthMap[next] = currentDepth + 1;
//                         parentMap[next] = current;
//                         stack.Push(next);
//                     }
//                 }
//             }
//         }
//     
//         return null;
//     }
//     #endregion
//
//
//
//
//     
//     // Get Path
//     private static List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int?> parentMap, Vector2Int goal)
//     {
//         List<Vector2Int> path = new List<Vector2Int>();
//         Vector2Int? current = goal;
//         while (current.HasValue)
//         {
//             path.Add(current.Value);
//             current = parentMap[current.Value];
//         }
//         path.Reverse();
//         return path;
//     }
//
//     // Check xem toa do co nam trong ma tran khong
//     static bool IsInBounds(int gridX, int gridY, int newX, int newY)
//     {
//         return newX >= 0 && newX < gridX && newY >= 0 && newY < gridY;
//     }
//     // Show Path
//     static void PrintPath(List<Vector2Int> path)
//     {
//         string res = "Path : ";
//         foreach (var p in path)
//             res += ($"({p.x},{p.y}) -> ");
//         Debug.Log(res);
//     }
//     
//     
//     // Dem so doan thang
//     static int CountStraightSegments(List<Vector2Int> path)
//     {
//         if (path == null || path.Count < 2) return 0;
//
//         int segments = 1;
//         int dxPrev = path[1].x - path[0].x;
//         int dyPrev = path[1].y - path[0].y;
//
//         for (int i = 1; i < path.Count - 1; i++)
//         {
//             int dx = path[i + 1].x - path[i].x;
//             int dy = path[i + 1].y - path[i].y;
//             if (dx != dxPrev || dy != dyPrev)
//             {
//                 segments++;
//                 dxPrev = dx;
//                 dyPrev = dy;
//             }
//         }
//
//         return segments;
//     }
//
// }
