using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    [SerializeField] private Dropdown levelsDropdown;

    public void GenerateDropdown(LevelData[] levels)
    {
        levelsDropdown.options = levels.Select(x => new Dropdown.OptionData(x.name)).ToList();
    }

    public void OnDropdownValueChanged()
    {
        GameManager.instance.LoadLevel(levelsDropdown.captionText.text);
    }
}
