using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using UnityEngine.SceneManagement;

public class DataPersistence: MonoBehaviour
{
    [Header("File Name config")]
    [SerializeField] private string filename;
    [SerializeField] private bool UseEncryption;

    [Header("Auto Save configure")]
    [SerializeField] private float autoSaveTimeinSeconds = 120f;


    private FileDataHandling dataHandling;
    private string selectedProfileId = "";

    private PlayerData playerData;
    private  List<IDataPersistence> dataPersistenceObjects;

    private Coroutine autosaveCoroutine;

    public static DataPersistence instance { get; private set; }


    
    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Found more than one datapersistence");
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        this.dataHandling = new FileDataHandling(Application.persistentDataPath, filename, UseEncryption);

        InitialiseSelectedProfileId();
    }



    private void OnEnable()
    {
        SceneManager.sceneLoaded += onSceneLoad;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= onSceneLoad;
    }



    public void onSceneLoad(Scene scene, LoadSceneMode mode)
    {
        this.dataPersistenceObjects = FindallDataPersistenceObjects();
        LoadGame();

        if (autosaveCoroutine != null)
        {
            StopCoroutine(autosaveCoroutine);
        }
        autosaveCoroutine = StartCoroutine(AutoSave());
    }

    public void NewGame()
    {
        this.playerData = new PlayerData();
    }

    public void LoadGame()
    {

        this.playerData = dataHandling.Load(selectedProfileId);

        if (this.playerData== null)
        {
            Debug.Log("No data was found loading new game");
            return;
        }


        foreach (IDataPersistence dataPersistenceobj in dataPersistenceObjects)
        {
            dataPersistenceobj.LoadData(playerData);
        }

    }

    public void SaveGame() 
    {
        if (playerData == null)
        {
            Debug.LogWarning("No data was found. A New Game needs to be started before data can be saved.");
            return;
        }


        foreach (IDataPersistence dataPersistenceobj in dataPersistenceObjects)
        {
            dataPersistenceobj.SaveData(playerData);
        }

        playerData.lastUpdated= System.DateTime.Now.ToBinary();

        dataHandling.Save(playerData, selectedProfileId);

    }

 
  
    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistence> FindallDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    public bool hasGameSave()
    {
        return playerData != null;
    }
   
    public Dictionary<string, PlayerData> getAllProfilesPlayerData()
    {
        return dataHandling.LoadAllProfiles(); 
    }

    public void changeSelectedProfile(string newprofileId)
    {
        this.selectedProfileId= newprofileId;
        LoadGame();
    }

    public void  DeleteProfile(string profileId)
    {
        dataHandling.DeleteProfile(profileId);
        InitialiseSelectedProfileId();
         
        LoadGame();
    }

    private void InitialiseSelectedProfileId()
    {
        this.selectedProfileId = dataHandling.GetMostRescentSaveProfileId();
    }

    private  IEnumerator AutoSave()
    {
        while (true)
        {
            yield return new WaitForSeconds(autoSaveTimeinSeconds);
            SaveGame();
            Debug.Log("AutoSaved Game");
        }
    }

}
