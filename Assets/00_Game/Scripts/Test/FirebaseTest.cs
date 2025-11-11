using Firebase;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseTest : MonoBehaviour
{
    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                Debug.Log("Firebase khởi tạo thành công!");
                // Có thể gọi Firebase Analytics, Auth, v.v... ở đây
            }
            else
            {
                Debug.LogError("Không thể khởi tạo Firebase: " + dependencyStatus);
            }
        });
    }
}