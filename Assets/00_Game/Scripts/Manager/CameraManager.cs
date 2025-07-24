using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField] private MainCameraController mainCameraController;

    private void Awake()
    {
        if (mainCameraController == null)
        {
            mainCameraController = GetComponentInChildren<MainCameraController>();
        }
        GameManager.Instance.OnMainMenu += () =>
        {
            mainCameraController.enabled = true;
            mainCameraController.InitPosCam();
        };
    }
}
