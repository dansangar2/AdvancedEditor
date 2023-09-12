using Data.Database.Core;
using Entities.BaseEntities;
using UnityEditor;
using UnityEngine;

namespace Windows.Core
{
    public class ListWindow<T1, T2>: Editor where T1 : BaseEntity where T2 : CoreDB<T1>
    {
        /**<summary>Item DB.</summary>*/
        private T2 db;
        /**<summary>Item to search by ID or name.</summary>*/
        private string searchByName = "";
        /**<summary>Check if _searchByName isn't null or empty.</summary>*/
        private bool canSearch;
        /**<summary>Item to delete.</summary>*/
        private T1 deleted;
        private string _entityName = typeof(T1).Name;

        /**<summary>Initialize the DB when the file is clicked.</summary>*/
        private void OnEnable()
        {
            db = (T2)target;
        }

        #region INSPECTOR

        /**<summary>List all items of the DB, with the filter and creator.</summary>*/
        public override void OnInspectorGUI()
        {
            if (!db) return;
            
            #region Total

            EditorGUILayout.BeginHorizontal("Box");
            GUILayout.Label(_entityName + " created: " + db.Count);
            EditorGUILayout.EndHorizontal();

            #endregion
            
            #region Search

            if (db.Count != 0)
            {
                EditorGUILayout.BeginHorizontal("Box"); 
                GUILayout.Label("Search: ");
                searchByName = GUILayout.TextField(searchByName); 
                EditorGUILayout.EndHorizontal();
            }

            #endregion

            #region Manage

            if (GUILayout.Button("Add " + _entityName))
            {
                Window<T1, T2>.Create(db);
            }
            canSearch = !string.IsNullOrEmpty(searchByName);
            
            foreach (T1 element in db.All)
            {
                if (canSearch)
                {
                    if (element.Name == searchByName || element.Name.Contains(searchByName) || element.ID.ToString() == searchByName)
                    {
                        Display(element);
                    }
                }
                else Display(element);

                if (deleted != null)
                {
                    db.Remove(deleted.ID);
                        
                }
            }

            #endregion
        }

        #endregion
        
        #region DISPLAY

        /**<summary>Show an item, with some data with buttons to delete, show and modify.</summary>*/
        private void Display(T1 obj)
        {

            EditorGUILayout.BeginVertical("Box");

            AdvancedEditor.Editor(obj.ID, "ID", true, listInterface:true);
            AdvancedEditor.Editor(obj.Name, "Name", true, listInterface:true);

            EditorGUILayout.BeginHorizontal();
            
            #region Show
            
            if (GUILayout.Button("Show"))
            {
                Window<T1, T2>.Show(obj);
            }

            #endregion

            #region Modify

            if (GUILayout.Button("Modify"))
            {
                Window<T1, T2>.Modify(db, obj);
            }

            #endregion

            #region Delete

            deleted = GUILayout.Button("Delete") ? obj : null;            

            #endregion
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            
        }
        
        #endregion
    }
}