using System;
using System.Collections.Generic;
using UnityEngine;

public class EventArgsType
{
    public class ActorDieMessage : EventArgs
    {
        public uint dead_dynamic_id;
        public bool removeActor = false;

        public ActorDieMessage(uint deadDynamicID, bool remove = false)
        {
            dead_dynamic_id = deadDynamicID;
            removeActor = remove;
        }
    }

    public class TurnNeedRemoveMessage : EventArgs
    {
        public TurnInstance turnInstance;

        public TurnNeedRemoveMessage(TurnInstance turnInstance)
        {
            this.turnInstance = turnInstance;
        }
    }

    public class ActorAttackingMessage : EventArgs
    {
        // 发动攻击方
        public uint attacker_dynamic_id;

        // 遭受攻击方
        public uint attacked_dynamic_id;

        public ActorAttackingMessage(uint attackerDynamicID, uint attackedDynamicID)
        {
            attacker_dynamic_id = attackerDynamicID;
            attacked_dynamic_id = attackedDynamicID;
        }
    }

    public class UpdateTurnManagerMessage : EventArgs
    {
        public TurnInstance newTurn;
        public HashSet<TurnInstance> turnRemoveSet;

        public UpdateTurnManagerMessage(TurnInstance newTurn, HashSet<TurnInstance> turnRemoveSets)
        {
            this.newTurn = newTurn;
            this.turnRemoveSet = turnRemoveSets;
        }
    }

    public class GameModeSwitchMessage : EventArgs
    {
        public enum GameMode
        {
            _3RD,
            Turn    
        }

        public GameMode gameMode;

        public GameModeSwitchMessage(GameMode mode)
        {
            gameMode = mode;
        }
    }
    
    public class PlayerSelectMessage : EventArgs
    {
        public int PlayerIdx => playerIdx;
        private int playerIdx;

        public PlayerSelectMessage(int playerIdx)
        {
            this.playerIdx = playerIdx;
        }
    }

    public class PlayerBackTurnMessage : EventArgs
    {
        public int backCount = 5;
    }
    
    // ------------------------------------------------------------------------------
    // AbilitySystem
    public class PlayerConfirmMessage : EventArgs
    {
        public bool bOnUI;

        public PlayerConfirmMessage(bool bOnUI)
        {
            this.bOnUI = bOnUI;
        }
    }

    public class PlayerCancelMessage : EventArgs
    {
        public bool bOnUI;

        public PlayerCancelMessage(bool bOnUI)
        {
            this.bOnUI = bOnUI;
        }
    }
}