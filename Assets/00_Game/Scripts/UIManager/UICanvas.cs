using System.Collections;
using UnityEngine;

public class UICanvas : MonoBehaviour
{
    [SerializeField] bool isDestroyOnClose;
    private void Awake()
    {
        RectTransform rect = GetComponent<RectTransform>();
        float ratio = Screen.width / (float)Screen.height;
        if (ratio < 2.1f)
        {
            Vector2 leftBottom = rect.offsetMin;
            Vector2 rightTop = rect.offsetMax;
            
            leftBottom.y = 0;
            rightTop.y = -50;
            
            rect.offsetMin = leftBottom;
            rect.offsetMax = rightTop;
        }
    }
    
    public virtual void Setup()
    {
        
    }
    public virtual void Open()
    {
        gameObject.SetActive(true);
    }
    public virtual void Close(float time)
    {
        StartCoroutine(CloseCoroutine(time));
    }


    IEnumerator CloseCoroutine(float time)
    {
        if (isDestroyOnClose)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(time);
    }
}
