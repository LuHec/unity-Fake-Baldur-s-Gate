using UnityEngine;

public class GameActor : MonoBehaviour
{
    public void Move(Vector3 worldPos)
    {
        transform.position = worldPos;
    }
}