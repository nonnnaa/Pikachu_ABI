using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UIManager.Instance.OpenUI<CanvasMainMenu>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V) && UIManager.Instance.IsUIOpened<CanvasGamePlay>())
        {
            UIManager.Instance.CloseAll();
            UIManager.Instance.OpenUI<CanvasVictory>();
        }

        if (Input.GetKeyDown(KeyCode.F) && UIManager.Instance.IsUIOpened<CanvasGamePlay>())
        {
            UIManager.Instance.CloseAll();
            UIManager.Instance.OpenUI<CanvasLoseGame>();           
        }
    }
}
