using UnityEngine;

public class MapManager : SingletonMono<MapManager>
{
    [SerializeField] private Transform mapParent;


    private void Start()
    {
        GameManager.Instance.OnLevelStart += SetDeactive;
        GameManager.Instance.OnMainMenu += SetActive;
        
        
        
    }

    public void SetDeactive()
    {
        mapParent.gameObject.SetActive(false);
    }

    public void SetActive()
    {
        mapParent.gameObject.SetActive(true);
    }
}
