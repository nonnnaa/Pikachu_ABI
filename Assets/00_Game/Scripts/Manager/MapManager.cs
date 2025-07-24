using System.Collections.Generic;
using UnityEngine;

public class MapManager : SingletonMono<MapManager>
{
    [SerializeField] private Transform mapParent;
    [SerializeField] private List<Transform> maps;

    private void Awake()
    {
        GameManager.Instance.OnLevelStart += SetDeactive;
        GameManager.Instance.OnMainMenu += SetActive;
        if (maps == null || maps.Count == 0)
        {
            Debug.LogError("Map Null");
        }
    }
    public void SetDeactive()
    {
        mapParent.gameObject.SetActive(false);
    }

    public void SetActive()
    {
        mapParent.gameObject.SetActive(true);
    }

    public SoundBGType GetBGNearest(Transform target)
    {
        float minDistance = float.MaxValue;
        int minIndex = 0;
        for (int i = 0; i < maps.Count; i++)
        {
            if (Mathf.Abs(target.position.y - maps[i].position.y) < minDistance)
            {
                minDistance = Mathf.Abs(target.position.y - maps[i].position.y);
                minIndex = i;
            }
        }
        return (SoundBGType)(minIndex + 1); // Cong len 1 cho trung voi enum (enum = 0 la StartGame Music)
    }
}
