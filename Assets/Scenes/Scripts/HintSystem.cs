using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HintSystem : MonoBehaviour ,IDataPersistence
{
    public int LocksUnlocked = 0;

    public GameObject hintUI;

    public TextMeshProUGUI hint;

    public Animator hintanimator;
   
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (hintUI.activeSelf)
            {
                return;
            }
            else
            {
                giveHint();
            }
        }

        
    }

    private void giveHint()
    {
        if (LocksUnlocked == 0)
        {
            this.hint.text = "The items in the list seem to be in order";
        }else if (LocksUnlocked == 1)
        {
            this.hint.text = "This stone on the pinboard looks like it has been partially deciphered maybe you should take a look at it";
        }else if(LocksUnlocked == 2)
        {
            this.hint.text = "Maybe sometimes the <i>solution</i> is hidden in plain sight";
        }else if (LocksUnlocked == 3)
        {
            
            this.hint.text = "Some items are hidden and are out of reach maybe you should explore the room some more";
        }else if (LocksUnlocked == 4)
        {
            this.hint.text = "Maybe the dates on the journals can be used to order them";
        }else if (LocksUnlocked == 5)
        {
            this.hint.text = "You should be able to escape";
        }


        hintanimator.Play("Hintanimation",0,0);
        hintUI.SetActive(true);
        StartCoroutine(disableHint());
    }

   IEnumerator disableHint()
    {
        yield return new WaitForSeconds(5.0f);
        hintUI.SetActive(!hintUI.activeSelf);

    }
    public void LoadData(PlayerData data)
    {
        LocksUnlocked = data.puzzlesSolved;
    }

    public void SaveData(PlayerData data)
    {
        
        data.puzzlesSolved = LocksUnlocked;
    }
}
