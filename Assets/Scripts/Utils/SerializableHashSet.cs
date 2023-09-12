using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    /**<summary>
     * Serializable HashSet
     </summary>*/
    [Serializable]
    public class SerializableHashSet<T> : HashSet<T>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<T> values = new List<T>();

        // save the HashSet to lists
        public void OnBeforeSerialize()
        {
            HashSet<T> contains = new HashSet<T>(values);

            foreach (T value in this)
            {
                if (!contains.Contains(value))
                {
                    values.Add(value);
                }
            }
        }

        // load dictionary from lists
        public void OnAfterDeserialize()
        {
            this.Clear();

            foreach (T value in values) this.Add(value);
        }

    }
}