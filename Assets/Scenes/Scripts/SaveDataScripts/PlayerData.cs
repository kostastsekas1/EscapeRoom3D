using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


[System.Serializable]
public class PlayerData
{
    public long lastUpdated;
    public float[] playerposition;
    public string saveDate;
    public int puzzlesSolved;
    public SerialisableDictionary<string, bool>  LocksUnclocked;
    public PlayerData()
    {
       

        playerposition= new float[3];

        playerposition[0] = -67.20633f;
        playerposition[1] = 1.936f;
        playerposition[2] = -39.49312f;

        puzzlesSolved = 0;
        saveDate = "";
        LocksUnclocked= new SerialisableDictionary<string, bool>();

    }
    
    public  int getPercentageOfRiddlesCompleted()
    {
        int totalRiddlesSolved = 0;

        foreach(bool unlocked in LocksUnclocked.Values) 
        {
            if (unlocked)
            {
                totalRiddlesSolved++;
            }
        }

        int percentagecomplete = -1;
        if(LocksUnclocked.Count !=0 )
        {
            percentagecomplete = (totalRiddlesSolved * 100 / LocksUnclocked.Count);
        }
      

        return percentagecomplete;
    }

   
}
