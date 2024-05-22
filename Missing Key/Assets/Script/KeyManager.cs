using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;
using DG.Tweening;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;

public class KeyManager : MonoBehaviour
{
    //DONT TOUCH THESE
    [SerializeField] private KeyData[] _keyDatas; //Array du script "KeyData"
    private readonly Array keyCodes = Enum.GetValues(typeof(KeyCode)); //Array contenant TOUTES les touches du clavier
    
    public static KeyManager instance;
    private UIManager _uiManager;

    [SerializeField] private List<KeyData> keyList = new List<KeyData>();
    
    public KeyData[] fakeVictoryKeys;
    public KeyData[] blinkKeys;
    public GameObject transitionCube;
    public Transform transitionStartPos;

    public int indexFakeVictory;
    public int indexWinEvent;
    
    [SerializeField] private float travelSpeed;
    [SerializeField] private float levelTransitionSpeed;
    public string loadCurrentLevel;
    [CanBeNull] public string loadNextLevel;
    
    [SerializeField] private bool canPlayLevel;
    [SerializeField] private bool isAdjacent;
    [SerializeField] private bool gameHasBegun;
    [SerializeField] private bool blinkLevel;
    [SerializeField] private Color blinkColor;
    [SerializeField] private Color blinkWarningColor;
    [SerializeField] private Color basicColor;

    

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        canPlayLevel = false;
        gameHasBegun = false;
        _uiManager = UIManager.instance;
        
        if (blinkLevel)
        {
            StartCoroutine(BlinkEvent());
        }

