using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool IsGamePaused = false;
    [Header("PauseMenu Variables")]
    [SerializeField] public GameObject PauseMenuUI;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            
            if (IsGamePaused)
            {
                
                Resume();
            }
            else 
            {
                
                Pause();
            }
        }
    }

    public void Resume() 
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        IsGamePaused= false;
    }

    void Pause() 
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        IsGamePaused= true;
       
    }
    public void LoadMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 1f;
        Debug.Log("LoadingMenu...");
        DataPersistence.instance.SaveGame();
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex - 1);
    }
    public void Quit()
    {
        Debug.Log("ExitingGame ...");
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying= false;

    }
}
