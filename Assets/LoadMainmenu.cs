using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMainmenu : MonoBehaviour
{
    public void Load()
    {
        SceneManager.LoadScene(Defines.SceneNames.mainMenu);
    }
}
