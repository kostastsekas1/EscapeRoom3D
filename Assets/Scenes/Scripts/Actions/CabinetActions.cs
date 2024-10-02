using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CabinetActions : InteractableObject
{
    
    public Animator Cabinetanimator;
    public bool isOpen = false;
    [Header("Lock")]
    [SerializeField] public MyPadlock Lock;

    public AudioSource audioSource;
    public AudioClip openingSound;
    public AudioClip LockedSound;
    public AudioClip ClosingSound;
    public string openingClip;
    public string closingClip;

    private enum CabinetState
    {
        Closed,
        Opening,
        Opened,
        Closing
    }

    private CabinetState cabinestate = CabinetState.Closed;

    public override void OnDrop()
    {
        return;
    }

    public override void OnFocus()
    {
        Debug.Log("Looking at door" + gameObject.name);
        if (Input.GetMouseButtonDown(0))
        {
            if( Lock == null)
            {
                Cabinetaction();
                return;
            }
            
            if(Lock.Unlocked)
            {
                Cabinetaction();
            }
            else
            {
                audioSource.PlayOneShot(LockedSound);

            }
        }
    }

    public void Cabinetaction()
    {
        if (cabinestate == CabinetState.Closed && !(cabinestate == CabinetState.Closing))
        {
            Debug.Log("opening Door");
            Cabinetanimator.Play(openingClip, 0, 0.0f);
            audioSource.PlayOneShot(openingSound);
            cabinestate = CabinetState.Opening;
            StartCoroutine(WaitForAnimation());
            return;
        }
        if (cabinestate == CabinetState.Opened && !(cabinestate == CabinetState.Opening))
        {
            Debug.Log("Closing Door");
            Cabinetanimator.Play(closingClip, 0, 0.0f);
            audioSource.PlayOneShot(ClosingSound);

            cabinestate = CabinetState.Closing;

            StartCoroutine(WaitForAnimation());
            return;
        }

    }

    IEnumerator WaitForAnimation()
    {

        while (Cabinetanimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.8f);

        if (cabinestate == CabinetState.Opening)
        {
            isOpen = true;
            cabinestate = CabinetState.Opened;

        }
        else if (cabinestate == CabinetState.Closing)
        {
            isOpen = false;
            cabinestate = CabinetState.Closed;

        }
    }

    public override void OnLoseFocus()
    {
        Debug.Log("Lost focus on door");
    }

    public override void OnInteract(Transform pickUpSlot)
    {
        return;
    }
}
