using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//限定泛型为Component
public class Singleton<T> : MonoBehaviour where T : Component
{
    public static T Instance {get; private set;}

    protected virtual void Awake()
    {
        Instance = this as T;
        Init();
    }

    protected virtual void Init()
    {
        
    }
}
