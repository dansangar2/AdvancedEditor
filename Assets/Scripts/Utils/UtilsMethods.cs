using System;
using System.Linq;
using UnityEngine;

namespace Utils
{
    public static class UtilsMethods
    {

        public static void AddToArray<T>(ref T[] array, T item)
        {
            Array.Resize(ref array, array.Length+1);
            array[^1] = item;
        }
        
        public static void RemoveFromArray<T>(ref T[] array, T item)
        {
            for (int i = Array.IndexOf(array, item); i < array.Length-1; i++) array[i] = array[i+1];
            Array.Resize(ref array, array.Length - 1); 
        }
        
        public static void ArrayCopy<T>(ref T[] values, T[] toCopy)
        {
            Array.Resize(ref values, toCopy.Length);
                
            for (int i = 0; i < toCopy.Length; i++)  values[i] = toCopy[i];

        }
        
        public static void DictionaryCopy<T, TU>(ref SerializableDictionary<T, TU> values, SerializableDictionary<T, TU> toCopy)
        {
            values = new SerializableDictionary<T, TU>();
            for (int i = 0; i < toCopy.Count; i++)  values.Add(toCopy.Keys.ToArray()[i], toCopy.Values.ToArray()[i]);

        }
        
        public static void ArrayCopyWithMax<T>(ref T[] values, T[] toCopy)
        {
            int size = Mathf.Max(values.Length, toCopy.Length);
            Array.Resize(ref values, size);
            Array.Resize(ref toCopy, size);
                
            for (int i = 0; i < size; i++)  values[i] = toCopy[i];

        }
        
        public static void ArrayCopyWithMin<T>(ref T[] values, T[] toCopy)
        {
            values ??= new T[] { };
            toCopy ??= new T[] { };
            for (int i = 0; i < Mathf.Min(values.Length, toCopy.Length); i++)  values[i] = toCopy[i];

        }
        
        public static void DictionaryCopyWithMin<T, TU>(ref SerializableDictionary<T, TU> values, SerializableDictionary<T, TU> toCopy)
        {
            values ??= new SerializableDictionary<T, TU>();
            toCopy ??= new SerializableDictionary<T, TU>();
                
            for (int i = 0; i < Mathf.Min(values.Count, toCopy.Count); i++)  values.Add(toCopy.Keys.ToArray()[i], toCopy.Values.ToArray()[i]);

        }
    }
}