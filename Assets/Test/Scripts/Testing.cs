using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

public class Testing : MonoBehaviour
{
    private PlayerInput _playerInput;
    private Pathfiding _pathfiding;
    public CharacterAttributesScriptobjectData _characterAttributesScriptobject;
    public WeaponAttributesScriptobjectData _weapon;

    public GameActor character;
    public Object objects;

    private void Start()
    {
        var objects = ResourcesLoader.LoadAllControlledActorsResource();
        
        character = objects[0].GetComponent<GameActor>();
        var ca = character as Character;
        Debug.Log(ca);
    }
    
    
}