public class PickableItem : GameActor
{
    #region #TypeInfo

    protected ActorEnumType.PickableItemType _pickableItemType;
    public ActorEnumType.PickableItemType GetPickableItemType() => _pickableItemType;
    
    #endregion

    protected override void InitExtend()
    {
        actorEnumType = ActorEnumType.ActorType.Pickableitem;
    }
}