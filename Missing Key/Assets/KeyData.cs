using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyData : MonoBehaviour
{
    [Header("DO NOT TOUCH")]
    public string code;
    
    public KeyStatus keyStatus; //Variable contenant l'enum "KeyStatus"
    public KeyData[] adjacentKeyDatas; //Contient  toute les touches adjacentes à la touche
    [HideInInspector] public Vector3 keyPos;
    public bool isPressed;
    public bool isPressedAdjacent;

    public enum KeyStatus //Tous les states spéciaux des touches spéciales
    {
        Basic, 
        Mine, 
        Hole
    }


    private void Start()
    {
        code = gameObject.name;
        keyPos = transform.position;
        isPressed = false;
        isPressedAdjacent = false;
    }

    public void Explode()
    {
        
    }
}
