using UnityEngine;
using System.Collections.Generic;

public class PersistentObject : MonoBehaviour
{
    private static Dictionary<string, PersistentObject> instances = new Dictionary<string, PersistentObject>();

    void Awake()
    {
        string key = gameObject.name;

        if (!instances.ContainsKey(key))
        {
            instances[key] = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}