using Data.Database;
using Entities;
using UnityEditor;
using Windows.Core;

namespace Windows
{
    /**<summary>System that list the DB items.</summary>*/
    [CustomEditor(typeof(EditorExamples))]
    public class EEList : ListWindow<EditorExample, EditorExamples> { }
}