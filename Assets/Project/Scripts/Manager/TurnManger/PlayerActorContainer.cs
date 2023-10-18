using System;
using System.Collections.Generic;
using OfficeOpenXml.FormulaParsing.Excel.Operators;

public class PlayerActorContainer
{
    public List<uint> PlayerActors => _playerActors;
    public int Ptr => _ptr;
    private ActorsManagerCenter _actorsManagerCenter;

    #region #delgate

    #endregion

    #region #Listenr

    #endregion

    private List<uint> _playerActors;
    private int _ptr;

    public PlayerActorContainer(List<uint> playerList)
    {
        _actorsManagerCenter = ActorsManagerCenter.Instance;
        _playerActors = playerList;
    }

    public void Init(List<uint> initIds)
    {
        _playerActors = initIds;
    }

    public void AddPlayerById(uint id)
    {
        PlayerActors.Add(id);
    }

    public uint GetPlayerIdByIndex(int idx) => PlayerActors[idx];
    public uint GetCurrentPlayer => PlayerActors[_ptr];

    /// <summary>
    /// 切换角色，调整ai模块
    /// </summary>
    /// <param name="idx"></param>
    public void ChangePlayerById(int idx)
    {
        _ptr = idx;

        for (int i = 0; i < PlayerActors.Count; i++)
        {
            GameActor actor = _actorsManagerCenter.GetActorByDynamicId(PlayerActors[i]);
            if (i == _ptr)
            {
                actor.SetCharacterStateTo(ActorEnumType.ActorStateTag.Player);
            }
            else
            {
                actor.SetCharacterStateTo(ActorEnumType.ActorStateTag.AI);
                (actor as Character).SetAIMode(ActorEnumType.AIMode.Follow);
            }
        }
    }
}