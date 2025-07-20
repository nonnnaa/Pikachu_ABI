using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 targetPosition;
    private bool isPanning;
    [SerializeField] private Transform bottomLeft, topRight;
    [SerializeField] private float panSpeed = 5f;
    [SerializeField] private Camera cam;
    private Vector3 touchStart;
    void Update()
    {
        if (UIManager.Instance.IsUIOpened<CanvasMainMenu>())
        {
            if (Input.GetMouseButtonDown(0))
            {
                touchStart = cam.ScreenToWorldPoint(Input.mousePosition);
            }
     
            if (Input.GetMouseButton(0))
            {
                Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                float finalYPos = cam.transform.position.y + direction.y;
                finalYPos = Mathf.Clamp(finalYPos, bottomLeft.transform.position.y, topRight.transform.position.y);
                if (Mathf.Abs(cam.transform.position.y - finalYPos) > 0.1f)
                {
                    cam.transform.position = Vector3.Lerp(cam.transform.position, new Vector3(cam.transform.position.x, finalYPos, cam.transform.position.z), panSpeed * Time.deltaTime);
                    //Debug.Log(finalYPos);
                }
            }
        }
    }
}
