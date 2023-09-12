using System;
using System.Collections.Generic;
using System.Linq;

namespace Utils
{
    /**<summary>Configuration for Unity editor for Entities</summary>*/ 
    public class EditorParams
    {
        
        #region ATTRIBUTES
        
        private string[] _parametters = new string[0];
        private Dictionary<string, object[]> _configurations = new();
        private Dictionary<string, string> _labels = new();
        private int _columns = 0;
        private int[][] _sizes = new int[][]{new int[]{1100, 700}, new int[]{1000, 650}};
           
        #endregion
        
        #region CONSTRUCTORS
        
        /**<summary>Empty constructor.</summary>*/ 
        public EditorParams(string[] parametters, 
                        Dictionary<string, object[]> configurations = null, 
                        Dictionary<string, string> labels = null, int columns = 1, 
                        int maxV = 1100, int maxH = 700, int minV = 1000, int minH = 650, 
                        EditorParams extensionOf = null)
        { 
            _configurations ??= new Dictionary<string, object[]>();
            labels ??= new Dictionary<string, string>();
            _sizes = new int[][]{new int[]{maxV, maxH}, new int[]{minV, minH}};

            _parametters = parametters.Clone() as string[];
            _configurations = new Dictionary<string, object[]>(configurations);

            if(extensionOf != null)
            {
                JoinEditorParams(extensionOf.Parametters);
                JoinEditorConfigurations(extensionOf.Configurations);
            }
            _columns = columns;
            _labels = new Dictionary<string, string>(labels);
        }
        
        #endregion
        
        #region GETTERS & SETTERS
        
        /**<summary>Params that you can edit in editor _windows.</summary>*/ 
        public string[] Parametters => _parametters;

        /**<summary>Restrictions and views _configurations for editorfields
        <para>"readonly" => only show the data.</para>
        "area" => if use space in the editor, it will be a area and not simple slot.</summary>*/ 
        public Dictionary<string, object[]> Configurations => _configurations;

        /**<summary>How the attribute will show in the label. If a param haven't any "label" it'll use the name of attribute in "_windows".</summary>*/ 
        public Dictionary<string, string> Labels => _labels; 

        /**<summary>Slots by row. This will be ignored for special options like "area" or
        some type of attributes</summary>*/ 
        public int Columns => _columns;

        /**<summary>Sizes of Windows editor</summary>*/ 
        public int[][] Sizes => _sizes;

        /**<summary>Name of "Editor" attribute of Entity.</summary>*/
        public readonly static string Editor = "Editor";
        /**<summary>Name of "Sizes" attribute.</summary>*/
        public readonly static string Size = "Sizes";
        /**<summary>Name of "Configurations" attribute.</summary>*/
        public readonly static string Configuration = "Configurations";
        /**<summary>Name of "Labels" attribute.</summary>*/
        public readonly static string Label = "Labels";
        /**<summary>Name of "Windows" attribute.</summary>*/
        public readonly static string Parametter = "Parametters";
        /**<summary>Name of "Column" attribute.</summary>*/
        public readonly static string Column = "Columns";
        /**<summary>Name of "Column" attribute.</summary>*/
        //public static string Column => "Columns";
          
        #endregion
        
        #region METHODS
        
        private void JoinEditorParams(string[] toAdd = null) => _parametters = toAdd?.Concat(_parametters ?? (new string[0])).ToArray();

        private void JoinEditorConfigurations(Dictionary<string, object[]> toAdd = null) => _configurations = toAdd?.ToLookup(k => k.Key)
                        .Concat((_configurations ?? (new Dictionary<string, object[]>())).ToLookup(k => k.Key))
                        .GroupBy(k => k.Key, k => k.Select(v => v.Value))
                        .ToDictionary(k => k.Key, k => k.SelectMany(v => v.SelectMany(x => x)).Distinct().ToArray());
        


        public enum ParamAction
        {
            ListObjects,
            ListLimits,
        }
        
        /**<summary>Set the limits size of list.</summary>*/
        public static Tuple<ParamAction, object> SetListLimits(int min, int max) => new(ParamAction.ListLimits, new int[]{min, max});

        /**<summary>Set the items that can choose. 
        En case of use Dictionaries, both (key, value) will use the items of array, just if it's int value.</summary>*/
        //public static Tuple<ParamAction, object> SetListObjects<T>(Dictionary<string, T> objs) => new(ParamAction.ListObjects, objs.Values.ToArray());

        public static Tuple<ParamAction, object> SetListObjects(object[] objs) => new(ParamAction.ListObjects, objs);

        /**<summary>Set the items that can choose. In Keys and Values. Null means that haven't 
        options and use normal int.</summary>*/
        public static Tuple<ParamAction, object> SetDictionaryObjects(object[] objsKeys, object[] objsValues) => new(ParamAction.ListObjects, new object[][]{objsKeys, objsValues});
        
        #endregion
        
    }
}