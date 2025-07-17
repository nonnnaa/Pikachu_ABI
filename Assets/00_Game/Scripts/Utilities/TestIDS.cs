using System.Collections.Generic;
using UnityEngine;

public class IDSPathFinder : MonoBehaviour
{
    public int limit = 20;
    public int target = 4;
    private void Start()
    {
        int[,] grid = {
            {0, 1, 1, 1, 1, 1, 1, 1, 1},
            {1, 1, 1, 1, 0, 1, 1, 1, 1},
            {1, 1, 1, 1, 1, 1, 1, 1, 1}
        };

        Utilities.PrintGrid(grid); // In ma trận ban đầu

        List<(int, int)> zeroPoints = new();
        for (int i = 0; i < grid.GetLength(0); i++)
            for (int j = 0; j < grid.GetLength(1); j++)
                if (grid[i, j] == 0)
                    zeroPoints.Add((i, j));

        if (zeroPoints.Count != 2)
        {
            Debug.Log("Grid must contain exactly two 0s.");
            return;
        }

        var start = zeroPoints[0];
        var goal = zeroPoints[1];

        bool found = IDS(grid, start, goal, limit, target);
        Debug.Log(found ? "Path found!" : "No path found.");
    }

    
    // Tinh theo toa do cua ma tran 2 chieu 
    static readonly int[,] directions = {
        {0, 1}, // right
        {1, 0}, // down
        {0, -1},// left
        {-1, 0} // up
    };
    
    // Duyet DFS theo limit va maxSegment
    public static bool IDS(int[,] grid, (int, int) start, (int, int) goal, int maxLimit, int targetSegments)
    {
        for (int depthLimit = 0; depthLimit <= maxLimit; depthLimit++)
        {
            Debug.Log($"🔍 Trying depth limit: {depthLimit}");
            var path = DLS(grid, start, goal, depthLimit, targetSegments);
            if (path != null)
            {
                Debug.Log($"✅ Found path at depth: {depthLimit}");
                PrintPath(path);
                Debug.Log($"📏 Number of straight segments: {CountStraightSegments(path)}");
                return true;
            }
        }
        return false;
    }
    
    
    // Chay DFS voi do sau cu the ( = limit)
    public static List<(int, int)> DLS(int[,] grid, (int, int) start, (int, int) goal, int limit, int targetSegments)
    {
        var stack = new Stack<(int, int)>();
        var depthMap = new Dictionary<(int, int), int>();
        var parentMap = new Dictionary<(int, int), (int, int)?>();
    
        stack.Push(start);
        depthMap[start] = 0;
        parentMap[start] = null;

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            int currentDepth = depthMap[current];

            if (current == goal)
            {
                var path = ReconstructPath(parentMap, goal);
                if (CountStraightSegments(path) == targetSegments)
                {
                    return path;
                }
                continue; // Không khớp số đoạn, thử nhánh khác
            }

            if (currentDepth >= limit)
                continue;

            for (int i = 0; i < 4; i++)
            {
                int newX = current.Item1 + directions[i, 0];
                int newY = current.Item2 + directions[i, 1];
                var next = (newX, newY);

                if (!IsInBounds(grid, newX, newY))
                    continue;

                int cellValue = grid[newX, newY];

                if (cellValue != 1 && next != goal)
                    continue;

                if (!depthMap.ContainsKey(next) || depthMap[next] > currentDepth + 1)
                {
                    depthMap[next] = currentDepth + 1;
                    parentMap[next] = current;
                    stack.Push(next);
                }
            }
        }

        return null;
    }

    // Get Path
    static List<(int, int)> ReconstructPath(Dictionary<(int, int), (int, int)?> parentMap, (int, int) goal)
    {
        var path = new List<(int, int)>();
        (int, int)? current = goal;

        while (current != null)
        {
            path.Add(current.Value);
            current = parentMap[current.Value];
        }

        path.Reverse();
        return path;
    }
    
    // Check xem toa do co nam trong ma tran khong
    static bool IsInBounds(int[,] grid, int x, int y)
    {
        return x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1);
    }

    
    // Show Path
    static void PrintPath(List<(int, int)> path)
    {
        Debug.Log("Path:");
        foreach (var p in path)
            Debug.Log($"({p.Item1},{p.Item2})");
    }
    
    
    // Dem so doan thang
    static int CountStraightSegments(List<(int, int)> path)
    {
        if (path == null || path.Count < 2) return 0;

        int segments = 1;
        var prevDir = (path[1].Item1 - path[0].Item1, path[1].Item2 - path[0].Item2);

        for (int i = 1; i < path.Count - 1; i++)
        {
            var dir = (path[i + 1].Item1 - path[i].Item1, path[i + 1].Item2 - path[i].Item2);
            if (dir != prevDir)
            {
                segments++;
                prevDir = dir;
            }
        }
        return segments;
    }
}

#region Old Version
// public static bool IDS_Solver(int[,] grid, (int, int) start, (int, int) goal, int maxLimit)
    // {
    //     for (int depthLimit = 0; depthLimit <= maxLimit; depthLimit++)
    //     {
    //         Debug.Log($"🔍 Trying depth limit: {depthLimit}");
    //         var path = DLS(grid, start, goal, depthLimit);
    //         if (path != null)
    //         {
    //             Debug.Log($"✅ Found path at depth: {depthLimit}");
    //             PrintPath(path);
    //             Debug.Log($"📏 Number of straight segments: {CountStraightSegments(path)}");
    //             return true;
    //         }
    //     }
    //     return false;
    // }

    // public static List<(int, int)> DLS(int[,] grid, (int, int) start, (int, int) goal, int limit)
    // {
    //     var stack = new Stack<(int, int)>();
    //     var depthMap = new Dictionary<(int, int), int>();
    //     var parentMap = new Dictionary<(int, int), (int, int)?>();
    //     
    //     stack.Push(start);
    //     depthMap[start] = 0;
    //     parentMap[start] = null;
    //
    //     while (stack.Count > 0)
    //     {
    //         var current = stack.Pop();
    //         int currentDepth = depthMap[current];
    //
    //         if (current == goal)
    //             return ReconstructPath(parentMap, goal);
    //
    //         if (currentDepth >= limit)
    //             continue;
    //
    //         for (int i = 0; i < 4; i++)
    //         {
    //             int newX = current.Item1 + directions[i, 0];
    //             int newY = current.Item2 + directions[i, 1];
    //             var next = (newX, newY);
    //
    //             if (!IsInBounds(grid, newX, newY))
    //                 continue;
    //
    //             int cellValue = grid[newX, newY];
    //
    //             // Chỉ đi qua ô 1 hoặc ô goal
    //             if (cellValue != 1 && next != goal)
    //                 continue;
    //
    //             // Nếu chưa duyệt hoặc có đường đi ngắn hơn
    //             if (!depthMap.ContainsKey(next) || depthMap[next] > currentDepth + 1)
    //             {
    //                 depthMap[next] = currentDepth + 1;
    //                 parentMap[next] = current;
    //                 stack.Push(next);
    //             }
    //         }
    //     }
    //
    //     return null;
    // }
#endregion

