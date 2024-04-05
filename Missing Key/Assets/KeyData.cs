using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyData : MonoBehaviour
{
    public string code;
    public KeyStatus keyStatus;
    public KeyData[] adjacentKeyDatas;
    
    public enum KeyStatus
    {
        Basic, Mine, Hole
    }

    private void Start()
    {
        code = gameObject.name;
    }

    public void Explode()
    {
        
    }
}
