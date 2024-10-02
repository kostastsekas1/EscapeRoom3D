using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;



public class MyPadlock :InteractableObject, IDataPersistence
{
    [SerializeField] private string id;

    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }



    [SerializeField]
    private bool isLocked;
    public bool Unlocked = false;
    public bool IsLocked
    {
        get
        {
            return isLocked;
        }
    }
    public GameObject lockingArc;
    public List<MyPadlockWheel> wheels;
    public string[] rightCode = { "0", "0", "0" };

    public AudioSource audioSource;
    public AudioClip wheelSound;
    public AudioClip openingSound;

    public Vector3 LockPosition;
    public Quaternion LockRotation;
    public Vector3 LockScale;
    public static bool IsFocused = false;
 
    public override void Awake()
    {
        gameObject.layer = 6;
    }

    void Start()
    {
        foreach (MyPadlockWheel wheel in wheels)
        {
            wheel.onWheelChangedEvent.AddListener(OnWheelChanged);
        }

    }


    public override void OnInteract(Transform pickUpSlot)
    {
        if (Unlocked)
        {
            return;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
        LockPosition = this.transform.position;
        LockRotation = this.transform.rotation;
        LockScale = this.transform.lossyScale;
        IsFocused = true;



       
       
        this.gameObject.transform.position = new Vector3(0.0061f, -0.0127f, -1.5915f);
        this.gameObject.transform.localScale = new Vector3(10f, 10f, 10f);
        this.gameObject.transform.rotation = Quaternion.Euler(-90, 0, 270);
        this.gameObject.transform.SetParent(pickUpSlot.transform, false);
    }

    public override void OnFocus()
    {
        string code = "( ";
        foreach(string digit in rightCode)
        {
            code = code + digit+ " ";
        }
        code = code + ")";

        Debug.Log(code);
    }

    public override void OnLoseFocus()
    {
        Debug.Log("LostFocus");
        

    }

    public override void OnDrop()
    {
        this.transform.position = LockPosition;
        this.transform.rotation = LockRotation;
        this.transform.localScale = LockScale;

        gameObject.transform.SetParent(null);
            
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        IsFocused= false;
    }



    public void OnWheelChanged()
    {
        PlaySound(wheelSound);

        bool unlocked = true;
            
        for (int i = 0; i < wheels.Count; i++)
        {
            if (wheels[i].GetCurrentValue() != rightCode[i])
            {
                Debug.Log("Value of Wheel " + wheels[i].GetCurrentValue() + "  does not Match With right Code digit " + i + " with value " + rightCode[i]);
                unlocked = false;
                break;
            }
            else
            {
                Debug.Log("Value of Wheel " + wheels[i].GetCurrentValue() + " Matches With right Code digit " + i + " with value " + rightCode[i]);
            }
        }
        if (unlocked)
        {
            Unlock();
            Unlocked=true;
            this.gameObject.tag = "Door";
        }
            
    }

    public void Lock()
    {
        isLocked = true;
        if (lockingArc != null)
        {
            lockingArc.transform.localPosition = new Vector3(0f, 0f, 0f);
        }
    }

    public void Unlock()
    {
        isLocked = false;
        if (lockingArc != null)
        {
            lockingArc.transform.localPosition = new Vector3(0f, 0f, 0.001f);
        }
        PlaySound(openingSound);
        gameEventSystem.instance.LockUnlocked();
    }

    private void PlaySound(AudioClip sound)
    {
        if (sound != null)
        {
            audioSource.clip = sound;
            audioSource.Play();
        }
    }

    public void LoadData(PlayerData data)
    {
        data.LocksUnclocked.TryGetValue(id, out Unlocked);
        Debug.Log("The lock"+ gameObject.name+" is "+ Unlocked);

        if (Unlocked)
        {
            if (lockingArc != null)
            {
                lockingArc.transform.localPosition = new Vector3(0f, 0f, 0.001f);
            }
        }
    }

    public void SaveData(PlayerData data)
    {
        if (data.LocksUnclocked.ContainsKey(id))
        {
            data.LocksUnclocked.Remove(id);
        }
        data.LocksUnclocked.Add(id, Unlocked);

    }

}
