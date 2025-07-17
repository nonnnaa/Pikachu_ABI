using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PathLineDrawer : SingletonMono<PathLineDrawer>
{
    public float lineWidth = 0.15f;
    public Color lineColor = Color.yellow;

    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
    }

    /// <summary>
    /// Vẽ đường nối dựa trên danh sách tọa độ Vector2Int (path từ thuật toán tìm đường)
    /// </summary>
    /// <param name="path">Danh sách điểm (grid)</param>
    /// <param name="cellSize">Kích thước mỗi ô trong bảng</param>
    /// <param name="boardOrigin">Góc trên bên trái của bảng (tọa độ thế giới)</param>
    /// <summary>
    /// Vẽ đường đi dựa trên danh sách tọa độ thế giới. Tự động ẩn sau 1 giây.
    /// </summary>
    public void DrawPath(List<Vector3> worldPositions, float duration = 1f)
    {
        if (worldPositions == null || worldPositions.Count < 2)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        lineRenderer.positionCount = worldPositions.Count;

        for (int i = 0; i < worldPositions.Count; i++)
        {
            lineRenderer.SetPosition(i, worldPositions[i]);
        }

        StopAllCoroutines();
        StartCoroutine(HideAfterSeconds(duration));
    }

    private IEnumerator HideAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        ClearPath();
    }

    
    public void DrawLine(Vector3 from, Vector3 to)
    {
        if (Instance == null) return;

        Instance.lineRenderer.positionCount = 2;
        Instance.lineRenderer.SetPosition(0, from);
        Instance.lineRenderer.SetPosition(1, to);
    }
    /// <summary>
    /// Xóa đường nối hiện tại
    /// </summary>
    public void ClearPath()
    {
        lineRenderer.positionCount = 0;
    }
}