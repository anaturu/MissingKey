using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;
using DG.Tweening;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class KeyManager : MonoBehaviour
{
    //DONT TOUCH THESE
    [SerializeField] private KeyData[] _keyDatas; //Array du script "KeyData"
    private readonly Array keyCodes = Enum.GetValues(typeof(KeyCode)); //Array contenant TOUTES les touches du clavier

    [SerializeField] private float travelSpeed;
    public string currentLevelName;
    [CanBeNull] public string nextLevelName;

    [SerializeField] private List<KeyData> keyList = new List<KeyData>();
    
    [SerializeField] private bool canPlayLevel;
    [SerializeField] private bool isAdjacent;

    public static KeyManager instance;

    private UIManager _uiManager;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        canPlayLevel = false;
        _uiManager = UIManager.instance;
    }

    public void Update()
    {
        PressKeyBehavior();
        ReleaseKeyBehavior(); 

        if (keyList.Count > 2 && keyList[0].keyStatus != KeyData.KeyStatus.Mine)
        {
            SceneManager.LoadScene(currentLevelName);
            Debug.Log("More than 2 keys : GAME OVER");
        }

        if (keyList.Count == 0 && canPlayLevel)
        {
            SceneManager.LoadScene(currentLevelName);
            Debug.Log("0 keys held down : GAME OVER");
        }
        
        if (keyList.Count == 1 && !canPlayLevel)
        {
            SceneManager.LoadScene(currentLevelName);
            Debug.Log("Wrong starting key pressed : GAME OVER");
        }
        
    }

    private void PressKeyBehavior()
    {
        if (Input.GetKeyDown(FindLastPressedInput())) //Si je press n'importe quel touche du clavier
        {
            KeyData currentKey = ReturnKeyDataFromInput(FindLastPressedInput());
            
            //FOR THE SINGLE KEY PRESSED
            currentKey.OnPressed();
            currentKey.isPressed = true;
            currentKey.transform.DOMove(currentKey.keyPos + new Vector3(0, 0.4f, 0), travelSpeed);

            
           
            
            //FOR EVERY ADJACENT KEYS PRESSED
            for (int i = 0; i < currentKey.adjacentKeyDatas.Length; i++) // i = int qui représente chaque touche du clavier adjacente à la currentKey une par une 
            {
                
            }
            
            #region Special Keys
            
            switch(currentKey.keyStatus)
            {
                case KeyData.KeyStatus.Basic:
                    keyList.Add(currentKey.GetComponent<KeyData>());
                    break;
                case KeyData.KeyStatus.Start:
                    canPlayLevel = true;
                    keyList.Add(currentKey.GetComponent<KeyData>());
                    break;
                case KeyData.KeyStatus.Mine:
                    keyList.Add(currentKey.GetComponent<KeyData>());
                    
                    for (int i = 0; i < currentKey.adjacentKeyDatas.Length; i++) // i = int qui représente chaque touche du clavier adjacente à la currentKey une par une 
                    {
                        currentKey.adjacentKeyDatas[i].transform.DORotate(new Vector3(0, 0, 180), 0.2f);
                    }
                    break;
                case KeyData.KeyStatus.Hole:
                    SceneManager.LoadScene(currentLevelName);
                    Debug.Log("Void pressed : GAME OVER");
                    break;
                case KeyData.KeyStatus.Victory:
                    keyList.Add(currentKey.GetComponent<KeyData>());
                    
                    if (canPlayLevel)
                    {
                        SceneManager.LoadScene(nextLevelName);
                        Debug.Log("LEVEL IS COMPLETED !");
                    }
                    else
                    {
                        SceneManager.LoadScene(currentLevelName);
                        Debug.Log("Wrong starting key pressed : GAME OVER");
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            #endregion
            
            if(keyList.Count > 1)
            {
                if (CheckIfAdjacent(currentKey, keyList[0])) //check si la touche est adjacente ou non (currentKey étant toujours la touche la plus récente)
                {
                    //DO CODE HERE pour la key pressé adjacente la plus récente
                }
                else
                {
                    SceneManager.LoadScene(currentLevelName);
                    Debug.Log("Pressed key is not adjacent : GAME OVER");
                }
            }

            if (keyList[0].keyStatus == KeyData.KeyStatus.Mine) //Si une mine est active
            {
                if (CheckIfNeutralized(keyList[0]))
                {
                    Debug.Log("Toutes les touches adjacentes à la mine ont été enfoncés");

                }
                
            }
        }
        
        
    }
    private void ReleaseKeyBehavior()
    {
        if (Input.GetKeyUp(FindLastReleasedInput()))
        {
            KeyData currentKey = ReturnKeyDataFromInput(FindLastReleasedInput());

            //FOR THE SINGLE KEY RELEASED
            currentKey.isPressed = false;
            currentKey.transform.DOMove(currentKey.keyPos, travelSpeed);
                
            if (currentKey.isPressed)
            {
                currentKey.OnPressed();
            }
            else
            {
                currentKey.OnRelease();
            }
            
            //FOR EVERY ADJACENT KEYS RELEASED
            for (int i = 0; i < currentKey.adjacentKeyDatas.Length; i++) // i = int qui représente chaque touche du clavier adjacente à la currentKey une par une 
            {
                
            }
            
            #region Special Keys

            switch(currentKey.keyStatus)
            {
                case KeyData.KeyStatus.Basic:
                    keyList.Remove(currentKey.GetComponent<KeyData>());
                    break;
                case KeyData.KeyStatus.Start:
                    keyList.Remove(currentKey.GetComponent<KeyData>());
                    break;
                case KeyData.KeyStatus.Mine:
                    keyList.Remove(currentKey.GetComponent<KeyData>());

                    break;
                case KeyData.KeyStatus.Victory:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            #endregion
        }
        
    }
    private IEnumerator VictoryEvent()
    {
        yield return new WaitForSeconds(1);
        
    }

    private bool CheckIfNeutralized(KeyData currentPressedMine)
    {
        for (int i = 0; i < currentPressedMine.adjacentKeyDatas.Length; i++)
        {
            if (!currentPressedMine.adjacentKeyDatas[i].isPressed) //Si toutes les touches adjacentes sont pressés
            {
                return false;
            }
        }
        return true;
    }
    private bool CheckIfAdjacent(KeyData keyToCheck, KeyData pressedKey)
    {
        for (int i = 0; i < pressedKey.adjacentKeyDatas.Length; i++)
        {
            if (keyToCheck == pressedKey.adjacentKeyDatas[i])
            {
                return true;
            }
        }

        return false;
    }
        
    public KeyData ReturnKeyDataFromInput(KeyCode keyCode) //Permet de traduire keyCode en KeyData (script)
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
    public KeyCode FindLastPressedInput() //Correspond au dernier Input pressé
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
    public KeyCode FindLastReleasedInput() //Correspond au dernier Input relâché
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
