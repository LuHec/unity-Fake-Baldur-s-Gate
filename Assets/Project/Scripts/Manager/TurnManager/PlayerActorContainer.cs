using System;
using System.Collections.Generic;
using OfficeOpenXml.FormulaParsing.Excel.Operators;

public class PlayerActorContainer
{
    public List<uint> PlayerActorsIdList => playerActorsIdList;
    public int Ptr => ptr;

    #region #delgate

    #endregion

    #region #Listenr

    #endregion

    private List<uint> playerActorsIdList;
    private int ptr;

    public PlayerActorContainer(List<uint> playerList)
    {
        playerActorsIdList = playerList;
    }

    public void Init(List<uint> initIds)
    {
        playerActorsIdList = initIds;
    }

    public void AddPlayerById(uint id)
    {
        PlayerActorsIdList.Add(id);
    }

    public uint GetPlayerIdByIndex(int idx) => PlayerActorsIdList[idx];
    public uint GetCurrentPlayer => PlayerActorsIdList[ptr];

    /// <summary>
    /// 切换角色，调整ai模块
    /// </summary>
    /// <param name="idx"></param>
    public void ChangePlayerByIdx(int idx)
    {
        var currentPlayer = ActorsManagerCenter.Instance.GetActorByDynamicId(GetCurrentPlayer) as Character;
        ptr = idx;
        var newPlayer = ActorsManagerCenter.Instance.GetActorByDynamicId(GetCurrentPlayer) as Character;

        currentPlayer.SetCharacterStateTo(ActorEnumType.ActorStateTag.AI);
        currentPlayer.SetAIMode(ActorEnumType.AIMode.Follow);

        newPlayer.SetCharacterStateTo(ActorEnumType.ActorStateTag.Player);
    }
}