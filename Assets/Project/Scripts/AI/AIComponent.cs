using UnityEngine;

public class AIComponent
{
    private Character _character;
    private CommandInstance _commandCache;
    public bool CanGenCommandCache => _commandCache == null;

    public AIComponent(Character character)
    {
        _character = character;
    }

    private void GenAICommand()
    {
        if (_character.CurrentTurn == null)
        {
            _commandCache = new EmptyActorCommand();
        }
        else
        {
            _commandCache = GetNormalMoveCommand();
        }
    }

    public CommandInstance GetCommand()
    {
        if (CanGenCommandCache) GenAICommand();
        return _commandCache;
    }

    MoveActorCommand GetNormalMoveCommand()
    {
        int maxDistance = 10;

        int trytimes = 100;
        while (trytimes-- > 0)
        {
            float randX = Random.Range(-maxDistance, maxDistance) + _character.transform.position.x;
            float randY = Random.Range(-maxDistance, maxDistance) + _character.transform.position.y;
            if (randX < 0 || randX >= MapSystem.Instance.GetGrid().Width || randY < 0 ||
                randY >= MapSystem.Instance.GetGrid().Width) continue;

            if (MapSystem.Instance.GetGridActor(randX, randY) == null)
                return new MoveActorCommand(randX, randY);
        }

        return null;
    }

    public void ClearCommandCache()
    {
        _commandCache = null;
    }
}