using Unity.VisualScripting;
using UnityEngine;

public class AIComponent
{
    private Character character;

    public AIComponent(Character character)
    {
        this.character = character;
    }

    private CommandInstance GenAICommand()
    {
        if (character.CurrentTurn == null)
        {
            return GetNormalState();
        }
        else
        {
            return GetTurnState();
        }
    }

    public CommandInstance GetCommand()
    {
        return GenAICommand();
    }
    

    private MoveActorCommand GetMoveCommand()
    {
        int maxDistance = 10;

        int trytimes = 100;
        while (trytimes-- > 0)
        {
            float randX = Random.Range(-maxDistance, maxDistance) + character.transform.position.x;
            float randY = Random.Range(-maxDistance, maxDistance) + character.transform.position.z;
            if (randX < 0 || randX >= MapSystem.Instance.GetGrid().Width || randY < 0 ||
                randY >= MapSystem.Instance.GetGrid().Width) continue;

            // if (MapSystem.Instance.GetGridActor(randX, randY) == null)
            return new MoveActorCommand(randX, randY, character.transform.position);
        }

        return null;
    }

    // 跟随状态的命令，在主角周围撒点，如果不可达就扩大范围，再大还不行就返回空
    private CommandInstance GetFollowMoveCommand()
    {
        GameActor player = ActorsManagerCenter.Instance.GetActorByDynamicId(TurnManager.Instance.GetCurrentPlayerId());
        var position = player.transform.position;

        if (Vector3.Distance(player.transform.position, character.transform.position) < 6) return null;

        float maxDistance = Random.Range(2, 4);

        int trytimes = 10;
        while (trytimes-- > 0)
        {
            float randX = Random.Range(1, maxDistance) + position.x;
            float randY = Random.Range(1, maxDistance) + position.z;
            if (randX < 0 || randX >= MapSystem.Instance.GetGrid().Width || randY < 0 ||
                randY >= MapSystem.Instance.GetGrid().Width) continue;

            // if (MapSystem.Instance.GetGridActor(randX, randY) == null)
            // {
            // Debug.Log("Player : " + player.transform + " " + randX + " " + randY);
            return new MoveActorCommand(randX, randY, character.transform.position);
            // }
        }

        return null;
    }


    private CommandInstance GetNormalState()
    {
        var state = character.GetCharacterType();
        if (state == ActorEnumType.AIMode.Follow)
        {
            return GetFollowMoveCommand();
        }
        else if (state == ActorEnumType.AIMode.Npc)
        {
            return GetMoveCommand();
        }

        return null;
    }

    private CommandInstance GetTurnState()
    {
        var state = character.GetCharacterType();
        if (state == ActorEnumType.AIMode.Npc)
        {
            TurnInstance turnInstance = character.CurrentTurn;
            foreach (var id in turnInstance.ActorQueue)
            {
                if (ActorsManagerCenter.Instance.GetActorByDynamicId(id).GetActorStateTag() ==
                    ActorEnumType.ActorStateTag.Player)
                {
                    return new AttackActorCommand(ActorsManagerCenter.Instance.GetActorByDynamicId(id));
                }
            }
        }

        return GetMoveCommand();
    }
}