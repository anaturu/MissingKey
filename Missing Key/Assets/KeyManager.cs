using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyManager : MonoBehaviour
{
    public KeyBehaviour keyState;
    
    public enum KeyBehaviour
    {
        BaseKey,
    }
    void Start()
    {
        
    }
    void Update()
    {
        switch (keyState) //Condition d'entrée du switch
        {
            case KeyBehaviour.BaseKey :
                //DO CODE HERE
                Debug.Log("TEST");
            break;
        }
    }
}
