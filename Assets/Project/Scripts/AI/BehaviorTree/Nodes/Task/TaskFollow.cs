using UnityEngine;

public class TaskFollow : BehaviorNode
{
    private Character _character;

    public TaskFollow(Character character)
    {
        _character = character;
    }

    public override NodeState Evaluate()
    {
        GameActor player = ActorsManagerCenter.Instance.GetActorByDynamicId(TurnManager.Instance.GetCurrentPlayerId());
        var position = player.transform.position;

        if (Vector3.Distance(player.transform.position, _character.transform.position) < 6) return NodeState.SUCCESS;

        float maxDistance = Random.Range(2, 4);

        int trytimes = 10;
        while (trytimes-- > 0)
        {
            float randX = Random.Range(1, maxDistance) + position.x;
            float randY = Random.Range(1, maxDistance) + position.z;
            if (randX < 0 || randX >= MapSystem.Instance.GetGrid().Width || randY < 0 ||
                randY >= MapSystem.Instance.GetGrid().Width) continue;

            if (MapSystem.Instance.GetGridActor(randX, randY) == null)
            {
                // Debug.Log("Player : " + player.transform + " " + randX + " " + randY);
                var moveActorCommand = new MoveActorCommand(randX, randY, _character.transform.position);
                return NodeState.SUCCESS;
            }
        }

        return NodeState.FAILURE;
    }
}