using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Utils;
using static Utils.EditorParams;
using Object = UnityEngine.Object;

namespace Windows.Core
{
    public static class AdvancedEditor
    {
        #region OPTIONS

        /**<summary>Visual configuration for entering fields values.</summary>*/
        private static readonly GUILayoutOption[] Options = { GUILayout.MaxWidth(150f), GUILayout.MinWidth(20f)};
        
        /**<summary>Visual configuration for images displaying a Sprite field.</summary>*/
        private static readonly GUILayoutOption[] OpIcon = { GUILayout.MaxWidth(150f), GUILayout.MinWidth(20f), GUILayout.Width(50f), GUILayout.Height(50f)};
        
        /**<summary>Visual configuration for string value with area option to enter field.</summary>*/
        private static readonly GUIStyle TextAreaStyle = new GUIStyle(GUI.skin.textArea) {wordWrap = true};
        
        /**<summary>Style for the label of the editor values.</summary>*/
        private static readonly GUIStyle LabelStyle = new GUIStyle(GUI.skin.label) {wordWrap = true, alignment = TextAnchor.MiddleCenter, margin = new RectOffset(0, 50, 0, 0)};
        
        /**<summary>Style for spaces between fields entered in lists.</summary>*/
        private static readonly GUIStyle SpaceStyle = new GUIStyle(GUI.skin.textField) {wordWrap = true, alignment = TextAnchor.MiddleCenter, margin = new RectOffset(0, 50, 0, 0)};
        
        /**<summary>Style for borders on entering fields.</summary>*/
        private static readonly GUIStyle BordersStyle = new GUIStyle(GUI.skin.button) {wordWrap = true, alignment = TextAnchor.MiddleCenter, margin = new RectOffset(0, 50, 0, 0)};
        
        /**<summary>Style for the icons.</summary>*/
        private static readonly GUIStyle SpriteStyle = new GUIStyle(GUI.skin.box) {wordWrap = true, alignment = TextAnchor.MiddleRight, margin = new RectOffset(0, 50, 0, 0)};

        /**<summary>Style for the data in display window.</summary>*/
        private static readonly GUIStyle DataStyle = new GUIStyle(GUI.skin.label) {wordWrap = true, alignment = TextAnchor.MiddleCenter, margin = new RectOffset(0, 50, 0, 0)};

        /**<summary>Keys that save field data to add values to lists.</summary>*/
        private static Dictionary<string, object> Keys = new();

        /**<summary>Add Key values.</summary>*/
        public static void AddKey(string paramName, object paramValue = default) => Keys[paramName] = DesnullTString(paramValue);
    
        /**<summary>Clear all data in Keys array.</summary>*/
        public static void ClearAllKeys() => Keys.Clear();

        /**<summary>Struct with the keywords for restrictions and configuration of the fields.
        Use it in EditorParams Constructor, in the configuration array.</summary>*/
        public struct Restrictions
        {
            /**<summary>For readonly param.</summary>*/
            public static string ReadOnly = "readOnly";
            /**<summary>For area entering string fields.</summary>*/
            public static string Area = "area";
            /**<summary>Unique value in lists.</summary>*/
            public static string CanNotRepeat = "canNotRepeat";
            /**<summary>For special list views in windows.</summary>*/
            public static string ListInterface = "listInterface";
            /**<summary>to make a list size by number of items or adding specifics items (by entries)</summary>*/
            public static string ListAddingByEntry = "byEntry";
        }

        #endregion

