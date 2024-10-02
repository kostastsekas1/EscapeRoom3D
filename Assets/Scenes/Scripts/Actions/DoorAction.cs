using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DoorAction : InteractableObject, IDataPersistence
{
    public Animator dooranimator;
    public bool isOpen = false;
    [Header("Lock")]
    [SerializeField] public MyPadlock Lock;
    public bool isExitDoor = false;
    public int LocksUnlocked = 0;

    public AudioSource audioSource;
    public AudioClip LockedSound;
    private enum DoorState
    {
        Closed,
        Opening,
        Opened,
        Closing
    }

    private DoorState doorState = DoorState.Closed;


    private void Start()
    {
        // subscribe to events
        gameEventSystem.instance.onLockUnlocked += onLockUnlocked;
    }

    private void OnDestroy()
    {
        // unsubscribe from events
        gameEventSystem.instance.onLockUnlocked -= onLockUnlocked;
    }

    private void onLockUnlocked()
    {
        LocksUnlocked++;
    }


    public override void OnDrop()
    {
        gameObject.transform.SetParent(null);
    }

    public override void OnFocus()
    {
        Debug.Log("Looking at door" + gameObject.name);

        if (Input.GetMouseButtonDown(0))
        {
            if (Lock == null)
            {
                Dooraction();
                return;
            }

            if (Lock.Unlocked)
            {
                Dooraction();
            }
            else
            {
                audioSource.PlayOneShot(LockedSound);

            }
        }
    }

    public void Dooraction()
    {

        if (doorState == DoorState.Closed && !(doorState == DoorState.Closing))
        {

            if (isExitDoor && LocksUnlocked == 6)
            {
                Debug.Log("Opening Door");
                dooranimator.Play("DoorOpening", 0, 0.0f);
                doorState = DoorState.Opening;

                StartCoroutine(WaitForAnimation());
                DataPersistence.instance.SaveGame();
                SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);

            }
            else if (isExitDoor && LocksUnlocked != 6)
            {
                return;
            }
            else
            {
                Debug.Log("Opening Door");
                dooranimator.Play("DoorOpening", 0, 0.0f);
                doorState = DoorState.Opening;

                StartCoroutine(WaitForAnimation());
            }

        }
        else if (doorState == DoorState.Opened && !(doorState == DoorState.Opening))
        {
            Debug.Log("Closing Door");
            dooranimator.Play("DoorClosing", 0, 0.0f);

            doorState = DoorState.Closing;

            StartCoroutine(WaitForAnimation());
        }

    }

    IEnumerator WaitForAnimation()
    {
        
        while (dooranimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f);

        if (doorState == DoorState.Opening)
        {
            isOpen = true;
            doorState = DoorState.Opened;
          
        }
        else if (doorState == DoorState.Closing)
        {
            isOpen = false;
            doorState = DoorState.Closed;
       
        }
      
    }


    public override void OnLoseFocus()
    {
        Debug.Log("Lost focus on door");
    }

    public override void OnInteract(Transform pickUpSlot)
    {
        OnDrop();
        return;
    }

    public void LoadData(PlayerData data)
    {
        LocksUnlocked = data.puzzlesSolved;
    }

    public void SaveData(PlayerData data)
    {
        //data.puzzlesSolved = LocksUnlocked;
        return;
    }

    private void Update()
    {
        Debug.Log(LocksUnlocked);
    }
}
