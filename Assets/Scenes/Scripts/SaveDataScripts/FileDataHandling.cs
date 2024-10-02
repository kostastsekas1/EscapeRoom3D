using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.Serialization.Formatters.Binary; 
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Playables;
using UnityEngine.Profiling;

public  class FileDataHandling
{
    private string dataDirPath = "";
    private string dataFileName = "";
    private bool useEncryption=false;
    private readonly string encryptionSecretKey = "word";
    private readonly string backupFileExtension = ".bak";


    public FileDataHandling(string datadirpath, string datafilename, bool useEncryption) 
    {
        
        this.dataDirPath = datadirpath;
        this.dataFileName = datafilename;
        this.useEncryption = useEncryption;
    }



    public PlayerData Load(string profileId, bool allowRestoreFromBackUp = true)
    {
        if (profileId == null)
        {
            return null;
        }
    
        string fullpath =Path.Combine(dataDirPath, profileId, dataFileName);
        PlayerData data = null;
        if (File.Exists(fullpath))
        {
            try
            {
                string datatoload = "";
                using (FileStream fileStream = new FileStream(fullpath, FileMode.Open))
                {
                    using(StreamReader reader = new StreamReader(fileStream))
                    {
                        datatoload= reader.ReadToEnd();
                    }
                }

                if (useEncryption)
                {
                    datatoload = EncryptDecrypt(datatoload);
                }

                data = JsonConvert.DeserializeObject<PlayerData>(datatoload);

            }
            catch (Exception e)
            {
                if(allowRestoreFromBackUp)
                {
                    Debug.LogWarning("Error occured when trying to load save from " + fullpath + " with error " + e);
                    bool rollbackSuccess = AttemptRollback(fullpath);
                    if (rollbackSuccess)
                    {
                        data = Load(profileId,false);
                    }
                }
                else
                {
                    Debug.LogError("Tried to Rollback Failed once");

                }


            }
        }

        return data;
    }

    public void Save(PlayerData data, string profileId)
    {
        if (profileId == null)
        {
            return;
        }


        string fullpath = Path.Combine(dataDirPath, profileId, dataFileName);
        string backupFilePath = fullpath + backupFileExtension;
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullpath));

            string datatoStore = JsonConvert.SerializeObject(data, Formatting.Indented);

            if (useEncryption)
            {
                datatoStore = EncryptDecrypt(datatoStore); 
            }


            using(FileStream fileStream = new FileStream(fullpath, FileMode.Create))
            {
                using(StreamWriter  writer = new StreamWriter(fileStream))
                {
                    writer.Write(datatoStore);
                }
            }

            PlayerData verifiedData = Load(profileId);
            if (verifiedData != null)
            {
                 File.Copy(fullpath,backupFilePath, true);
            }
            else
            {
                throw new Exception("Save file not verified  and backupd could not be made");
            }


        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to save to " + fullpath + " with error " + e);
        }   
    }

    private string EncryptDecrypt(string data)
    {
        string modifiedData = "";
        for (int i = 0; i < data.Length; i++)
        {
            modifiedData += (char)(data[i] ^ encryptionSecretKey[i % encryptionSecretKey.Length]);
        }
        return modifiedData;
    }

    public  string GetMostRescentSaveProfileId()
    {
        string mostrecentprofileid = null;

        Dictionary<string ,PlayerData> profiles = LoadAllProfiles(); 

        foreach(KeyValuePair<string ,PlayerData>pair in profiles)
        {
            string profileId = pair.Key;
            PlayerData data = pair.Value;

            if (data == null)
            {
                continue;
            }

            if(mostrecentprofileid== null)
            {
                mostrecentprofileid = profileId;
            }
            else
            {
                DateTime mostrecent = DateTime.FromBinary(profiles[mostrecentprofileid].lastUpdated);
                DateTime newDateTime = DateTime.FromBinary(data.lastUpdated);
               if(newDateTime >  mostrecent)
                {
                    mostrecentprofileid = profileId;
                }

            }

        }
        return mostrecentprofileid;
    }


    public Dictionary<string,PlayerData> LoadAllProfiles()
    {
        Dictionary<string,PlayerData> profiles = new Dictionary<string,PlayerData>();

        IEnumerable<DirectoryInfo> dirinfos = new DirectoryInfo(dataDirPath).EnumerateDirectories();

        foreach (DirectoryInfo dirinfo in dirinfos)
        {
            string profileId = dirinfo.Name;


            string fullpath =Path.Combine(dataDirPath, profileId, dataFileName);

            if (!File.Exists(fullpath))

            {
                Debug.LogWarning("this doesn not contain data for player  skipping loading it" + profileId);
                continue;
            }

            PlayerData playerdata = Load(profileId);

            if(playerdata!= null)
            {


                profiles.Add(profileId, playerdata); 
            }
            else
            {
                Debug.Log("something went terribly wrong with profile id " + profileId);
            }
        }


        return profiles;
    }

    public void DeleteProfile(string profileid)
    {
        if (profileid == null)
        {
            return;
        }

        string fullpath = Path.Combine(dataDirPath, profileid, dataFileName);
        try
        {
            if (File.Exists(fullpath))
            {
                Directory.Delete(Path.GetDirectoryName(fullpath), true);
            }
            else
            {
                Debug.Log("Tried to delete data but data did not exist");
            }

        }
        catch(Exception e)
        {
            Debug.LogError("Something went wrong while trying to delete data from " + fullpath + " with exception " + e);
        }


    }

    private  bool AttemptRollback(string fullpath)
    {
        bool success = false;

        string backupFilePath = fullpath + backupFileExtension;
        try
        {
            if (File.Exists(backupFilePath))
            {
                File.Copy(backupFilePath, fullpath, true);
                success = true;
                Debug.Log("Successful Rollback to back up file");
            }
            else
            {
                Debug.Log("Tried to Rollback to back up file but backup file does not exist");
            }

        }
        catch (Exception e)
        {
            Debug.LogError("Something went wrong while trying to rollback to backup " + backupFilePath + " with exception " + e);
        }


        return success;
    }
}
