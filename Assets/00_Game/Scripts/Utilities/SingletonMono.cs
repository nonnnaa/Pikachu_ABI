using UnityEngine;
public class SingletonMono<T>: MonoBehaviour where T: MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if(instance==null)
            {
                instance = (T)FindAnyObjectByType(typeof(T));
                if(instance==null)
                {
                    GameObject go = new GameObject();
                    go.name = "[@" + typeof(T).Name + "]";
                    instance = go.AddComponent<T>();
                }
            }
            return instance;
        }
    }
}