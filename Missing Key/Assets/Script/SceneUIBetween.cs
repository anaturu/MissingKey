using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneUIBetween : MonoBehaviour
{
    public static SceneUIBetween instance;
    private KeyManager _keyManager;

    public float appearDeathTextTime;
    public float disappearDeathTextTime;
    public TextMeshProUGUI[] deathTexts;
    
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
        _keyManager = KeyManager.instance;
       
    }

    public void UpdateText()
    {
        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator DeathTextEvent(int index)
    {
        //Appear Death Text
        deathTexts[index].DOTMPScale(1, appearDeathTextTime);
        yield return new WaitForSeconds(1f);
        deathTexts[index].DOTMPScale(0, disappearDeathTextTime);
    }
}
