using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifePrefab : MonoBehaviour
{
    public GameObject Prefab;
    public Vector3[] PresetPos;
    private Vector3 pos;
    private Vector3[] positions;
    public Axis DefaltMoveDirection;
    public Axis Axis { get; set; }
    public void Start()
    {
        Vector3 position = transform.position;
        pos = new Vector3((int)position.x,(int)position.y,(int)position.z);
        
        var len = PresetPos.Length;
        positions = new Vector3[len];
        for (int i = 0; i < len; i++) {
            positions[i] = pos + PresetPos[i]-PresetPos[0];
        }
    }
    public void Update()
    {
        if (LifeManager.isLifeManagerInitialized)
        {
            LifeManager.PutLife(positions, Prefab);
            Destroy(this);
        }
    }
}
public enum Axis
{
    x,y,z
}
