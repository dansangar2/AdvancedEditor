using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utils
{
    /**<summary>
     * Serializable Dictionary that can serialize
     </summary>*/
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<TKey> keys = new List<TKey>();

        [SerializeField] private List<TValue> values = new List<TValue>();

        // save the dictionary to lists
        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();
            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }

        // load dictionary from lists
        public void OnAfterDeserialize()
        {
            Clear();

            if (keys.Count != values.Count)
                throw new Exception(
                    $"there are {keys.Count} keys and {values.Count} values after deserialization. Make sure that both key and value types are serializable.");

            for (int i = 0; i < keys.Count; i++)
                Add(keys[i], values[i]);
        }

        public void ReplaceKey(TKey oldK, TKey newK)
        {
            OnBeforeSerialize();
            if(!keys.Contains(oldK)) throw new KeyNotFoundException("Key " + oldK + " not found");
            keys = keys.Select(v => v.Equals(oldK) ? newK : v).ToList();
            OnAfterDeserialize();
            
        }

        public static SerializableDictionary<TKey, TValue> CopyDictionary(Dictionary<TKey, TValue> dictionary)
        {
            SerializableDictionary<TKey, TValue> d = new ();

            foreach(KeyValuePair<TKey, TValue> kvp in dictionary)
            {
                d.Add(kvp.Key, kvp.Value);
            }
            
            return d;
        }

        public SerializableDictionary<TKey, TValue> Copy(Dictionary<TKey, TValue> dictionary)
        {
            SerializableDictionary<TKey, TValue> d = new ();

            foreach(KeyValuePair<TKey, TValue> kvp in dictionary)
            {
                d.Add(kvp.Key, kvp.Value);
            }
            
            return d;
        }
        
        public void Copy(SerializableDictionary<TKey, TValue> dictionary)
        {
            foreach(KeyValuePair<TKey, TValue> kvp in dictionary)
            {
                Add(kvp.Key, kvp.Value);
            }
        }

        public Dictionary<TKey, TValue> Dictionary()
        {
            Dictionary<TKey, TValue> d = new ();

            for(int i=0; i<keys.Count;i++)
            {
                d.Add(keys[i], values[i]);
            }
            
            return d;
        }

    }
}