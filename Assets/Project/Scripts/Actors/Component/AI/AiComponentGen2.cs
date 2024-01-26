using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class AiComponentGen2
{
    private Character character;
    private AbilitySystem abilitySystem;

    private MoveComponent moveComponent;

    // private Action onActionEnd;
    private bool isAction;

    private float maxFollowDistance = 6f;
    int maxDistance = 10;

    public AiComponentGen2(Character character)
    {
        this.character = character;
        moveComponent = character.moveComponent;
        abilitySystem = character.abilitySystem;

        moveComponent.onMoveFinished += OnActionEnd;
    }

    public void Loop()
    {
        if (character.CurrentTurn == null)
        {
            if (character.GetCharacterType() == ActorEnumType.AIMode.Follow)
            {
                TryFollow();
            }

            if (character.GetCharacterType() == ActorEnumType.AIMode.Npc)
            {
                TryMove();
            }
        }
    }

    private void TryMove()
    {
        if (TryLock() == false) return;

        int trytimes = 100;
        while (trytimes-- > 0)
        {
            float randX = Random.Range(-maxDistance, maxDistance) + character.transform.position.x;
            float randY = Random.Range(-maxDistance, maxDistance) + character.transform.position.z;
            if (randX < 0 || randX >= MapSystem.Instance.GetGrid().Width || randY < 0 ||
                randY >= MapSystem.Instance.GetGrid().Width) continue;

            // if (MapSystem.Instance.GetGridActor(randX, randY) == null)
            moveComponent.SetTarget(new Vector3(randX, character.transform.position.y, randY));
            break;
        }
    }

    private void TryFollow()
    {
        if (TryLock() == false) return;
        
        GameActor player = ActorsManagerCenter.Instance.GetActorByDynamicId(TurnManager.Instance.GetCurrentPlayerId());
        var position = player.transform.position;

        if (Vector3.Distance(player.transform.position, character.transform.position) < maxFollowDistance)
        {
            OnActionEnd();
            return;
        }

        float maxDistance = Random.Range(2, 4);


        int trytimes = 10;
        while (trytimes-- > 0)
        {
            float randX = Random.Range(1, maxDistance) + position.x;
            float randY = Random.Range(1, maxDistance) + position.z;
            if (randX < 0 || randX >= MapSystem.Instance.GetGrid().Width || randY < 0 ||
                randY >= MapSystem.Instance.GetGrid().Width) continue;

            moveComponent.SetTarget(new Vector3(randX, character.transform.position.y, randY));
        }
    }


    public void OnActionEnd()
    {
        isAction = false;
    }

    private bool TryLock()
    {
        if (isAction) return false;
        return isAction = true;
    }
}