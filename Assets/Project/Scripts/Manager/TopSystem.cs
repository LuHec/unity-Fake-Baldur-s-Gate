using System;
using System.Collections;
using System.Collections.Generic;
using LuHec.Utils;
using UnityEngine;

/// <summary>
///  顶层系统
/// </summary>
public class TopSystem : MonoBehaviour
{
    [SerializeField] private int _gridwidth = 10;
    [SerializeField] private int _gridheight = 10;
    [SerializeField] private float _cellsize = 10f;
    [SerializeField] private Vector3 _originPos = Vector3.zero;

    private CommandCenter _commandCenter;
    private ActorsManagerCenter _actorsManagerCenter;
    private MessageCenter _messageCenter;
    private MapSystem _mapSystem;
    
    #region #System Functions

    /// <summary>
    /// 初始化系统
    /// </summary>
    private void Awake()
    {
        InitMap();
        InitSystem();
    }


    private void Update()
    {
        AddCommand();
        Excute();
    }

    #endregion
    

    #region #Init Functions

    void InitSystem()
    {
        _commandCenter = new CommandCenter(_mapSystem);
        _actorsManagerCenter = new ActorsManagerCenter(_mapSystem);
        _messageCenter = new MessageCenter(_mapSystem);
    }

    void InitMap()
    {
        _mapSystem = new MapSystem(_gridwidth, _gridheight, _cellsize, _originPos);
    }

    #endregion

    #region #Custom Functions

    void AddCommand()
    {
        List<GameActor> actors = _actorsManagerCenter.GetPlayerControlledActorsList();
        foreach (var actor in actors)
        {
            _commandCenter.AddCommand(_commandCenter.GetInputCommand(), actor);
        }
    }
    
    void Excute()
    {
        List<GameActor> actors = _actorsManagerCenter.GetPlayerControlledActorsList();
        foreach (var actor in actors)
        {
            _commandCenter.Excute(actor.GetCommand(), actor);
        }
        
    }
    

    #endregion
}