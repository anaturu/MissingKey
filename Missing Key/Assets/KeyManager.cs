using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;
using DG.Tweening;

public class KeyManager : MonoBehaviour
{
    [Header("DO NOT TOUCH")]
    [SerializeField] private KeyData[] _keyDatas; //Array du script "KeyData"
    private readonly Array keyCodes = Enum.GetValues(typeof(KeyCode)); //Array contenant TOUTES les touches du clavier

    [SerializeField] private float travelSpeed;

    public void Update()
    {
        if (Input.GetKeyDown(FindLastPressedInput())) //Si je press n'importe quel touche du clavier
        {
            KeyData currentKey = ReturnKeyDataFromInput(FindLastPressedInput());

            #region AllKeys
            
            //DO CODE pour n'importe quelle touche
            for (int i = 0; i < currentKey.adjacentKeyDatas.Length; i++) // i = int qui représente chaque touche du clavier adjacente à la currentKey une par une 
            {
                currentKey.adjacentKeyDatas[i].transform.DOMove(currentKey.adjacentKeyDatas[i].keyPos + new Vector3(0, 1, 0), travelSpeed);
            }
            
            #endregion
           
            
            switch(currentKey.keyStatus) //DO CODE pour les touches spécifiques
            {
                case KeyData.KeyStatus.Basic:
                    
                    break;
                case KeyData.KeyStatus.Mine:
                    currentKey.Explode();
                    break;
                case KeyData.KeyStatus.Hole:
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        if (Input.GetKeyUp(FindLastReleasedInput()))
        {
            KeyData currentKey = ReturnKeyDataFromInput(FindLastReleasedInput());
            
            Debug.Log("OAJUFOLIAEPIHJAEFP");
            //DO CODE pour n'importe quelle touche
            
            for (int i = 0; i < currentKey.adjacentKeyDatas.Length; i++) // i = int qui représente chaque touche du clavier adjacente à la currentKey une par une 
            {
                currentKey.adjacentKeyDatas[i].transform.DOMove(currentKey.adjacentKeyDatas[i].keyPos, travelSpeed);
            }
            
            switch(currentKey.keyStatus) //DO CODE pour les touches spécifiques
            {
                case KeyData.KeyStatus.Basic:
                    
                    break;
                case KeyData.KeyStatus.Mine:
                    currentKey.Explode();
                    break;
                case KeyData.KeyStatus.Hole:
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    KeyData ReturnKeyDataFromInput(KeyCode keyCode) //Permet de traduire keyCode en KeyData (script)
    {
        KeyData value = new KeyData();
        
        foreach (var kd in _keyDatas)
        {
            if (kd.code == keyCode.ToString())
            {
                value = kd;
                break;
            }
        }

        return value;
    }

    KeyCode FindLastPressedInput() //Correspond au dernier Input pressé
    {
        KeyCode value = new KeyCode();
        foreach (KeyCode keyCode in keyCodes)
        {
            if (Input.GetKeyDown(keyCode))
            {
                value = keyCode;
                break;
            }
        }
        return value;
    }
    KeyCode FindLastReleasedInput() //Correspond au dernier Input relâché
    {
        KeyCode value = new KeyCode();
        foreach (KeyCode keyCode in keyCodes)
        {
            if (Input.GetKeyUp(keyCode))
            {
                value = keyCode;
                break;
            }
        }
        return value;
    }
}
