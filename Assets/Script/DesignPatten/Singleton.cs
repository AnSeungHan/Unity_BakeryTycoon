using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour
    where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (!instance)
            {
                instance = (T)FindObjectOfType(typeof(T));

                if (!instance)
                {
                    GameObject obj = new GameObject(typeof(T).Name, typeof(T));
                    instance = obj.GetComponent<T>();
                }
            }

            return instance;
        }
    }

    protected void Awake()
    {
        if (instance)
            return;

        instance = Instance;

        if (transform.parent && transform.root)
            DontDestroyOnLoad(this.transform.root.gameObject);
        else
            DontDestroyOnLoad(this.gameObject);
    }

    /*protected void OnEnable()
    {

    }

    protected void Start()
    {

    }*/
}
