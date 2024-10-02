using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInteractable : InteractableObject
{
    public override void OnDrop()
    {
       gameObject.transform.SetParent( null);
       Rigidbody rb = GetComponent<Rigidbody>();
       if (rb != null)
       {
           rb.isKinematic = false;
       }
    }

    public override void OnFocus()
    {
       print("Looking at "+ gameObject.name);
    }

    public override void OnInteract(Transform pickUpSlot)
    {
        Rigidbody rb= GetComponent<Rigidbody>();
        gameObject.transform.position = Vector3.zero;
        gameObject.transform.rotation = Quaternion.identity;
        gameObject.transform.SetParent(pickUpSlot.transform, false);
        if(rb!= null)
        {
            rb.isKinematic= true;
        }
        return;
    }

    public override void OnLoseFocus()
    {
        print("Stopped looking at " + gameObject.name);

    }
}
