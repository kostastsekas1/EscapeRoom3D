using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScene : MonoBehaviour
{

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
    public void LoadMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 1f;
        Debug.Log("LoadingMenu...");
        DataPersistence.instance.SaveGame();
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex - 2);
    }
    public void Quit()
    {
        Debug.Log("ExitingGame ...");
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;

    }
}
