using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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

    private MeshRenderer mr;


    public enum KeyStatus //Tous les states spéciaux des touches spéciales
    {
        Basic, 
        Mine, 
        Hole,
        Start
    }

    private void Start()
    {
        mr = GetComponent<MeshRenderer>();
        
        code = gameObject.name;
        keyPos = transform.position;
        isPressed = false;
        isPressedAdjacent = false;
    }

    public void OnPressed()
    {
        mr.material.DOColor(KeyManager.instance.keyPressedColor, 0.2f);
    }

    public void OnRelease()
    {
        mr.material.DOColor(KeyManager.instance.keyColor, 0.2f);
    }

    public void OnHighlight()
    {
        mr.material.DOColor(KeyManager.instance.adjacentKeyColor, 0.2f);
    }

    private void OnGUI()
    {
        var position = Camera.main.WorldToScreenPoint(transform.position);
        var text = $"{isPressed} / {isPressedAdjacent}";
        
        var textSize = GUI.skin.label.CalcSize(new GUIContent(text));
        GUI.Label(new Rect(position.x, Screen.height - position.y, 200, 500), text);
    }
}
