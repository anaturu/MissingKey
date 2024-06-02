using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    private KeyManager keyManager;
    public bool[] levelIsCompleted;
    public Material matCompletedColor;
    public String levelToRemember;

    private void Awake()
    {
        #region Singleton
        if (instance != null)
        {
            Debug.Log("There is more than one instance of this singleton !!!");
            Destroy(gameObject);
            return;
        }
        instance = this;
        #endregion
    }

    public void UpdateKeys()
    {
        keyManager = KeyManager.instance;
        DontDestroyOnLoad(gameObject);

        for (int i = 0; i < keyManager._keyDatas.Length; i++)
        {
            if (levelIsCompleted[i]) //Si index est true
            {
                keyManager._keyDatas[i].gameObject.GetComponent<MeshRenderer>().material = matCompletedColor;
            }
        }
    }

    public void ResumeCurrentLevel()
    {
        SceneManager.LoadScene(levelToRemember);
    }
}
