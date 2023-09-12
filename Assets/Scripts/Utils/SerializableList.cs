using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    /**<summary>
     * Serializable List that can serialize
     </summary>*/
    [Serializable]
    public class SerializableList<T> : List<T>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<T> values = new List<T>();

        // save the dictionary to lists
        public void OnBeforeSerialize()
        {
            values.Clear();
            foreach (T item in this)
            {
                values.Add(item);
            }
        }

        // load dictionary from lists
        public void OnAfterDeserialize()
        {
            Clear();
            for (int i = 0; i < values.Count; i++) Add(values[i]);
        }

    }
}