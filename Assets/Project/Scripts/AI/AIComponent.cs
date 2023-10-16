

using UnityEngine;

public class AIComponent
{
    private Character _character;
    
    public AIComponent(Character character)
    {
        _character = character;
    }

    public CommandInstance GenAIInstance()
    {
        if (_character.GetCharacterBattleState() == ActorEnumType.ActorBattleState.Normal)
        {
            return GetNormalMoveCommand();
        }
        else if (_character.GetCharacterBattleState() == ActorEnumType.ActorBattleState.Battle)
        {
            
        }

        return new EmptyActorCommand();
    }

    public void OnEventHappend()
    {
        
    }
    
    MoveActorCommand GetNormalMoveCommand()
    {
        int maxDistance = 10;

        int trytimes = 100;
        while (trytimes -- > 0)
        {
            float randX = Random.Range(-maxDistance, maxDistance) + _character.transform.position.x;
            float randY = Random.Range(-maxDistance, maxDistance) + _character.transform.position.y;
            if(randX < 0 || randX >= MapSystem.Instance.GetGrid().Width || randY < 0 || randY >= MapSystem.Instance.GetGrid().Width) continue;

            if (MapSystem.Instance.GetGridActor(randX, randY) == null)
                return new MoveActorCommand(randX, randY);
        }

        return null;
    }
}