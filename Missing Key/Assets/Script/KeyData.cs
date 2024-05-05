using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class KeyData : MonoBehaviour
{
    [Header("DO NOT TOUCH")]
    public string code;
    
    private KeyManager keyManager;
    
    public KeyStatus keyStatus; //Variable contenant l'enum "KeyStatus"
    public KeyData[] adjacentKeyDatas; //Contient  toute les touches adjacentes à la touche
    [HideInInspector] public Vector3 keyPos;

    private Transform keyTransform;
    public GameObject tpOutput;


    public bool isPressed;

    public enum KeyStatus //Tous les states spéciaux des touches spéciales
    {
        Basic, 
        Mine, 
        Hole,
        Start,
        Victory,
        Teleporter
    }

    private void Start()
    {
        code = gameObject.name;
            
        keyPos = transform.position;
        isPressed = false;
        keyManager = KeyManager.instance;
    }

    public void OnPressed()
    {
        
    }

    public void OnRelease()
    {
        
    }
}
