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

public class ActorTargetData
{
    public List<GameActor> targets;

    public ActorTargetData(List<GameActor> targets)
    {
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