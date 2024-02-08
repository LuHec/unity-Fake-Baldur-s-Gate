using System.Collections.Generic;
using EActorTargetConfirmation;

namespace EActorTargetConfirmation
{
    public enum Type
    {
        // 直接提供目标
        INSTANT = 0,
        // 等待用户输入
        USER_CONFIRMED,
        // 等待AI输入
        CUSTOM
    }
}
public class TargetData

{
    public bool bConfirm;
    public List<GameActor> targets;

    /// <summary>
    /// 不要用其他list的引用
    /// </summary>
    /// <param name="bConfirm"></param>
    /// <param name="targets"></param>
    public TargetData(bool bConfirm, List<GameActor> targets)
    {
        this.bConfirm = bConfirm;
        this.targets = targets;
    }
}

public struct IndicatorData
{
    public EActorTargetConfirmation.Type type;
    public int maxTargetsCount;

    public IndicatorData(Type type, int maxTargetsCount)
    {
        this.type = type;
        this.maxTargetsCount = maxTargetsCount;
    }
}