        /**<summary>Set value to "obj" and make an interface for the field.</summary>
        <param name="configurations">Configuration for field. Use "AdvancedEditor.Restrictions" for know more</param>*/
        public static T SetValue<T>(T obj, string paramID, string label, object[] configurations = null) {
            configurations ??= new string[0];

            string[] p = configurations.Where(p=>p.GetType() == typeof(string)).Select(p=>p.ToString()).ToArray(); 
            
            //Check if have restrictions and other configurations.
            bool readOnly = p.Contains(Restrictions.ReadOnly, StringComparer.OrdinalIgnoreCase);
            bool area = p.Contains(Restrictions.Area, StringComparer.OrdinalIgnoreCase);
            bool repeat = !p.Contains(Restrictions.CanNotRepeat, StringComparer.OrdinalIgnoreCase);
            bool listInterface = p.Contains(Restrictions.ListInterface, StringComparer.OrdinalIgnoreCase);
            bool listEntry = p.Contains(Restrictions.ListAddingByEntry, StringComparer.OrdinalIgnoreCase);
            
            //Check if list/array or dictionary values are limited by a list of objects.
            Tuple<ParamAction, object>[] list = configurations.Where(s => s is Tuple<ParamAction, object>).Select(s => (Tuple<ParamAction, object>)s).ToArray();
            object objs = null;
            int[] limits = new int[]{99999999, 1};
            if(list!=null && list.Length!=0)
            {
                objs = list.FirstOrDefault(t => t.Item1.GetType() == typeof(ParamAction) && t.Item1==ParamAction.ListObjects)?.Item2;
                int[] l = (int[])list.FirstOrDefault(t => t.Item1.GetType() == typeof(ParamAction) && t.Item1==ParamAction.ListLimits)?.Item2;
                limits = l ?? limits;
            }

            if(typeof(ICollection).IsAssignableFrom(typeof(T)) || obj is Array || obj is IDictionary) 
            {
                
                //Check if it's a Dictionary, else it'll be a array/list
                if (obj is IDictionary)
                {
                    //Get Types of Dictionary
                    Type[] ts = (Type[])typeof(Type).GetMethod("GetGenericArguments").Invoke(typeof(T)
                        .GetMethod("GetType").Invoke(obj, new object[0]), new object[0]);

                    object[][] lists = new object[][] { null, null };
                    if(objs is object[][] v) lists = v;
                    else
                    {
                        if(ts[0] == typeof(int)) lists[0] = objs as object[];
                        if(ts[1] == typeof(int)) lists[1] = objs as object[];
                    }
                    
                    return (T)typeof(AdvancedEditor).GetMethod("Dictionary")
                        .MakeGenericMethod(ts[0], ts[1])
                        .Invoke(null, new object[]{obj, paramID, label, readOnly, lists[0], lists[1], limits[1], limits[0], default, listInterface});
                }
                //Get Type of list/array
                Type t = (Type)typeof(Type).GetMethod("GetElementType").Invoke(typeof(T).GetMethod("GetType").Invoke(obj, new object[0]), new object[0]);
                
                return (T)typeof(AdvancedEditor).GetMethod("List")
                            .MakeGenericMethod(t).Invoke(null, new object[]{obj, label, readOnly, paramID, listEntry, repeat, limits[0], limits[1], 
                            objs, listInterface, default});
            }

            //Basic value interface
            return Field(obj, label, (string[])objs, readOnly, area, listInterface);
        }

        #region BASE TYPES

        public static T Field<T>(T obj, string label = "", string[] listOptions = null, bool readOnly = false, bool area = false, bool listInterface = false)
        {
            if (listOptions!=null && listOptions.Length!=0) 
                return Convert<T, int>(Select(Convert<int, T>(obj), listOptions, label, readOnly, listInterface));
            else return Editor(obj, label, readOnly, area, listInterface);
        }

        /**<summary>Generic editor field generator for basic types like numbers, strings or enums.</summary>*/
        public static T Editor<T>(T obj, string label = "", bool readOnly = false, bool area = false, bool listInterface = false)
        {
            GUILayoutOption[] options = listInterface ? new GUILayoutOption[]{} : Options;
            GUIStyle style = area ? TextAreaStyle : DataStyle;
            GUILayoutOption[] option = area ? new GUILayoutOption[]{GUILayout.MinHeight(100)} : options;
            T res = obj;
            if(!area) EditorGUILayout.BeginHorizontal();
            
            if(!label.Equals("")) GUILayout.Label(label + ": ");
            if(readOnly) GUILayout.Label(obj != null ? obj.ToString() : "Null", style, option);
            else res = Write(obj, area, listInterface);
            
            if(!area) EditorGUILayout.EndHorizontal();

            if (obj is Sprite)
            {
                EditorGUILayout.BeginHorizontal();
                Sprite s = (Sprite)System.Convert.ChangeType(obj, typeof(Sprite));
                if(s) GUILayout.Box(s.texture, SpriteStyle, OpIcon);
                else GUILayout.Box("No icon", SpriteStyle, OpIcon);
                EditorGUILayout.EndHorizontal();
            } 
            return res;
        }

