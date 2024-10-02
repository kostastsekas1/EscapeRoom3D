using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SerialisableDictionary <Tkey,TValue> : Dictionary<Tkey, TValue> , ISerializationCallbackReceiver
{
    [SerializeField] private List<Tkey> keys = new List<Tkey>();
    [SerializeField] private List<TValue> values = new List<TValue>();

    public void OnAfterDeserialize()
    {
        keys.Clear();
        values.Clear(); 
        foreach( KeyValuePair<Tkey,TValue> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    public void OnBeforeSerialize()
    {
        this.Clear();

        if(keys.Count != values.Count)
        {
           Debug.LogError("Tried to serialise dictionary for locks but the keys and the values are not of same size");
        }



        for ( int  i = 0; i < keys.Count; i++)
        {
            this.Add(keys[i], values[i]);
        }
    }
}
