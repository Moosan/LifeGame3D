using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LifeManager : MonoBehaviour {
    public int Max;
    public int Min;
    private static World world;
    public GameObject Prefab;
    private static List<GameObject> objects;
    public Vector3[] PutLives;
    private int index;
    public static bool isLifeManagerInitialized;
    public void Start()
    {
        Life.LifeInitializer(Max,Min);
        world = Life.World;
        objects=new List<GameObject>();
        index = 0;
        ok = false;
        PutLife(PutLives,Prefab);
        isLifeManagerInitialized = true;
    }

    public static void PutLife(Vector3[] array,GameObject prefab)
    {
        foreach (var pos in array)
        {
            if(objects.Any(obj=>obj.transform.position==pos))continue;
            world.Put(pos);
            GameObject newObj = Instantiate(prefab, pos, new Quaternion());
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
                    world.CallLookEnv();
                    break;
                case 1:
                    index = 2;
                    world.CallMove();
                    
                    break;
                case 2:
                    index = 0;
                    MakeView(world.Actives());
                    //ok = false;
                    break;
                default:
                    break;
            }
        }
    }

    private void MakeView(IEnumerable<Vector3> poss)
    {
        var array = poss as Vector3[] ?? poss.ToArray();
        if (array.Length > objects.Count)
        {
            for (int i = 0; i < array.Length - objects.Count; i++)
            {
                objects.Add(Instantiate(Prefab,new Vector3(),new Quaternion()));
            }
        }
        for (int i=0;i<objects.Count;i++)
        {
            objects[i].transform.position = i < array.Length ? array[i] : new Vector3(-100,-100,-100);
        }
        /*foreach (var pos in array)
        {
            if(objects.Any(obj=>obj.transform.position==pos))continue;
            GameObject newObj=Instantiate(Prefab,pos,new Quaternion());
            objects.Add(newObj);
        }
        foreach (var obj in objects)
        {
            if (array.Any(pos => obj.transform.position == pos)) obj.GetComponent<MeshRenderer>().enabled = true;
            else obj.GetComponent<MeshRenderer>().enabled = false;
        }*/
    }
}