        /**<summary>Check the type of value and make entering field for it.</summary>*/
        private static T Write<T>(T obj, bool area = false, bool list = false) where T : notnull
        {
            GUIStyle defStyle = obj is Enum ? new GUIStyle(GUI.skin.button) : new GUIStyle(GUI.skin.textField);
            GUIStyle optStyle = obj is Enum ? BordersStyle : SpaceStyle;
            
            GUIStyle style = list ? optStyle : defStyle;
            GUILayoutOption[] options = list ? new GUILayoutOption[]{} : Options;

            string typeEditor = area ? "Area" : "Field";

            obj = DesnullTString(obj);

            string method = obj switch
            {
                Enum => "EnumPopup",
                string => "Text" + typeEditor,
                int => "Int" + typeEditor,
                float => "Float" + typeEditor,
                double => "Double" + typeEditor,
                Color => "Color" + typeEditor,
                bool => "Toggle",
                _ => "Object" + typeEditor

            };

            List<object> paramss = new () {obj};
            List<string> mustContains = new (){};

            if (!new List<string>(){"ColorField", "ObjectField", "Toggle"}.Contains(method)) mustContains.Add("style");
            if ("ObjectField".Equals(method)) mustContains.AddRange(new string[]{"objType", "allowSceneObjects"});

            Type typeValidation = obj switch
            {
                Enum => typeof(Enum),
                Object => typeof(Object),
                _ => typeof(T),
            };

            if(mustContains.Contains("style")) paramss.Add(area ? TextAreaStyle : style);
            if(mustContains.Contains("objType")) paramss.AddRange(new object[]{typeof(T), true});
            paramss.Add(area ? new GUILayoutOption[]{GUILayout.MinHeight(100)} : options);
            
            return (T)typeof(EditorGUILayout).GetMethods()
                .First(x => x.Name == method && mustContains
                    .All(s => x.GetParameters()[1..^1].Select(y=>y.Name).Contains(s))
                ).Invoke(null, paramss.ToArray());
        }

        /**<summary>Field for select a item from a list "opts".</summary>*/
        public static int Select(int index, string[] opts, string label = "", bool readOnly = false, 
            bool listInterface = false)
        {
            GUIStyle style = listInterface ? LabelStyle : new GUIStyle(GUI.skin.button);
            GUILayoutOption[] options = listInterface ? new GUILayoutOption[]{} : Options;
            EditorGUILayout.BeginHorizontal();
            if (!label.Equals("")) GUILayout.Label(label + ": ");
            if (readOnly) GUILayout.Label(opts[index], DataStyle, options);
            else index = EditorGUILayout.Popup(index, opts, style, options);
            EditorGUILayout.EndHorizontal();

            return index;
        }

        #region list

        /**<summary>Make a list field system to edit lists param.</summary>
        <param name="listOptions">The names of the options if you want make a selection.
        Can be null if you don't want choose 1 option from this list.</param>
        <param name="listEntry">If you check it true, the values of list will add by values, else by number (size) of objects.</param>*/
        public static T[] List<T>(T[] obj, string label = "", bool readOnly = false, string key = null, bool listEntry = false, 
            bool canRepeatItem = true, int limit = 9999999, int minimum = 1, string[] listOptions = null, 
            bool listInterface = false, params T[] canRepeat)
        {
            if(key!=null && key.Length!=0 && listEntry)
            {
                return ListByEntry(obj, key, label, readOnly, limit, minimum, listOptions, canRepeatItem, listInterface, canRepeat);
            }
            return ListByLenght(obj, label, readOnly, limit, minimum, listOptions, canRepeatItem, listInterface, canRepeat);
        }

