using System;
using System.Linq;
using Entities.BaseEntities;
using UnityEngine;

namespace Data.Database.Core
{
    /**<summary>Core of all database system.</summary>*/
    public class CoreDB<T> : ScriptableObject where T : BaseEntity
    {
        
        /**<summary>Array that save the Serialize data.</summary>*/
        [SerializeField] protected T[] objects = {};

        /**<summary>Clone.</summary>*/ 
        public void Clone(CoreDB<T> db)
        {
            objects = new T[db.Count];
            for (int i = 0; i < db.Count; i++)
            {
                objects[i] = db.All[i];
            }
        }
        
        #region GET
        
        /**<summary>Get the number of items.</summary>*/ 
        public int Count => objects.Length;
        
        /**<summary>Get All data in the array.</summary>*/ 
        public T[] All => objects;
     
        /**<summary>Get All Names of the items of the array.</summary>*/ 
        public string[] Names 
        { 
            get 
            { 
                string[] res = new string[objects.Length]; 
                for (int i = 0; i < objects.Length; i++) { res[i] = objects[i].Name; } 
                return res;
            }
        }
        
        #endregion
        
        #region FINDBY
        
        /**<summary>Find item by ID.</summary>*/ 
        public T FindByID(int id) 
        { 
            return objects.FirstOrDefault(element => element.ID == id);
        }
        
        /**<summary>Find item by name.</summary>*/ 
        public T FindByName(string nameOf) 
        { 
            return objects.FirstOrDefault(element => element.Name == nameOf);
        }

        public T this[int id]
        {
            get => FindByID(id);
            set 
            { 
                value.ID = id; 
                Modify(value);    
            }
        }
        
        #endregion
        
        #region MANAGE
        
        /**<summary>Add a item to the array.</summary>*/ 
        public void Add(T newValue) 
        { 
            Array.Resize(ref objects, Count + 1); 
            objects[Count - 1] = newValue;
        }
        
        /**<summary>Modify a item of the array. It's modify by the item ID.</summary>*/ 
        public void Modify(T newValue) 
        { 
            objects[Array.IndexOf(objects, FindByID(newValue.ID))] = newValue;
        }
        
        /**<summary>Remove a item of the array</summary>*/ 
        public void Remove(int id) 
        { 
            for (int i = Array.IndexOf(objects, FindByID(id)); i < Count-1; i++) 
            { 
                objects[i] = objects[i+1]; 
                objects[i].ID--; 
            } 
            Array.Resize(ref objects, Count - 1); 
        }
        
        #endregion
        
    }
}