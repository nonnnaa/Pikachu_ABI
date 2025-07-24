using UnityEngine;
public class MainCameraController : MonoBehaviour
{
    [SerializeField] private Transform bottomLeft, topRight;
    [SerializeField] private float scrollSpeed = 5f;
    [SerializeField] private Camera cam;
    private Vector3 touchStart;
    private SoundBGType currentBG = SoundBGType.SoundBG1;

    void LateUpdate()
    {
        if (!UIManager.Instance.IsUIOpened<CanvasMainMenu>())
        {
            InitPosCam();
            enabled = false;
            return;
        }
        
        // if (Input.GetMouseButtonDown(0))
        // {
        //     touchStart = cam.ScreenToWorldPoint(Input.mousePosition);
        // }
        //
        // if (Input.GetMouseButton(0))
        // {
        //     Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //     float finalYPos = cam.transform.position.y + direction.y;
        //     finalYPos = Mathf.Clamp(finalYPos, bottomLeft.transform.position.y, topRight.transform.position.y);
        //     if (Mathf.Abs(cam.transform.position.y - finalYPos) > 0.1f)
        //     {
        //         cam.transform.position 
        //             = Vector3.MoveTowards(cam.transform.position, new Vector3(cam.transform.position.x, finalYPos, cam.transform.position.z), scrollSpeed * Time.deltaTime);
        //     }
        //     SoundBGType tmp = MapManager.Instance.GetBGNearest(transform);
        //     if (currentBG != tmp)
        //     {
        //         currentBG = tmp;
        //         if (!isFirst)
        //         {
        //             SoundManager.Instance.SetMusicBG(currentBG);
        //         }
        //         isFirst =  false;
        //     }
        //}
        
#if UNITY_EDITOR || UNITY_STANDALONE
        // Xử lý bằng chuột
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            HandleCameraDrag(Input.mousePosition);
        }
#elif UNITY_ANDROID || UNITY_IOS
    // Xử lý bằng cảm ứng
    if (Input.touchCount > 0)
    {
        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            touchStart = cam.ScreenToWorldPoint(touch.position);
        }

        if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
        {
            HandleCameraDrag(touch.position);
        }
    }
#endif
    }
    public void InitPosCam()
    {
        cam.transform.position = bottomLeft.transform.position;
    }
    void HandleCameraDrag(Vector3 inputPosition)
    {
        Vector3 direction = touchStart - cam.ScreenToWorldPoint(inputPosition);
        float finalYPos = cam.transform.position.y + direction.y;
        finalYPos = Mathf.Clamp(finalYPos, bottomLeft.transform.position.y, topRight.transform.position.y);

        if (Mathf.Abs(cam.transform.position.y - finalYPos) > 0.1f)
        {
            cam.transform.position = Vector3.MoveTowards(
                cam.transform.position,
                new Vector3(cam.transform.position.x, finalYPos, cam.transform.position.z),
                scrollSpeed * Time.deltaTime
            );
        }

        SoundBGType tmp = MapManager.Instance.GetBGNearest(transform);
        if (currentBG != tmp)
        {
            currentBG = tmp;

            SoundManager.Instance.SetMusicBG(currentBG);
        }
    }
}