        /**<summary>Make a list field by entering value system.</summary>
        <param name="listOptions">The names of the options if you want make a selection.
        Can be null if you don't want choose 1 option from this list.</param>
        <param name="key">Key of dictionary that save the current value in a "adding field".</param>*/
        public static T[] ListByEntry<T>(T[] obj, string key, string label = "", bool readOnly = false, int maximum = 9999999, int minimum = 0, 
            string[] listOptions = null, bool canRepeatItem = true, bool listInterface = false, params T[] itemsThatCanRepeat)
        {
            if (!Keys.ContainsKey(key)) AddKey(key, DesnullTString(default(T)));

            EditorGUILayout.BeginVertical("Box");

            GUILayoutOption[] options = listInterface ? new GUILayoutOption[]{} : Options;

            bool notHaveOptions = listOptions==null || listOptions.Length==0;

            SetSizes<T>(canRepeatItem, listOptions, ref minimum, ref maximum);
            
            if(minimum > obj.Length) Array.Resize(ref obj, minimum);

            EditorGUILayout.BeginHorizontal();
            if (!label.Equals("")) GUILayout.Label(label + ":", options);
            if (!readOnly)
            {
                //if (notHaveOptions) Keys[key] = (T)Editor(Keys[key], listInterface:listInterface);
                //else Keys[key] = Select((int)Keys[key], listOptions.ToArray(), listInterface:listInterface);
                Keys[key] = Field(Keys[key], "", listOptions:listOptions?.ToArray(), listInterface:listInterface);
                if (GUILayout.Button("+", Options) 
                    && maximum > obj.Length 
                    && (!obj.Contains((T)Keys[key]) 
                        || canRepeatItem 
                        || (itemsThatCanRepeat != null && itemsThatCanRepeat.Contains((T)Keys[key]))))
                {
                    Array.Resize(ref obj, obj.Length+1);
                    obj[^1] = (T)Keys[key];
                }
            }
            EditorGUILayout.EndHorizontal();
            
            obj = DisplayList(obj, readOnly, minimum, listOptions, listInterface, canRepeatItem);

            EditorGUILayout.EndVertical();

            return obj;
           
        }

        /**<summary>Make a list field by number of objects system.</summary>
        <param name="listOptions">The names of the options if you want make a selection.
        Can be null if you don't want choose 1 option from this list.</param>*/
        public static T[] ListByLenght<T>(T[] obj, string label = "", bool readOnly = false, int maximum = 9999999, int minimum = 0, 
            string[] listOptions = null, bool canRepeatItem = true, bool listInterface = false, params T[] itemsThatCanRepeat)
        {
            EditorGUILayout.BeginVertical("Box");

            GUIStyle style = listInterface ? SpaceStyle : new GUIStyle(GUI.skin.textArea);
            GUILayoutOption[] options = listInterface ? new GUILayoutOption[]{} : Options;

            SetSizes<T>(canRepeatItem, listOptions, ref minimum, ref maximum);

            int size = obj.Length;

            EditorGUILayout.BeginHorizontal();
            if (!label.Equals("")) GUILayout.Label(label + ": ", options);
            if(readOnly) GUILayout.Label(size.ToString(), DataStyle, options);
            else size = EditorGUILayout.IntField(size, style, options);
            EditorGUILayout.EndHorizontal();
            
            Array.Resize(ref obj, size < minimum ? minimum : size > maximum ? maximum : size);
            
            obj = DisplayList(obj, readOnly, minimum, listOptions, listInterface, canRepeatItem);

            EditorGUILayout.EndVertical();

            return obj;
        }

        #endregion

        #region dictionary

        /**<summary>Make a dictionary field by entering value system.</summary>
        <param name="listOptions1">The names of the key options if you want make a selection.
        Can be null if you don't want choose 1 option from this list.</param>
        <param name="listOptions2">The names of the value options if you want make a selection.
        Can be null if you don't want choose 1 option from this list.</param>
        <param name="key">Key of dictionary that save the current value in a "adding field".</param>*/
        public static Dictionary<T1, T2> Dictionary<T1, T2>(Dictionary<T1, T2> obj, string key, string label = "", 
            bool readOnly = false, string[] listOptions1 = null, string[] listOptions2 = null, int maximum = 9999999, int minimum = 0, 
            T2 defaultValue = default, bool listInterface = false)
        {
            if (!Keys.ContainsKey(key)) AddKey(key, default(T1));

            string[] keys = null;
            string[] values = null;

            SetSizesAndKeys<T1>(listOptions1, ref minimum, ref maximum, ref keys);
            SetSizesAndKeys<T1>(listOptions2, ref minimum, ref maximum, ref values);

            EditorGUILayout.BeginHorizontal();
            if(label!=null && !label.Equals("")) GUILayout.Label(label + ": " + obj.Count);
            EditorGUILayout.EndHorizontal();

            if (!readOnly)
            {
                EditorGUILayout.BeginHorizontal();
                //if(notHaveOptions) Keys[key] = Editor(DesnullTString((T1)Keys[key]), listInterface:listInterface);
                //else Keys[key] = Select((int)Keys[key], keys, listInterface:listInterface);
                Keys[key] = Field(DesnullTString((T1)Keys[key]), "", listOptions:keys, listInterface:listInterface);

                if (GUILayout.Button("+", Options) && obj.Count < maximum 
                && !obj.ContainsKey(Convert<T1, object>(Keys[key]))) obj.Add(Convert<T1, object>(Keys[key]), defaultValue);
                EditorGUILayout.EndHorizontal();
            }

            T1[] objs = obj.Keys.ToArray();
            int i = 0;
            foreach (T1 o in objs) 
            {
                T1 change = o;
                EditorGUILayout.BeginHorizontal(); 
                
                /*if (notHaveOptions) change = Editor(obj.Keys.ToArray()[i], readOnly:readOnly, listInterface:listInterface);
                else change = Convert<T1, int>(Select(Convert<int, T1>(objs[i>=objs.Length ? objs.Length-1 : i]), 
                    listOptions, readOnly:readOnly, listInterface:listInterface));*/
                change = Field(o, "", listOptions1, readOnly, false, listInterface);

                if (!o.Equals(change) && !obj.ContainsKey(change))
                {
                    if (obj is SerializableDictionary<T1, T2> dictionary) dictionary.ReplaceKey(o, change);
                    else obj = obj.ToDictionary(k => k.Key.Equals(o) ? change : k.Key, v => v.Value);
                }
                    
                obj[change] = Field(obj[change], "", listOptions2, readOnly, false, listInterface);
                if (!readOnly && GUILayout.Button("-", Options) && obj.Count>minimum) obj.Remove(o);
                EditorGUILayout.EndHorizontal();
                i++;
            }
            
            return obj;
        }

