using System;
using System.Collections.Generic;
using OfficeOpenXml.FormulaParsing.Excel.Operators;

public class PlayerActorContainer
{
    public List<uint> PlayerActorsIdList => _playerActorsIdList;
    public int Ptr => _ptr;
    private ActorsManagerCenter _actorsManagerCenter;

    #region #delgate

    #endregion

    #region #Listenr

    #endregion

    private List<uint> _playerActorsIdList;
    private int _ptr;

    public PlayerActorContainer(List<uint> playerList)
    {
        _actorsManagerCenter = ActorsManagerCenter.Instance;
        _playerActorsIdList = playerList;
    }

    public void Init(List<uint> initIds)
    {
        _playerActorsIdList = initIds;
    }

    public void AddPlayerById(uint id)
    {
        PlayerActorsIdList.Add(id);
    }

    public uint GetPlayerIdByIndex(int idx) => PlayerActorsIdList[idx];
    public uint GetCurrentPlayer => PlayerActorsIdList[_ptr];

    /// <summary>
    /// 切换角色，调整ai模块
    /// </summary>
    /// <param name="idx"></param>
    public void ChangePlayerByIdx(int idx)
    {
        var currentPlayer = ActorsManagerCenter.Instance.GetActorByDynamicId(GetCurrentPlayer) as Character;
        _ptr = idx;
        var newPlayer = ActorsManagerCenter.Instance.GetActorByDynamicId(GetCurrentPlayer) as Character;

        currentPlayer.SetCharacterStateTo(ActorEnumType.ActorStateTag.AI);
        currentPlayer.SetAIMode(ActorEnumType.AIMode.Follow);

        newPlayer.SetCharacterStateTo(ActorEnumType.ActorStateTag.Player);
    }
}