using UnityEngine;

public class GameActor : MonoBehaviour
{
    [SerializeField] private PlacedObjectTypeSO _placedObjectTypeSo;
    public PlacedObjectTypeSO PlacedObject => _placedObjectTypeSo;
    public string _name;
    private MapSystem _mapSystem;
    public float X => _x;
    public float Z => _z;
    private float _x;
    private float _z;
   
    
    public void MoveTo(float x, float z)
    {
        _x = x;
        _z = z;
        transform.position = new Vector3(x, transform.position.y, z);
    }
}