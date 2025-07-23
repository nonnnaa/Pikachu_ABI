using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PathLineDrawer : SingletonMono<PathLineDrawer>
{
    public float lineWidth = 0.15f;
    [SerializeField] private Texture[] textures;
    [SerializeField] private float duration = .3f;
    [SerializeField] private float fps = 20f;
    private int animationStep;
    private float fpsCounnter;
    private LineRenderer lineRenderer;
    private bool isDrawing;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Start()
    {
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        GameManager.Instance.OnLevelEnd += ClearPath;
    }

    void Update()
    {
        if (!isDrawing || textures == null || textures.Length == 0)
            return;

        fpsCounnter += Time.deltaTime;
        if (fpsCounnter >= 1f / fps)
        {
            animationStep = (animationStep + 1) % textures.Length;
            lineRenderer.material.mainTexture = textures[animationStep];
            fpsCounnter = 0f;
        }
    }

    public void DrawPath(List<Vector3> worldPositions)
    {
        if (worldPositions == null || worldPositions.Count < 2)
        {
            lineRenderer.positionCount = 0;
            isDrawing = false;
            return;
        }

        lineRenderer.positionCount = worldPositions.Count;

        for (int i = 0; i < worldPositions.Count; i++)
        {
            lineRenderer.SetPosition(i, worldPositions[i]);
        }

        isDrawing = true;

        StopAllCoroutines();
        StartCoroutine(HideAfterSeconds(duration));
    }

    private IEnumerator HideAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        ClearPath();
    }

    public void ClearPath()
    {
        isDrawing = false;
        lineRenderer.positionCount = 0;
    }
}
