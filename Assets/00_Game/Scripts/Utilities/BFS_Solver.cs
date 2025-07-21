using System.Collections.Generic;
using UnityEngine;

public class BFS_Solver : MonoBehaviour
{
    // Dinh nghia 4 huong co the di chuyen trong ma tran
    private static readonly Vector2Int[] directions = {
        new Vector2Int(0, 1),  // up
        new Vector2Int(1, 0),  // right
        new Vector2Int(0, -1), // down
        new Vector2Int(-1, 0)  // left
    };
    
    // Goi BFS
    public static (bool, List<Vector2Int>) BFS(List<List<Fruit>> currentBoard, Vector2Int start, Vector2Int goal, int maxSegments)
    {
        var path = FindPath(currentBoard, start, goal, maxSegments);
        if (path != null)
        {
            //Debug.Log($"Found path with {CountStraightSegments(path)} segments.");
            PrintPath(path);
            return (true, path);
        }
        //Debug.Log("No valid path found.");
        return (false, null);
    }
    
    // Duyet BFS
    private static List<Vector2Int> FindPath(List<List<Fruit>> board, Vector2Int start, Vector2Int goal, int maxSegments)
    {
        int height = board.Count;
        if (height <= 0) return null;
        int width = board[0].Count;

        var visited = new Dictionary<(Vector2Int pos, int dir), int>();
        var parentMap = new Dictionary<(Vector2Int pos, int dir), (Vector2Int parentPos, int parentDir)>();

        var queue = new Queue<(Vector2Int pos, int dir, int turns)>();

        for (int i = 0; i < 4; i++)
        {
            Vector2Int next = start + directions[i];
            if (!IsInBounds(width, height, next.x, next.y)) continue;

            var cell = board[next.y][next.x];
            if (IsCanMove(cell, next, goal))
            {
                queue.Enqueue((next, i, 1));
                visited[(next, i)] = 1;
                parentMap[(next, i)] = (start, i);
            }
        }

        while (queue.Count > 0)
        {
            var (current, dir, turns) = queue.Dequeue();

            if (current == goal && turns <= maxSegments)
            {
                return ReconstructPath(parentMap, (current, dir), start);
            }

            for (int i = 0; i < 4; i++)
            {
                Vector2Int next = current + directions[i];
                int newTurns = (i == dir) ? turns : turns + 1;

                if (newTurns > maxSegments) continue;
                if (!IsInBounds(width, height, next.x, next.y)) continue;

                var cell = board[next.y][next.x];
                if (!IsCanMove(cell, next, goal)) continue;

                var key = (next, i);
                if (visited.TryGetValue(key, out int prevTurns) && prevTurns <= newTurns)
                    continue;

                visited[key] = newTurns;
                parentMap[key] = (current, dir);
                queue.Enqueue((next, i, newTurns));
            }
        }
        return null;
    }
    
    // Check Dieu kien thoa man de di chuyen
    private static bool IsCanMove(Fruit cell, Vector2Int pos, Vector2Int goal)
    {
        if (pos == goal) return true;
        if (cell == null) return false;
        return !cell.gameObject.activeSelf || cell.nameType == TileNameType.TileEmpty;
    }

    // Check dieu kien co nam trong board hay khong
    private static bool IsInBounds(int width, int height, int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }
    
    // Tim duong start -> end bang parent map
    private static List<Vector2Int> ReconstructPath(Dictionary<(Vector2Int pos, int dir), (Vector2Int parentPos, int parentDir)> parentMap, (Vector2Int pos, int dir) endKey, Vector2Int start)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        HashSet<(Vector2Int, int)> visitedSet = new HashSet<(Vector2Int, int)>();

        var current = endKey;

        while (true)
        {
            if (!visitedSet.Add(current))
            {
                //Debug.LogError($"Infinite loop detected while reconstructing path at {current.pos}");
                return null;
            }

            path.Add(current.pos);

            if (current.pos == start) break;

            if (!parentMap.TryGetValue(current, out var parent))
            {
                //Debug.LogWarning($"Parent not found for {current.pos} with dir {current.dir}");
                return null;
            }

            current = (parent.parentPos, parent.parentDir);
        }

        path.Reverse();
        return path;
    }
    
    // Dem doan thang tren path
    private static int CountStraightSegments(List<Vector2Int> path)
    {
        if (path == null || path.Count < 2) return 0;

        int segments = 1;
        Vector2Int prevDir = path[1] - path[0];

        for (int i = 1; i < path.Count - 1; i++)
        {
            Vector2Int dir = path[i + 1] - path[i];
            if (dir != prevDir)
            {
                segments++;
                prevDir = dir;
            }
        }
        return segments;
    }

    // In path
    private static void PrintPath(List<Vector2Int> path)
    {
        if (path == null || path.Count == 0)
        {
            //Debug.Log("Path is empty or null.");
            return;
        }

        string output = "Path: ";
        for (int i = 0; i < path.Count; i++)
        {
            output += path[i].ToString();
            if (i < path.Count - 1) output += " -> ";
        }
        //Debug.Log(output);
    }
}
