public class TaskAttack : BehaviorNode
{
    private Character _character;

    public TaskAttack(Character character)
    {
        _character = character;
    }

    public override NodeState Evaluate()
    {
        TurnInstance turnInstance = _character.CurrentTurn;
        foreach (var id in turnInstance.ConActorDynamicIDs)
        {
            if (ActorsManagerCenter.Instance.GetActorByDynamicId(id).GetActorStateTag() ==
                ActorEnumType.ActorStateTag.Player)
            {
                var attackActorCommand = new AttackActorCommand(ActorsManagerCenter.Instance.GetActorByDynamicId(id));
                break;
            }
        }

        return NodeState.SUCCESS;
    }
}