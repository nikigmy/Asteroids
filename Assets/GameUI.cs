using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class GameUI : MonoBehaviour
{
    private LevelManager levelManager;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Transform heartContaner;
    public void Init(LevelManager levelManager)
    {
        this.levelManager = levelManager;
        
        levelManager.HealthChanged += UpdateHearths;
        levelManager.ScoreChanged += UpdateScore;
        
        levelManager.PoolManager.RegisterGenerator(Defines.PoolKey.Heart.ToString(), new ObjectGenerator(Defines.ResourcePaths.uiHearthPrefabPath) );
        levelManager.PoolManager.PopulatePool<GameObject>(Defines.PoolKey.Heart.ToString(),
            GameManager.instance.Config.startHealth);
        
        UpdateHearths(levelManager.Health);
        UpdateScore(levelManager.Score);
    }

    private void UpdateScore(int score)
    {
        scoreText.text = "Score: " + levelManager.Score;
    }

    private void UpdateHearths(int health)
    {
        var hearts = heartContaner.childCount;
        if (health < hearts)
        {
            for (int i = hearts - 1; i > health; i--)
            {
                var child = heartContaner.GetChild(i).gameObject;
                levelManager.PoolManager.ReturnObject(Defines.PoolKey.Heart.ToString(), child);
            }
        }
        else if(health > hearts)
        {
            var heartObjects = levelManager.PoolManager.RetrieveObjects<GameObject>(Defines.PoolKey.Heart.ToString(), health - hearts);
            for (int i = 0; i < health - hearts; i++)
            {
                heartObjects[i].transform.parent = heartContaner;
                heartObjects[i].SetActive(true);
            }
        }
    }

    public void OnSettingButtonClicked()
    {
        OpenSettings();
    }

    private void OpenSettings()
    {
        throw new System.NotImplementedException();
    }


    public void OnMainMenuClicked()
    {
        GameManager.instance.LoadMainMenu();
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
