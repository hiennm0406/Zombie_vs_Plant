using UnityEngine;
using System.Collections;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T sharedInstance;
    public static T SharedInstance
    {
        get
        {
            return sharedInstance;
        }
    }
    protected virtual void Awake()
    {
        if (sharedInstance == null)
        {
            sharedInstance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
            return;
        }
    }
}
