using System;
using Data.Database.Core;
using Entities.BaseEntities;
using UnityEditor;
using UnityEngine;

namespace Windows.Core
{
    /**<summary>System that generates a new window to create.</summary>*/
    public class Window<T1, T2> : EditorWindow where T1 : BaseEntity where T2 : CoreDB<T1>
    {
        
        /**<summary>The new window to generate.</summary>*/
        public static EditorWindow _window;

        /**<summary>Empty item.</summary>*/
        private static T1 _obj;
        /**<summary>Item DB.</summary>*/
        private static T2 _db;
        /**<summary>Action of the Window.</summary>*/
        private static WindowAction _action = WindowAction.Create;
        
        
        /**<summary>Open Creator Window.</summary>*/
        public static void Create(T2 db) => NewWindow(WindowAction.Create, db:db);

        /**<summary>Open Show Window.</summary>*/
        public static void Show(T1 obj) => NewWindow(WindowAction.Show, obj);

        /**<summary>Open Modifier Window.</summary>*/
        public static void Modify(T2 db, T1 obj) => NewWindow(WindowAction.Modify, obj, db);

        
        
        /**<summary>Open the new window.</summary>*/
        private static void NewWindow(WindowAction action, T1 obj = null, T2 db = null) {
            
            //Set data
            _db = db;
            _action = action;
            _obj = (T1)Activator.CreateInstance(typeof(T1), _action == WindowAction.Create ? _db.Count : obj);
            WindowInterface.SetData(action, _obj, _db);
            _window = GetWindow<WindowInterface>();
            //Show the window data
            Display<T1>.Window(_window);

        }
    }
}