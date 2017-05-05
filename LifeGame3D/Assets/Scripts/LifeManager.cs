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
    private float t;
    private double startDt, endDt, ts;
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
                    Debug.Log("-------------------------------------------");
                    //Life全体に、自分の周りにいるLifeの状況(State)を見ろって言ってる
                    //Call→LookEnvironment
                    //CallLookEnvはworldクラスにあって、foreachで書かれてる。
                    //並列化したい。
                    index = 1;
                    
                    startDt =Time.realtimeSinceStartup ;
                    world.CallLookEnv();
                    endDt = Time.realtimeSinceStartup;
                    ts = endDt - startDt;
                    Debug.Log("LookEnv:"+ts); 
                    break;
                case 1:
                    //さっき周りの状況を見たので、それに従って生まれるか生き残るか死ぬかしろって言ってる。
                    //このCallMoveメソッドもforeachで書かれてる
                    //並列化したい。
                    index = 2;
                    
                    startDt = Time.realtimeSinceStartup;
                    world.CallMove();
                    endDt = Time.realtimeSinceStartup;
                    ts = endDt - startDt;
                    Debug.Log("CallMove:"+ts);
                    break;
                case 2:
                    //ライフが勝手に動いてくれたみたいなので
                    //worldから、生きてるライフの場所のリストだけもらって
                    //このリストから、ライフの場所をUnity上に表示する。
                    index = 0;
                    
                    startDt = Time.realtimeSinceStartup;
                    MakeView(world.Actives());
                    endDt = Time.realtimeSinceStartup;
                    ts = endDt - startDt;
                    Debug.Log("MakeView:"+ts);
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
