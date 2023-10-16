using System;

public class EventArgsType
{
    public class ActorDieMessage : EventArgs
    {
        public ActorDieMessage(uint deadDynamicID)
        {
            dead_dynamic_id = deadDynamicID;
        }

        public uint dead_dynamic_id;
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
}