using System;
using System.Linq;
using System.Reflection;
using Entities.BaseEntities;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Utils;

namespace Windows.Core
{
    /**<summary>Windows for edit and/or show the data of objects.</summary>*/
    public static class Display<T> where T : BaseEntity
    {
        /**<summary>Save the Type of the object.</summary>*/
        private static Type _type;
        /**<summary>Reference to property "editor".</summary>*/
        private static PropertyInfo _propinfo;
        /**<summary>Save the configuration of the windows.</summary>*/
        private static object _editorPrefs;
        /**<summary>Max sizes of the windows.</summary>*/
        private static int[] _max = new int[]{1100, 700};
        /**<summary>Min sizes of the windows.</summary>*/
        private static int[] _min = new int[]{1000, 650}; 
        /**<summary>Editor reference object.</summary>*/
        private static EditorWindow _window;
        
        /**<summary>Set the general data for make and display the windows.</summary>*/
        public static void Window(EditorWindow window) 
        {
            _type = typeof(T);
            _window = window;
            _editorPrefs = null;
            AdvancedEditor.ClearAllKeys();
        }
        
        /**<summary>Generate the window interface, adapted to the entity. With the data of the item.</summary>*/
        public static void Displayed(T obj, bool readOnly = false)
        {
            
            
            #region Windows size settings.
            if(_propinfo==null)
            {
                _propinfo = _type.GetProperty(EditorParams.Editor);
                _editorPrefs = _propinfo.GetValue(obj);
                if(_editorPrefs != null)
                {
                    int[][] s = (int[][])_editorPrefs.GetType().GetProperty(EditorParams.Size).GetValue(_editorPrefs);
                    if(s!=null)
                    {
                        _max = s[0];
                        _min = s[1];
                    }
                }
                _window.maxSize = new Vector2(_max[0], _max[1]);
                _window.minSize = new Vector2(_min[0], _min[1]);
            }
            else _editorPrefs = _propinfo.GetValue(obj);
            
            #endregion
            
            #region Get Editor configuration.
            Type t = typeof(T);
            string[] attributes = (string[])_editorPrefs.GetType().GetProperty(EditorParams.Parametter)?.GetValue(_editorPrefs);
            Dictionary<string, string> labels = (Dictionary<string, string>)_editorPrefs.GetType().GetProperty(EditorParams.Label)?.GetValue(_editorPrefs);
            Dictionary<string, object[]> options = (Dictionary<string, object[]>)_editorPrefs.GetType().GetProperty(EditorParams.Configuration)?.GetValue(_editorPrefs);
            int numberOfColumns = (int)_editorPrefs.GetType().GetProperty(EditorParams.Column)?.GetValue(_editorPrefs);
            #endregion

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical("Box");

            foreach(string s in attributes)
            {
                for(int i = 0; i<numberOfColumns; i++) {
                    PropertyInfo pinfo = t.GetProperty(s);
                    
                    //Add readonly attribute if the window is to show the data.
                    if (readOnly) 
                    {
                        if(options.ContainsKey(s)) 
                        {
                            if(!options[s].Contains("readonly"))
                            {    
                                object[] o = options[s];
                                Array.Resize(ref o, o.Length + 1);
                                o[^1] = "readonly";
                                options[s] = o;                                
                            }
                        } else options.Add(s, new object[]{"readonly"});
                    }
                    
                    object minfo = AdvancedEditor.SetValue(pinfo.GetValue(obj), t+":"+s, labels != null && labels.ContainsKey(s) ? labels[s] : s, 
                                options != null && options.ContainsKey(s) ? options[s] : null);
                    pinfo.SetValue(obj, minfo);
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();            

            return;
        }
    }
}