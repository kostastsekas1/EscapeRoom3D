using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Menu Navigation")]
    [SerializeField] private SaveSlotMenu saveSlotMenu;


    [Header("MenuButtons")]
    [SerializeField] private Button NewGameButton;
    [SerializeField] private Button ContinueGameButton;
    [SerializeField] private Button LoadGameButton;
    [SerializeField] private Button OptionsButton;
    [SerializeField] private Button QuitButton;


    private void Start()
    {
        DisablebuttonsifdatanotExist();
    }

    private void DisablebuttonsifdatanotExist()
    {
        if (!DataPersistence.instance.hasGameSave())
        {
            ContinueGameButton.interactable = false;
            LoadGameButton.interactable = false;
        }
    }


    public void NewGame()
    {
        

        saveSlotMenu.activateMenu(false);

        this.DeactivateMenu();
    }

    public void LoadGame()
    {
        saveSlotMenu.activateMenu(true);
        this.DeactivateMenu();
    }

    public void ContinueGame()
    {
        disableButtons();
        Debug.Log("Continue Game Loading...");
        DataPersistence.instance.SaveGame();
        Debug.Log("Continue Game saved");

        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);

    }



    public void Quit()
    {
        disableButtons();
        Debug.Log("ExitingGame ...");
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;

    }

    private void disableButtons()
    {
        NewGameButton.interactable = false;
        ContinueGameButton.interactable = false;
        LoadGameButton.interactable = false;
        OptionsButton.interactable = false;
        QuitButton.interactable = false;
    }

    public void ActivateMenu()
    {
        this.gameObject.SetActive(true);
        DisablebuttonsifdatanotExist();

    }

    public void DeactivateMenu() 
    {
        this.gameObject.SetActive(false);
    }
}
