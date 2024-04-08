using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class KeyManager : MonoBehaviour
{
    //DONT TOUCH THESE
    [SerializeField] private KeyData[] _keyDatas; //Array du script "KeyData"
    private readonly Array keyCodes = Enum.GetValues(typeof(KeyCode)); //Array contenant TOUTES les touches du clavier

    [SerializeField] private float travelSpeed;
    [SerializeField] private Color keyColor;
    [SerializeField] private Color adjacentKeyColor;
    
    [SerializeField] private List<GameObject> keyList = new List<GameObject>();
    
    [SerializeField] private bool canPlayLevel;

    private void Start()
    {
        canPlayLevel = false;

    }

    public void Update()
    {
        PressKeyBehavior();
        ReleaseKeyBehavior();

        if (keyList.Count > 2)
        {
            //SceneManager.LoadScene("SampleScene");
            Debug.Log("More than 2 keys : GAME OVER");
        }

        if (keyList.Count == 0 && canPlayLevel)
        {
            //SceneManager.LoadScene("SampleScene");
            Debug.Log("0 keys held down : GAME OVER");
        }
        
        if (keyList.Count == 1 && !canPlayLevel)
        {
            //SceneManager.LoadScene("SampleScene");
            Debug.Log("Wrong starting key pressed : GAME OVER");
        }
        
    }

    private void PressKeyBehavior()
    {
        if (Input.GetKeyDown(FindLastPressedInput())) //Si je press n'importe quel touche du clavier
        {
            KeyData currentKey = ReturnKeyDataFromInput(FindLastPressedInput());

            
            
            //FOR THE SINGLE KEY PRESSED
            for (int i = 0; i < keyCodes.Length; i++)
            {
                currentKey.isPressed = true;
                currentKey.transform.DOMove(currentKey.keyPos + new Vector3(0, 0.4f, 0), travelSpeed);
                currentKey.gameObject.GetComponent<MeshRenderer>().material.DOColor(keyColor, 0.2f);
            }
            
            //FOR EVERY ADJACENT KEYS PRESSED
            for (int i = 0; i < currentKey.adjacentKeyDatas.Length; i++) // i = int qui représente chaque touche du clavier adjacente à la currentKey une par une 
            {
                currentKey.adjacentKeyDatas[i].isPressedAdjacent = true;
                currentKey.adjacentKeyDatas[i].gameObject.GetComponent<MeshRenderer>().material.DOColor(adjacentKeyColor, 0.2f);

                Debug.Log("Adjacent keys are being pushed down");
            }

            #region Special Keys
            
            switch(currentKey.keyStatus)
            {
                case KeyData.KeyStatus.Basic:
                    keyList.Add(currentKey.gameObject);
                    break;
                case KeyData.KeyStatus.Start:
                    canPlayLevel = true;
                    keyList.Add(currentKey.gameObject);

                    Debug.Log("AZEAZEAZE");

                    break;
                case KeyData.KeyStatus.Mine:
                    break;
                case KeyData.KeyStatus.Hole:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            #endregion

        }
    }
    private void ReleaseKeyBehavior()
    {
        if (Input.GetKeyUp(FindLastReleasedInput()))
        {
            KeyData currentKey = ReturnKeyDataFromInput(FindLastReleasedInput());

            //FOR THE SINGLE KEY RELEASED
            for (int i = 0; i < keyCodes.Length; i++)
            {
                currentKey.isPressed = false;
                currentKey.transform.DOMove(currentKey.keyPos, travelSpeed);
                
            }
            
            //FOR EVERY ADJACENT KEYS RELEASED
            for (int i = 0; i < currentKey.adjacentKeyDatas.Length; i++) // i = int qui représente chaque touche du clavier adjacente à la currentKey une par une 
            {
                currentKey.adjacentKeyDatas[i].isPressedAdjacent = false;
                currentKey.adjacentKeyDatas[i].gameObject.GetComponent<MeshRenderer>().material.DOColor(keyColor, 0.2f);
            }
            
            #region Special Keys

            switch(currentKey.keyStatus)
            {
                case KeyData.KeyStatus.Basic:
                    keyList.Remove(currentKey.gameObject);
                    break;
                case KeyData.KeyStatus.Start:
                    keyList.Remove(currentKey.gameObject);

                    break;
                case KeyData.KeyStatus.Mine:
                    break;
                case KeyData.KeyStatus.Hole:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            #endregion
            
            
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
