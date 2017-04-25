using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LifeManager : MonoBehaviour {
    public int Max;
    public int Min;
    private World world;
    public GameObject Prefab;
    private List<GameObject> objects;
    public Vector3[] PutLives;
    private int index;
    public void Start()
    {
        Life.LifeInitializer(Max,Min);
        world = Life.world;
        objects=new List<GameObject>();
        index = 0;
        ok = false;
        PutLife();
    }

    private void PutLife()
    {
        foreach (var pos in PutLives)
        {
            world.Put(pos);
            GameObject newObj = Instantiate(Prefab, pos, new Quaternion());
            objects.Add(newObj);
        }
    }

    public bool ok;
    public void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow)) ok = false;
        if (ok)
        {
            switch (index%3)
            {
                case 0:
                    index = 1;
                    Debug.Log("fase:"+0);
                    world.CallLookEnv();
                    Debug.Log("Environment Get");
                    break;
                case 1:
                    index = 2;
                    Debug.Log("fase:"+1);
                    world.CallMove();
                    Debug.Log("Lives moved.");
                    
                    break;
                case 2:

                    index = 0;
                    Debug.Log("fase:"+2);
                    MakeView(world.Actives());
                    Debug.Log("MakeView");
                    break;
                default:
                    Debug.Log("なぞい");
                    break;
            }
        }
    }

    private void MakeView(IEnumerable<Vector3> poss)
    {
        var enu = poss as Vector3[] ?? poss.ToArray();
        foreach (var pos in enu)
        {
            if(objects.Any(obj=>obj.transform.position==pos))continue;
            GameObject newObj=Instantiate(Prefab,pos,new Quaternion());
            objects.Add(newObj);
        }
        foreach (var obj in objects)
        {
            if (enu.Any(pos => obj.transform.position == pos)) obj.GetComponent<MeshRenderer>().enabled = true;
            else obj.GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
