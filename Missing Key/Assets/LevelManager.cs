using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    private KeyManager keyManager;
    public bool[] levelIsCompleted;
    public Material matCompletedColor;
    public Material matUncompletedColor;
    public Color completedColor;
    public Color uncompletedColor;
    
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
    
    
}
