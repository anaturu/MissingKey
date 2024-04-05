using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class KeyManager : MonoBehaviour
{
    [SerializeField] private KeyData[] _keyDatas;
    
    private readonly Array keyCodes = Enum.GetValues(typeof(KeyCode));

    public void Update()
    {
        if (Input.GetKeyDown(FindLastPressedInput()))
        {
            KeyData currentKey = ReturnKeyDataFromInput(FindLastPressedInput());
            
            switch(currentKey.keyStatus)
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

    KeyData ReturnKeyDataFromInput(KeyCode keyCode)
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

    KeyCode FindLastPressedInput()
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
}