        #endregion
        
        #endregion


        #region UTILS

        private static T DesnullTString<T>(T s) => typeof(T) == typeof(string) && s == null? Convert<T, string>("") : s;  
        
        /**<summary>Convert TU value in type T.</summary>*/
        private static T1 Convert<T1, T2>(T2 value) => (T1)System.Convert.ChangeType(value, typeof(T1));

        private static void SetSizes<T>(bool canRepeatItem, 
            string[] listOptions, ref int min, ref int max)
        {
            bool notHaveOptions = listOptions==null || listOptions.Length==0;
            if(!canRepeatItem)
            {
                if(notHaveOptions) 
                {
                    max = SetMinMax(max, typeof(T)); 
                    min = SetMinMax(min, typeof(T));
                }
                else
                {   
                    max = SetMinMaxWithOptions(max, listOptions);
                    min = SetMinMaxWithOptions(min, listOptions);
                }
            }
        }

        /**<summary>Set the maximum and minimum sizes for list/Dictionary fields</param>*/
        private static int SetMinMax(int value, Type t) => 
            t.IsEnum ? value > Enum.GetValues(t).Length ? Enum.GetValues(t).Length : value : value;

        /**<summary>Set the maximum and minimum sizes for list/Dictionary fields</param>*/
        private static int SetMinMaxWithOptions(int value, string[] listOptions) => 
            value > listOptions.Length ? listOptions.Length : value;

        private static T[] DisplayList<T>(T[] obj, bool readOnly, int minimum, 
            string[] listOptions, bool listInterface, bool canRepeatItem)
        {
            for (int i = 0; i < obj.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                if(!readOnly)
                {
                    T change;

                    //if (notHaveOptions) change = Editor(obj[i], listInterface:listInterface);
                    //else change = Convert<T, int>(Select(Convert<int, T>(obj[i]), listOptions, listInterface:listInterface));
                    change = Field(obj[i], "", listOptions:listOptions, listInterface:listInterface);
                    if (!obj.Contains(change) || canRepeatItem) obj[i] = change;
                    
                    if (GUILayout.Button("-", Options) && minimum < obj.Length)
                    {
                        for (int j = i; j < obj.Length-1; j++)
                        {
                            obj[j] = obj[j + 1];
                        }
            
                        Array.Resize(ref obj, obj.Length-1);
                    }

                }
                else GUILayout.Label(obj[i].ToString());
                EditorGUILayout.EndHorizontal();
            }
            return obj;
        }
        
        private static void SetSizesAndKeys<T>(string[] listOptions, ref int min, ref int max, ref string[] keys)
        {
            bool notHaveOptions = listOptions==null || listOptions.Length==0;

            if(notHaveOptions) keys = typeof(T) == typeof(Enum) ? 
                Enum.GetValues(typeof(T)).Cast<T>().Select(s=>s.ToString()).ToArray() : new string[0];
            else keys = listOptions.ToArray();
        
            SetSizes<T>(true, listOptions, ref min, ref max);
        }

        #endregion
    }
}