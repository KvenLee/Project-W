using UnityEngine;

/*******************************
 *  Author: Andrea Chow.
 *  Time:   
 *  Function: 继承MonoBehavior的单例模板
 * *****************************/

public class TUSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance;

    public virtual void Awake()
    {
        if (Instance == null)
            Instance = this as T;
    }

    void OnDestroy()
    {
        Notify.Event.deregister(this);
    }
}
