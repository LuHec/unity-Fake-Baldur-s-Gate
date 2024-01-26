public class TurnItem
{
    public readonly int maxAtbAmount = 10;
    public int currentAtbAmount = 0;

    public bool BIsCharacter => character == null;
    public AbilitySystem abilitySystem;
    public TurnInstance turnInstance;
    public Character character;

    // 作为角色使用
    public TurnItem(TurnInstance turnInstance, Character character)
    {
        this.turnInstance = turnInstance;
        this.character = character;
        this.abilitySystem = character.abilitySystem;
    }

    // 作为Tick使用
    public TurnItem(TurnInstance turnInstance, AbilitySystem abilitySystem)
    {
        this.turnInstance = turnInstance;
        this.abilitySystem = abilitySystem;
    }

    public void UpdateItem()
    {
        // 角色和回合分开更新
        if (BIsCharacter)
        {
            character.ActorUpdate();
        }
        else
        {
            turnInstance.AddAbilityTask(abilitySystem.TryGetAbility("Ga_TickTurn"));
        }
    }

    public void ClearAtb()
    {
        currentAtbAmount = 0;
    }

    public void IncreaseAtb()
    {
        currentAtbAmount += abilitySystem.characterAttributeSet.SpeedAtb;
    }
}