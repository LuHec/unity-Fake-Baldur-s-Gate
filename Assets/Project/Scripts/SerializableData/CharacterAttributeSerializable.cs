[System.Serializable]
public class CharacterAttributeSerializable
{
    // 读表部分
    public uint id;
    public string name;
    public float maxHp;
    public int maxActPoints;
    public uint weaponId;

    public CharacterAttributeSerializable(uint id, string name, float maxHp, int maxActPoints, uint weaponId)
    {
        this.id = id;
        this.name = name;
        this.maxHp = maxHp;
        this.maxActPoints = maxActPoints;
        this.weaponId = weaponId;
    }
}