using System;
using UnityEngine;

public class gameEventSystem : MonoBehaviour
{
    public static gameEventSystem instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Game Events Manager in the scene.");
        }
        instance = this;
    }

    public event Action onLockUnlocked;
    public void LockUnlocked()
    {
        if (onLockUnlocked != null)
        {
            onLockUnlocked();
        }
    }
}
