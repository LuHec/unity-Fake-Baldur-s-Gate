using UnityEngine;

public class AiComponentGen3
{
    private Character character;

    public AiComponentGen3(Character character)
    {
        this.character = character;
    }

    public void UpdateAi()
    {
        if (character.moveComponent.BIsMoving)
        {
            return;
        }

        if (character.CurrentTurn == null)
        {
            UpdateFreeMode();
        }
        else
        {
            UpdateTurnMode();
        }
    }

    private void UpdateFreeMode()
    {
        if (character.GetCharacterType() == ActorEnumType.AIMode.Npc)
        {
            Move();
        }

        if (character.GetCharacterType() == ActorEnumType.AIMode.Follow)
        {
            Follow();
        }
    }

    private void UpdateTurnMode()
    {
        if (character.GetCharacterType() == ActorEnumType.AIMode.Npc)
        {
            Move();
        }

        if (character.GetCharacterType() == ActorEnumType.AIMode.Follow)
        {
            Move();
        }
    }

    // ------------------------------------------------------------------------------
    // Ai actions
    private void Move()
    {
        int tryTimes = 100;
        float maxDistance = 10;
        while (tryTimes-- > 0)
        {
            var position = character.transform.position;
            float randX = Random.Range(-maxDistance, maxDistance) + position.x;
            float randY = Random.Range(-maxDistance, maxDistance) + position.z;
            if (randX < 0 || randX >= MapSystem.Instance.GetGrid().Width || randY < 0 ||
                randY >= MapSystem.Instance.GetGrid().Width) continue;

            var target = new Vector3(randX, character.transform.position.y, randY);
            character.moveComponent.SetTarget(target);
            break;
        }
    }

    private void Follow()
    {
        float maxFollowDistance = 6f;

        GameActor player = ActorsManagerCenter.Instance.GetActorByDynamicId(TurnManager.Instance.GetCurrentPlayerId());
        var position = player.transform.position;

        if (Vector3.Distance(player.transform.position, character.transform.position) < maxFollowDistance) return;


        float maxDistance = Random.Range(2, 4);


        int tryTimes = 10;
        while (tryTimes-- > 0)
        {
            float randX = Random.Range(1, maxDistance) + position.x;
            float randY = Random.Range(1, maxDistance) + position.z;
            if (randX < 0 || randX >= MapSystem.Instance.GetGrid().Width || randY < 0 ||
                randY >= MapSystem.Instance.GetGrid().Width) continue;

            character.moveComponent.SetTarget(new Vector3(randX, character.transform.position.y, randY));
            break;
        }
    }

    private void EndAction()
    {
        character.EndAction();
    }
}