using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private KeyManager keyManager;
    public int currentLevel;
    public bool[] levelIsCompleted;
    public Color completedColor;
    public Color uncompletedColor;
    void Start()
    {
        keyManager = KeyManager.instance;
        DontDestroyOnLoad(gameObject);

        for (int i = 0; i < keyManager._keyDatas.Length; i++)
        {
            if (levelIsCompleted[i])
            {
                keyManager._keyDatas[i].gameObject.GetComponent<MeshRenderer>().material.DOColor(completedColor, 0.1f);
            }
            else
            {
                keyManager._keyDatas[i].gameObject.GetComponent<MeshRenderer>().material.DOColor(uncompletedColor, 0.1f);
            }
        }
        
    }
    
    
}
