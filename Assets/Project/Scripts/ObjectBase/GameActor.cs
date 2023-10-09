using UnityEngine;

public class GameActor : MonoBehaviour
{
    private MapSystem _mapSystem;

    public void MoveUnit(float x, float z)
    {
        transform.position = new Vector3(x, transform.position.y, z);
    }
}