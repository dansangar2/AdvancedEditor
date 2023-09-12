using System;
using Windows.Core;
using Utils;
using UnityEngine;

namespace Entities.BaseEntities
{
    
    /**<summary>Base parameters for all entities. The parameters in common.</summary>*/
    [Serializable]
    public abstract class BaseEntity
    {
        #region ATTRIBUTES
        
        [SerializeField] protected int id;
        [SerializeField] protected string name = ""; 
        public EditorParams Editor => new(
            new[]{"ID", "Name"},
            new(){{"ID", new []{AdvancedEditor.Restrictions.ReadOnly}}},
            new(){}, 1);

        #endregion
        
        #region CONSTRUCTORS

        public BaseEntity(){}
        
        /**<summary>Empty constructor.</summary>*/ 
        protected BaseEntity(int id) => this.id = id;

        /**<summary>Clone constructor.</summary>*/ 
        protected BaseEntity(BaseEntity baseEntity)
        {
            id = baseEntity.id;
            name = baseEntity.name; 
        }
        
        #endregion
        
        #region GETTERS AND SETTERS
            
        /**<summary>The ID of the object.</summary>*/
        public int ID { get => id; set => id = value; }
        
        /**<summary>The name that will show in the game.</summary>*/
        public string Name { get => name; set => name = value; } 

        #endregion
        
        public override string ToString() 
        { 
            return "(#ID:" + id + ") " + GetType() + ": " + Name; 
        }
    
    }
}
