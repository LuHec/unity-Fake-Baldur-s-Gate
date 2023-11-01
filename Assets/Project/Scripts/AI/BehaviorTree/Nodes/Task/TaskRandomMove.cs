using UnityEngine;

public class TaskRandomMove : BehaviorNode
{
    private Character _character;


    public TaskRandomMove(Character character)
    {
        _character = character;
    }

    public override NodeState Evaluate()
    {
        // 运行随机走动，GenCommand到Tree上
        int maxDistance = 10;

        int trytimes = 100;
        while (trytimes-- > 0)
        {
            float randX = Random.Range(-maxDistance, maxDistance) + _character.transform.position.x;
            float randY = Random.Range(-maxDistance, maxDistance) + _character.transform.position.z;
            if (randX < 0 || randX >= MapSystem.Instance.GetGrid().Width || randY < 0 ||
                randY >= MapSystem.Instance.GetGrid().Width) continue;

            if (MapSystem.Instance.GetGridActor(randX, randY) == null)
            {
                var moveActorCommand = new MoveActorCommand(randX, randY, _character.transform.position);
                break;
            }
        }

        return NodeState.SUCCESS;
    }
}