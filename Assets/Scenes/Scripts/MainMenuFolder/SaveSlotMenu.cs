using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveSlotMenu : MonoBehaviour
{
    [Header("Main Menu Navigation")]
    [SerializeField] private MainMenu mainMenu;

    [Header("BackButton")]
    [SerializeField] private Button backButton;

    private SaveSlot[] saveSlots;

    [Header("Confirmation PopUp")]
    [SerializeField] private PopUpMenu popUpMenu; 


    private bool isLoadingGame =false;

    private void Awake()
    {
        saveSlots = this.GetComponentsInChildren<SaveSlot>();
        
    }

    public void activateMenu(bool isloadingGame)
    {


        this.gameObject.SetActive(true);

        this.isLoadingGame = isloadingGame;

        backButton.interactable= true;
        Dictionary<string, PlayerData> profiles =  DataPersistence.instance.getAllProfilesPlayerData(); 

        foreach (SaveSlot saveSlot in saveSlots) 
        {

            PlayerData profileData = null;
            profiles.TryGetValue(saveSlot.getProfileId(), out profileData);
            saveSlot.setData(profileData);

            if (profileData == null && isLoadingGame ) 
            {
                saveSlot.setInteractable(false);
            }
            else
            {
                saveSlot.setInteractable(true);
                returnPlayerDataLoaded(profileData);
            }

        }
    }
    
    public void deactivateMenu()
    {
        this.gameObject.SetActive(false);
    }

    public void OnBack()
    {
        mainMenu.ActivateMenu();
        this.deactivateMenu();
    }


    public void SaveSlotClicked(SaveSlot saveslot)
    {
        DisableMenuButtons();


        if(isLoadingGame)
        {
            DataPersistence.instance.changeSelectedProfile(saveslot.getProfileId());
            saveGameAndLoad();

        }
        else if(saveslot.hasData)
        {
            popUpMenu.ActivateMenu("Starting a New Game in this slot. Are you sure you want to override the already existing one?",
                    () =>
                    {
                        DataPersistence.instance.changeSelectedProfile(saveslot.getProfileId());
                        DataPersistence.instance.NewGame();
                        saveGameAndLoad();
                    },

                    () =>
                    {
                        this.activateMenu(isLoadingGame);
                    }
                );
        }
        else
        {
            DataPersistence.instance.changeSelectedProfile(saveslot.getProfileId());
            DataPersistence.instance.NewGame();
            saveGameAndLoad();
        }


       
    }

    private void DisableMenuButtons()
    {
        foreach (SaveSlot saveSlot in saveSlots)
        {
            saveSlot.setInteractable(false);
        }
        backButton.interactable = false;
    }

    public void ClearButtonClicked(SaveSlot saveslot)
    {
        DisableMenuButtons();
        popUpMenu.ActivateMenu("Are you sure you want to delete the data in this save?",
                    () =>
                    {
                        DataPersistence.instance.DeleteProfile(saveslot.getProfileId());
                        activateMenu(isLoadingGame);
                    },

                    () =>
                    {
                        this.activateMenu(isLoadingGame);
                    }
                );



        
    }

    private void saveGameAndLoad()
    {
        DataPersistence.instance.SaveGame();
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public PlayerData returnPlayerDataLoaded(PlayerData playerData)
    {
        return playerData;
    }
}

