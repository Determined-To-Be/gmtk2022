using UnityEngine;

public abstract class SingletonBehavior : MonoBehaviour
{
    protected static SingletonBehavior instance = null;

    public static SingletonBehavior Instance
    {
        get;
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        
    }
}