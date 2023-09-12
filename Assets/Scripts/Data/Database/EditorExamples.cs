using Data.Database.Core;
using Entities;
using UnityEngine;

namespace Data.Database
{
    /**<summary>The Editor Examples Database system.</summary>*/
    [CreateAssetMenu(menuName = "Database/EditorExamples", fileName = "EditorExmpleDB")]
    public class EditorExamples : CoreDB<EditorExample> { }
}