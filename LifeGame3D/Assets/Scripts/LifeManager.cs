using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class LifeManager : MonoBehaviour {
    public int Max;
    public int Min;
    private static World world;
    public GameObject Prefab;
    private static GameObject gameobject;
    private static List<GameObject> objects;
    public Vector3[] PutLives;
    private int index;
    public static bool isLifeManagerInitialized;
    public bool NonStop;
    public void Start()
    {
        gameobject = gameObject;
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
            //if(objects.Any(obj=>obj.transform.localPosition==pos))continue;
            bool con = false;
            for(int i = 0; i < objects.Count; i++)
            {
                if (objects[i].transform.localPosition == pos) {
                    con = true;
                    continue;
                }
            }
            if (con)
            {
                continue;
            }
            world.Put(pos);
            GameObject newObj = Instantiate(prefab, pos, new Quaternion());
            newObj.transform.parent = gameobject.transform;
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
                    Debug.Log(world.Actives().Count+":"+objects.Count);
                    if (NonStop) break;
                    ok = false;
                    break;
                default:
                    break;
            }
        }
    }
    private void MakeView(IEnumerable<Vector3> poss)
    {
        var array = poss as Vector3[] ?? poss.ToArray();
        var delta=array.Length-objects.Count;
        if (delta>0)
        {
            for (int i = 0; i < delta; i++)
            {
                GameObject newObj = Instantiate(Prefab, new Vector3(), transform.rotation);
                newObj.transform.parent = gameobject.transform;
                objects.Add(newObj);
            }
        }
        for (int i=0;i<objects.Count;i++)
        {
            objects[i].transform.localPosition = i < array.Length ? array[i] :array.Length<1?new Vector3(-100,-100,-100):array[0];
        }
    }
}
