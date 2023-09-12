using System;
using Data.Database.Core;
using Entities.BaseEntities;
using UnityEditor;
using UnityEngine;

namespace Windows.Core
{
    /**<summary>System that generates a new window to create.</summary>*/
    public class WindowInterface : EditorWindow 
    {
        
        /**<summary>The new window to open.</summary>*/
        public static EditorWindow _window;

        public static Type _dbType;
        public static Type _entityType;

        /**<summary>Save the current item.</summary>*/
        private static object _obj;
        /**<summary>Save the current item DB.</summary>*/
        private static object _db;
        /**<summary>Type of action for manage items (create for example).</summary>*/
        private static WindowAction _action = WindowAction.Create;
        
        /**<summary>Scroll for move the DB items list.</summary>*/
        private Vector2 scroll;
        
        

        /**<summary>Show the window.</summary>*/
        public void OnGUI()
        {
            
            scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.MinHeight(5), GUILayout.MinHeight(position.height));
            EditorGUILayout.BeginVertical("Box");
            
            _obj = Convert.ChangeType(_obj, _entityType);
            typeof(Display<>).MakeGenericType(_entityType).GetMethod("Displayed")
                .Invoke(null, new object[]{_obj, _action == WindowAction.Show});
                
            if (GUILayout.Button("Confirm")) 
            { 
                if(_action!= WindowAction.Show) Action();
                AdvancedEditor.ClearAllKeys();
                this.Close();
            }
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
            if(_action != WindowAction.Show) EditorGUI.EndDisabledGroup();
            
        }

        public static void SetData<T1, T2>(WindowAction action, T1 entity, T2 db) where T1: BaseEntity where T2 : CoreDB<T1>
        {
            _action = action;
            _db = db;
            _obj = entity;
            _dbType = typeof(T2);
            _entityType = typeof(T1);
        }

        /**<summary>Do action of create or modify.</summary>*/
        private static void Action()
        {
            Undo.RecordObject((UnityEngine.Object)_db, _db.GetType().Name);
            switch(_action) {
                case WindowAction.Create:
                    _db.GetType().GetMethod("Add").Invoke(_db, new object[]{_obj});
                    break;
                case WindowAction.Modify:
                    _db.GetType().GetMethod("Modify").Invoke(_db, new object[]{_obj});
                    break;
            }
        
            EditorUtility.SetDirty((UnityEngine.Object)_db);
        }
        
    }
}