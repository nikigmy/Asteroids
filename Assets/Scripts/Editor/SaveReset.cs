using UnityEngine;
using System.Collections;
using UnityEditor;

public class SaveReset : MonoBehaviour {
    
    [MenuItem("EditorTools/SaveReset")]
    static void ResetSave()
    {
        PlayerPrefs.DeleteAll();
    }
}
