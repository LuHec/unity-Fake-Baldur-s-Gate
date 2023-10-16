using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

public class Testing : MonoBehaviour
{
    public Projectile projectile;
    private PlayerInput _playerInput;
    private Pathfiding _pathfiding;
    public CharacterAttributesScriptobjectData _characterAttributesScriptobject;
    public WeaponAttributesScriptobjectData _weapon;

    public GameActor character;
    public Object objects;

    private void Start()
    {
        var proj = Instantiate(projectile);
        proj.StartMove(new Vector3(200, 1, 200));
    }
    
    
}