using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class SaveReset : MonoBehaviour
    {
        [MenuItem("EditorTools/SaveReset")]
        private static void ResetSave()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}