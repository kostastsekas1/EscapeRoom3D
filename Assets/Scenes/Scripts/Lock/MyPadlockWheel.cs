using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MyPadlockWheel : InteractableObject
{
    public int currentIndex = 9;
    public string[] numbers = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
    public string[] letters ={ "a", "l", "c", "d", "e", "f", "r", "h", "i", "j" };
    public string[] lettersv2 = { "a", "t", "o", "d", "e", "n", "r", "h", "i", "j" };

    public string[] values = { };
    [SerializeField] public bool UsesLetters=false;
    [SerializeField] public bool UsesLettersv2 = false;


    public UnityEvent onWheelChangedEvent; //gets a listener added by the padlock to what it belongs

    private void Start()
    {
        if (UsesLetters)
        {
            if (UsesLettersv2)
            {
                values = lettersv2;
            }
            else
            {
                values = letters;
            }
        }
        else
        {

            values = numbers;
        }
    }

    public override void OnDrop()
    {
        return;
    }

    public override void OnFocus()
    {
        Debug.Log("looking at wheel " + gameObject.name);
        if (Input.GetMouseButtonDown(0))
        {
                wheelRotate(currentIndex + 1);
        }
    }

    public override void OnInteract(Transform pickUpSlot)
    {   
        return;
    }


    public override void OnLoseFocus()
    {
        Debug.Log(" stopped looking at wheel " + gameObject.name);
        return;
    }

    public void wheelRotate(int newIndex)
    {
        if (newIndex < 0 || newIndex >= values.Length)
        {
            newIndex= 0;
        };

        float degreesToRotate;
        if (newIndex < currentIndex)
        {
            degreesToRotate = (currentIndex - newIndex) * -1f * 360f / values.Length;
        }
        else
        {
            degreesToRotate = (newIndex - currentIndex) * 360f / values.Length;
        }
        transform.Rotate(new Vector3(0f, 0f, degreesToRotate));

       
        currentIndex = newIndex;
        OnWheelChanged();
        Debug.Log("Turning wheel " + gameObject.name +" is at the number " + values[currentIndex]  +"with index " + currentIndex);
    }

    public void OnWheelChanged()
    {
        onWheelChangedEvent.Invoke();
    }

    public string GetCurrentValue()
    {
       return values[currentIndex];
    }   
       
}
