using System;

public class ActorEnumType
{
    [Serializable]
    public enum ActorType
    {
        Character,
        Pickableitem,
        Unpickableitem
    }

    [Serializable]
    public enum CharacterType
    {
    }

    [Serializable]
    public enum PickableItemType
    {
        Weapon,
        Good
    }

    [Serializable]
    public enum ActorStateTag
    {
        AI,
        Player,
    }

    [Serializable]
    public enum ActorHateState
    {
        Hate,
        Normal,
        Ally,
        Self
    }

    [Serializable]
    public enum ActorBattleState
    {
        Normal,
        Battle
    }
}