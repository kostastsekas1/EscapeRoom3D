using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    [Header("Profile")]
    [SerializeField] private string profileId = "";


    [Header("Save")]
    [SerializeField] private GameObject NoContent;
    [SerializeField] private GameObject hasContent;
    [SerializeField] private TextMeshProUGUI SaveDate;
    [SerializeField] private TextMeshProUGUI CompletionPercentage;

    private Button saveslotButton;

    [Header("Clear Button")]
    [SerializeField] private Button ClearButton;


    public bool hasData { get; private set; } = false;

    private void Awake()
    {
        saveslotButton = this.GetComponent<Button>();
    }

    public void setData(PlayerData data)
    {
        if(data==null)
        {
            hasData= false;
            NoContent.SetActive(true);
            hasContent.SetActive(false);
            ClearButton.gameObject.SetActive(false);

        }
        else
        {
            hasData = true;
            NoContent.SetActive(false);
            hasContent.SetActive(true);
            ClearButton.gameObject.SetActive(true);

            CompletionPercentage.text = data.getPercentageOfRiddlesCompleted() + "% Complete";
            SaveDate.text = "Last Saved at: " + data.saveDate;

        }
    }

    public string getProfileId()
    {
        return this.profileId;    
    
    }

    public  void setInteractable(bool interactable)
    {
        saveslotButton.interactable = interactable;
        ClearButton.interactable = interactable;
    }

}
