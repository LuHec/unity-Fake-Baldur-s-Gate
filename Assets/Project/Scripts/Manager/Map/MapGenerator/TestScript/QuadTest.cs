using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class QuadTest : MonoBehaviour
{
    [SerializeField] private int width = 100;
    [SerializeField] private int height = 100;
    [SerializeField] private int smallMapSize = 10;
    [SerializeField]private int generateSize = 100;
    private QuadTree quadTree;

    public void Awake()
    {
        quadTree = new QuadTree(Vector3.zero, width, height);

        for (int i = 0; i < generateSize; i++)
        {
            var center = new Vector3(Random.Range(smallMapSize, width - smallMapSize), 0,
                Random.Range(smallMapSize, height - smallMapSize));
            var size = new Vector3(smallMapSize, smallMapSize, smallMapSize);
            float subWidth = size.x / 2;
            float subHeight = size.z / 2;

            Vector3 topLeft = new Vector3(center.x - subWidth, 0, center.z + subHeight);
            Vector3 topRight = new Vector3(center.x + subWidth, 0, center.z + subHeight);
            Vector3 botLeft = new Vector3(center.x - subWidth, 0, center.z - subHeight);
            Vector3 botRight = new Vector3(center.x + subWidth, 0, center.z - subHeight);

            Debug.DrawLine(topLeft, botLeft, Color.green, 1000f);
            Debug.DrawLine(topLeft, topRight, Color.green, 1000f);
            Debug.DrawLine(botLeft, botRight, Color.green, 1000f);
            Debug.DrawLine(botRight, topRight, Color.green, 1000f);
            
            Debug.Log(center);

            quadTree.Insert(new QuadMapInfo(new Bounds(center, size), (uint)i + 1));
        }
    }

    private void Update()
    {
        if (quadTree != null)
        {
            Debug.Log(quadTree.QueryMap(GetMouse3DPosition(LayerMask.GetMask("Default"))));
        }
        Debug.Log(GetMouse3DPosition(LayerMask.GetMask("Default")));
    }
    
    public Vector3 GetMouse3DPosition(int mouseLayerMask)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, mouseLayerMask))
        { 
            return raycastHit.point;
        }
        else
        {
            return Vector3.zero;
        }
        
    }

    public Vector3 GetMouse3DPosition()
    {
        Vector3 point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
        return point;
    }
}