        //Keyboard.current.onTextInput += CheckKeyboardInputPressed;
    }

    public void Update()
    {
        PressKeyBehavior();
        ReleaseKeyBehavior();
        
        //CheckKeyboardInputStillPressed();
        //return;

        if (keyList.Count > 2 && keyList[0].keyStatus != KeyData.KeyStatus.Mine)
        {
            SceneManager.LoadScene(loadCurrentLevel);
            Debug.Log("More than 2 keys : GAME OVER");
        }

        if (keyList.Count == 0 && canPlayLevel)
        {
            SceneManager.LoadScene(loadCurrentLevel);
            Debug.Log("0 keys held down : GAME OVER");
        }
        
        if (keyList.Count == 1 && !canPlayLevel)
        {
            SceneManager.LoadScene(loadCurrentLevel);
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
            currentKey.transform.DOMove(currentKey.keyPos + new Vector3(0, 0.2f, 0), travelSpeed);
            //currentKey.transform.DORotate(new Vector3(0f, 360f, 0f), 0.2f, RotateMode.LocalAxisAdd);

            
            //FOR EVERY ADJACENT KEYS PRESSED
            for (int i = 0; i < currentKey.adjacentKeyDatas.Length; i++) // i = int qui représente chaque touche du clavier adjacente à la currentKey une par une 
            {
                
            }
            
            #region Special Keys

            var keyData = currentKey.GetComponent<KeyData>();
            
            switch(currentKey.keyStatus)
            {
                case KeyData.KeyStatus.Basic:
                    keyList.Add(keyData);
                    break;
                case KeyData.KeyStatus.Start:
                    canPlayLevel = true;
                    keyList.Add(keyData);
                    break;
                case KeyData.KeyStatus.Mine:
                    keyList.Add(keyData);
                    for (int i = 0; i < currentKey.adjacentKeyDatas.Length; i++) // Pour toutes les touches adjacentes à la touche pressé
                    {
                        currentKey.adjacentKeyDatas[i].transform.DORotate(new Vector3(0, 0, 180), 0.2f);
                    }
                    break;
                case KeyData.KeyStatus.Hole:
                    SceneManager.LoadScene(loadCurrentLevel);
                    Debug.Log("Void pressed : GAME OVER");
                    break;
                case KeyData.KeyStatus.Teleporter:
                    keyList.Add(keyData);
                    currentKey.tpOutput.GetComponent<MeshRenderer>().material = currentKey.GetComponent<MeshRenderer>().material;
                    break;
                case KeyData.KeyStatus.FakeVictory:
                    keyList.Add(keyData);
                    if (fakeVictoryKeys[indexFakeVictory].isPressed) //Si l'index actuel est pressé
                    {
                        if (fakeVictoryKeys[indexFakeVictory+1] == fakeVictoryKeys[fakeVictoryKeys.Length -1]) //Si l'élément suivant dans la liste est le dernier élément de la liste
                        {
                            fakeVictoryKeys[indexFakeVictory+1].GetComponent<MeshRenderer>().material = currentKey.GetComponent<MeshRenderer>().material;
                            fakeVictoryKeys[indexFakeVictory+1].keyStatus = KeyData.KeyStatus.Victory;
                            Debug.Log("Last Touch");
                        }
                        else
                        {
                            fakeVictoryKeys[indexFakeVictory+1].GetComponent<MeshRenderer>().material = currentKey.GetComponent<MeshRenderer>().material;
                            fakeVictoryKeys[indexFakeVictory+1].keyStatus = KeyData.KeyStatus.FakeVictory;
                            indexFakeVictory++;
                        }
                    }
                    break;
                case KeyData.KeyStatus.Victory:
                    keyList.Add(currentKey.GetComponent<KeyData>());
                    if (canPlayLevel)
                    {
                        StartCoroutine(StartWinEvent());
                    }
                    else
                    {
                        SceneManager.LoadScene(loadCurrentLevel);
                        Debug.Log("Wrong starting key pressed : GAME OVER");
                    }
                    break;
                case KeyData.KeyStatus.Blink:
                    keyList.Add(currentKey.GetComponent<KeyData>());
                    SceneManager.LoadScene(loadCurrentLevel);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            #endregion
            
            if(keyList.Count > 1)
            {
                //check si la touche est adjacente ou non (currentKey étant toujours la touche la plus récente),
                //et keyList[0] étant la plus ancienne
                if (CheckIfAdjacent(currentKey, keyList[0]))
                {
                    //DO CODE HERE pour la key pressé adjacente la plus récente
                }
                else
                {
                    SceneManager.LoadScene(loadCurrentLevel);
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
            
            if (currentKey.keyStatus == KeyData.KeyStatus.Teleporter)
            {
                Debug.Log("TP est enfoncé");
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
                case KeyData.KeyStatus.Teleporter:
                    keyList.Remove(currentKey.GetComponent<KeyData>());
                    break;
                case KeyData.KeyStatus.FakeVictory:
                    keyList.Remove(currentKey.GetComponent<KeyData>());
                    break;
                case KeyData.KeyStatus.Blink:
                    keyList.Remove(currentKey.GetComponent<KeyData>());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            #endregion
        }
    }

    private IEnumerator StartWinEvent()
    {
        //Freeze la touche de victoire pressé sur sa position
        if (keyList[indexWinEvent].isPressed) //Si l'index actuel est pressé
        {
            //Si la key pressé dans la liste est le dernier élément de la liste de touches pressés ET a le state "Victory"
            if (keyList[indexWinEvent + 1] == keyList[keyList.Count - 1]) 
            {
                //Tween sur la victory key
                // keyList[indexWinEvent + 1].transform.DOMoveY(1f, 0.3f).OnComplete(() =>
                // {
                //     keyList[keyList.Count - 1].transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InElastic);
                // });
            }
        }
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(loadNextLevel);
        Debug.Log("LEVEL IS COMPLETED !");

    }

    private IEnumerator BlinkEvent()
    {
        for (int i = 0; i < blinkKeys.Length; i++)
        {
            blinkKeys[i].gameObject.GetComponent<MeshRenderer>().material.DOColor(blinkColor, 0.1f);
            blinkKeys[i].keyStatus = KeyData.KeyStatus.Blink;

            //GAME OVER
            if (blinkKeys[i].isPressed)
            {
                yield return new WaitForSeconds(2f);
                SceneManager.LoadScene(loadCurrentLevel);
            }
            
            if (i != blinkKeys.Length - 1) //Début de la liste
            {
                blinkKeys[i + 1].gameObject.GetComponent<MeshRenderer>().material.DOColor(blinkWarningColor, 0.1f);
            }
            else //Fin de la liste
            {
                blinkKeys[0].gameObject.GetComponent<MeshRenderer>().material.DOColor(blinkWarningColor, 0.1f);
            }
            yield return new WaitForSeconds(0.3f);
            
            blinkKeys[i].gameObject.GetComponent<MeshRenderer>().material.DOColor(basicColor, 0.1f);
            blinkKeys[i].keyStatus = KeyData.KeyStatus.Basic;
            yield return new WaitForSeconds(0.1f);
            
            if (i == blinkKeys.Length - 1)
            {
                StartCoroutine(BlinkEvent());
            }
        }
        
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

    //------------------------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------------------------
    
    private void CheckKeyboardInputPressed(Char c)
    {
        KeyData currentKey = ReturnKeyDataFromChar(c);

        if (!currentKey) return;
        if (currentKey.isPressed) return;
        
        keyList.Add(currentKey);
        
        currentKey.OnPressed();
        currentKey.isPressed = true;
        currentKey.transform.DOMove(currentKey.keyPos + new Vector3(0, 0.4f, 0), travelSpeed);
    }

    private void CheckKeyboardInputStillPressed()
    {
        foreach (KeyData currentKey in keyList.ToArray())
        {
            if (currentKey.isPressed)
            {
                Debug.Log("YOUPI");
        
                currentKey.OnPressed();
                currentKey.isPressed = false;
                currentKey.transform.DOMove(currentKey.keyPos, travelSpeed);

                keyList.Remove(currentKey);
            }
        }
    }

    private KeyData ReturnKeyDataFromChar(Char c)
    {
        KeyData value = new KeyData();
        
        foreach (var kd in _keyDatas)
        {
            if (kd.code == c.ToString().ToUpper())
            {
                value = kd;
                break;
            }
        }

        return value;
    }
}
