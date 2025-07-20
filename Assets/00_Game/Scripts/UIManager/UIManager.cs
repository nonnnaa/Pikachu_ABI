using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    Dictionary<System.Type, UICanvas> canvasActives;
    Dictionary<System.Type, UICanvas> canvasPrefabs;
    [SerializeField] private Transform parent;
    private void Awake()
    {
        canvasActives = new Dictionary<System.Type, UICanvas>();
        canvasPrefabs = new Dictionary<System.Type, UICanvas>();
        // load UI Prefab tu Resources
        UICanvas[] prefabs = Resources.LoadAll<UICanvas>("UI");
        for (int i = 0; i < prefabs.Length; i++)
        {
            canvasPrefabs.Add(prefabs[i].GetType(), prefabs[i]);
        }
    }

    private void Start()
    {
        GameManager.Instance.OnStartGame += () =>
        {
            CloseAll();
            OpenUI<CanvasStartGame>();
        };
        GameManager.Instance.OnLevelStart += () =>
        {
            CloseAll();
            OpenUI<CanvasGamePlay>();
        };
        GameManager.Instance.OnMainMenu += () =>
        {
            CloseAll();
            OpenUI<CanvasMainMenu>();
        };
        GameManager.Instance.OnGamePause += () =>
        {
            OpenUI<CanvasPauseGame>();
        };
        GameManager.Instance.OnGameLose += () =>
        {
            OpenUI<CanvasLoseGame>();
        };
        
    }
    
    // mo canvas
    public T OpenUI<T>() where T : UICanvas
    {
        T canvas =  GetUI<T>();
        //Debug.Log(canvas +" : " + canvas.gameObject.activeInHierarchy);
        canvas.Setup();
        canvas.Open();
        //Debug.Log(canvas +" : " + canvas.gameObject.activeInHierarchy);
        return canvas;
    }
    // dong canvas sau t (s)
    public void CloseUI<T>(float time) where T : UICanvas
    {
        if (IsUIOpened<T>())
        {
            canvasActives[typeof(T)].Close(time);
        }
    }
    // kiem tra canvas da duoc tao hay chua
    public bool IsUILoaded<T>() where T : UICanvas
    {
        return canvasActives.ContainsKey(typeof(T)) && canvasActives[typeof(T)] != null;
    }
    
    // kiem tra canvas duoc active hay chua
    public bool IsUIOpened<T>() where T : UICanvas
    {
        return IsUILoaded<T>() && canvasActives[typeof(T)].gameObject.activeInHierarchy;
    }
    
    // lay active canvas
    public T GetUI<T>() where T : UICanvas
    {
        if (!IsUILoaded<T>())
        {
            T prefab = GetUIPrefab<T>();
            T canvas =  Instantiate(prefab, parent);
            canvasActives[typeof(T)] = canvas;
        }
        return canvasActives[typeof(T)] as T;
    }
    private T GetUIPrefab<T>() where T : UICanvas
    {
        return canvasPrefabs[typeof(T)] as T;
    }
    // dong tat ca
    public void CloseAll()
    {
        foreach (var canvas in canvasActives)
        {
            if (canvas.Value != null && canvas.Value.gameObject.activeInHierarchy)
            {
                canvas.Value.Close(0);
                Debug.Log(canvas +" : " + canvas.Value.gameObject.activeInHierarchy);
            }
            else
            {
                Debug.Log($"Canvas null ? : {canvas.Key} + {canvas.Value}, Canvas active ? : {canvas.Value.gameObject.activeSelf}");
            }
        }
    }
}
