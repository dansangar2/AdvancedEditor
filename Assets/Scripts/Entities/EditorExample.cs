using System;
using System.Collections.Generic;
using Entities.BaseEntities;
using UnityEngine;
using Utils;
using Windows.Core;
using Entities.Enums;
using System.Linq;

namespace Entities
{
    
    /**<summary>Base parameters for all entities. The parameters in common.</summary>*/
    [Serializable]
    public class EditorExample : BaseEntity
    {
        #region ATTRIBUTES
        
        [SerializeField] protected int intExample;
        [SerializeField] protected string stringExample = "";
        [SerializeField] protected string areaExample = "";
        [SerializeField] protected float floatExample; 
        [SerializeField] protected bool boolExample;
        [SerializeField] protected EnumExample enumExample;
        [SerializeField] protected int selectionExample;
        [SerializeField] protected string[] simpleList = new string[0];
        [SerializeField] protected int[] dataList = new int[0];  
        [SerializeField] protected EnumExample[] enumList = new EnumExample[0];
        [SerializeField] protected SerializableDictionary<string, string> simpleDictionary = new(); 
        [SerializeField] protected SerializableDictionary<int, EnumExample> dataDictionary = new(); 
        [SerializeField] protected SerializableDictionary<EnumExample, int> enumDictionary = new(); 
        [SerializeField] protected SerializableDictionary<int, int> doubleDataDictionary = new(); 
        protected static string[] listDataElements = new []{"Option1", "Option2", "Option3"};
        protected static string[] listDataElements2 = new []{"NOption1", "NOption2", "NOption3"};
        
        public new EditorParams Editor => new(
            new[]{"IntExample", "StringExample", "AreaExample", "FloatExample",
                "BoolExample", "EnumExample", "SelectionExample", "SimpleList", "DataList", "EnumList",
                "SimpleDictionary", "DataDictionary", "EnumDictionary", "DoubleDataDictionary"
                },
            new()
            {
                {"SimpleList", new []{AdvancedEditor.Restrictions.ListAddingByEntry }},
                {"SelectionExample", new object[]{EditorParams.SetListObjects(listDataElements)}},
                {"AreaExample", new []{AdvancedEditor.Restrictions.Area}},
                {"DataList", new object[]{EditorParams.SetListObjects(listDataElements), AdvancedEditor.Restrictions.CanNotRepeat, AdvancedEditor.Restrictions.ListAddingByEntry}},
                {"DataDictionary", new []{EditorParams.SetListObjects(listDataElements)}},
                {"EnumDictionary", new []{EditorParams.SetListObjects(listDataElements2), EditorParams.SetListLimits(0,2)}},
                {"DoubleDataDictionary", new []{EditorParams.SetDictionaryObjects(listDataElements, listDataElements2)}}
            },
            new(){}, 1, extensionOf:base.Editor);

        #endregion
        
        #region CONSTRUCTORS

        public EditorExample(){}
        
        /**<summary>Empty constructor.</summary>*/ 
        public EditorExample(int id) : base(id) { }

        /**<summary>Clone constructor.</summary>*/ 
        public EditorExample(EditorExample editorExample) : base(editorExample)
        {
            intExample = editorExample.intExample;
            stringExample = editorExample.stringExample;
            floatExample = editorExample.floatExample;
            areaExample = editorExample.areaExample;
            boolExample = editorExample.boolExample;
            enumExample = editorExample.enumExample;
            selectionExample = editorExample.selectionExample;
            SimpleList = editorExample.simpleList.ToArray();
            DataList = editorExample.dataList.ToArray();
            EnumList = editorExample.enumList.ToArray();
            simpleDictionary.Copy(editorExample.simpleDictionary);
            dataDictionary.Copy(editorExample.dataDictionary);
            enumDictionary.Copy(editorExample.enumDictionary);
            doubleDataDictionary.Copy(editorExample.doubleDataDictionary);
        }
        
        #endregion
        
        #region GETTERS AND SETTERS
            
        public int IntExample { get => intExample; set => intExample = value; }
        public string StringExample { get => stringExample; set => stringExample = value; }
        public string AreaExample { get => areaExample; set => areaExample = value; }
        public float FloatExample { get => floatExample; set => floatExample = value; } 
        public bool BoolExample { get => boolExample; set => boolExample = value; }
        public EnumExample EnumExample { get => enumExample; set => enumExample = value; }
        public int SelectionExample { get => selectionExample; set => selectionExample = value; }
        public string[] SimpleList { get => simpleList; set => simpleList = value; }
        public int[] DataList { get => dataList; set => dataList = value; }
        public EnumExample[] EnumList { get => enumList; set => enumList = value; }
        public SerializableDictionary<string, string> SimpleDictionary { get => simpleDictionary; set => simpleDictionary = value; }
        public SerializableDictionary<int, EnumExample> DataDictionary { get => dataDictionary; set => dataDictionary = value; }
        public SerializableDictionary<EnumExample, int> EnumDictionary { get => enumDictionary; set => enumDictionary = value; } 
        public SerializableDictionary<int, int> DoubleDataDictionary { get => doubleDataDictionary; set => doubleDataDictionary = value; } 

        #endregion
        
        public override string ToString() 
        { 
            return "(#ID:" + id + ") " + GetType() + ": " + Name; 
        }
    
    }
}
