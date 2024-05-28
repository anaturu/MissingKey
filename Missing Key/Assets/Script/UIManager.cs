using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    
    public GameObject pauseMenu;
    public GameObject optionsMenu;

    public float targetTime;
    public TextMeshProUGUI timerText;
    
    public bool isPaused;
    public bool isLevelSelector;
    private void Awake()
    {
        #region Singleton
        if (instance != null)
        {
            Debug.LogWarning("There is more than one instance of this singleton !!!");
            return;
        }
        instance = this;
        #endregion
    }

    private void Start()
    {
        pauseMenu.SetActive(false);
        timerText = GameObject.Find("Timer").GetComponent<TextMeshProUGUI>();

        

    }

    private void Update()
    {
        targetTime -= Time.deltaTime;
        timerText.text = targetTime + "";
        
        if (!isLevelSelector) //Si le menu LevelSelector est fermé
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused) //Si le menu Pause est ouvert
                {
                    ButtonResumeGame(); //Ferme le menu Pause
                }
                else if (!isPaused) //Si le menu Pause est fermé
                {
                    PauseGame(); //Ouvre le menu Pause
                }
            } 
        }

        if (isLevelSelector) //Si le menu LevelSelector est ouvert
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!isPaused) //Si le menu Pause est fermé
                {
                    ButtonBack(); //Retourne au menu Pause
                }
                else //Si le menu Pause est ouvert
                {
                    ButtonResumeGame();
                }
            }
        }
    }
    
    public void ButtonResumeGame()
    {
        pauseMenu.SetActive(false); //Désactive Menu Pause
        Time.timeScale = 1f;

        isPaused = false;
    }
    
    public void ButtonOptionsMenu()
    {
        pauseMenu.SetActive(false); //Désactive Menu Pause
        optionsMenu.SetActive(true); //Active Menu Options
        isLevelSelector = true;
        isPaused = false;
        
        SceneManager.LoadScene("Level Selector");
    }

    
    public void PauseGame()
    {
        pauseMenu.SetActive(true); //Active Menu Pause
        Debug.Log("PAUSE");
        
        isPaused = true;
    }
    public void ButtonBack()
    {
        optionsMenu.SetActive(false); //Désactive Menu Options
        pauseMenu.SetActive(true); //Active Menu Pause

        isLevelSelector = false;
        isPaused = true;
    }
    
    public void ButtonMainMenu()
    {
        Time.timeScale = 1f;
        //SceneManager.LoadScene("START SCREEN");
    }


}
