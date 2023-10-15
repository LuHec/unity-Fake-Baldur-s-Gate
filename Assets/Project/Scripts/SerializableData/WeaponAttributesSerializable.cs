[System.Serializable]
public class WeaponAttributesSerializable
{
    public uint id;
    public string name;
    public float damage;
    public int aoe;
    public float maxDist;

    public WeaponAttributesSerializable(uint id, string name, float damage, int aoe, float maxDist)
    {
        this.id = id;
        this.name = name;
        this.damage = damage;
        this.aoe = aoe;
        this.maxDist = maxDist;
    }
}