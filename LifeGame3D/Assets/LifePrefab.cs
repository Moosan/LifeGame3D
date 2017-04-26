using UnityEngine;

public class LifePrefab : MonoBehaviour
{
    public GameObject Prefab;
    public Vector3 CenterPos;
    public Vector3[] PresetPos;
    private Vector3 pos;
    private Vector3[] positions;
    public Axis DefaltMoveDirection;
    public Axis Axis { get; set; }
    public void Start()
    {
        Vector3 position = transform.position;
        pos = new Vector3((int)position.x,(int)position.y,(int)position.z);
        //向きを変えれるようにしたいけどまあそこはおいおい
        var len = PresetPos.Length;
        positions = new Vector3[len];
        for (int i = 0; i < len; i++) {
            positions[i] = pos + PresetPos[i]-CenterPos;
        }
    }
    public void Update()
    {
        if (LifeManager.isLifeManagerInitialized)
        {
            LifeManager.PutLife(positions, Prefab);
            Destroy(gameObject);
            Destroy(this);
        }
    }
}
public enum Axis
{
    x,y,z
}
