using UnityEngine;
public class SingletonMono<T>: MonoBehaviour where T: MonoBehaviour
{
    private static T instance_;
    public static T Instance
    {
        get
        {
            if(instance_==null)
            {
                instance_ = (T)FindAnyObjectByType(typeof(T));
                if(instance_==null)
                {
                    GameObject go = new GameObject();
                    go.name = "[@" + typeof(T).Name + "]";
                    instance_ = go.AddComponent<T>();
                }
            }
            return instance_;
        }
    }
}