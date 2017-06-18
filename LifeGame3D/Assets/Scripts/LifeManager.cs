using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading;
namespace LifeGame
{
    public class LifeManager : MonoBehaviour
    {
        //Unityのインスペクターで操作したい変数
        [SerializeField] private int Max;
        [SerializeField] private int Min;
        [SerializeField] private GameObject Prefab;
        [SerializeField] private Vector3[] PutLives;
        [SerializeField] private float interval;
        [SerializeField] private bool PutTime;

        //スクリプト内で完結する変数
        public bool Ok { get; set; }
        private static GameObject Gameobject { get; set; }
        private static List<GameObject> Objects { get; set; }
        public static bool IsLifeManagerInitialized { get; set; }
        private static World World { get; set; }
        private List<List<Vector3>> PosList { get; set; }
        private float Time { get; set; }
        private bool End { get; set; }
        private bool Treadmove { get; set; }
        private static List<Vector3> StartPositions;
        //Start関数
        private void Start()
        {//いろいろと初期化してる。
            Gameobject = gameObject;
            Life.LifeInitializer(Max, Min);
            World = Life.World;
            Objects = new List<GameObject>();
            Ok = false;
            //PutLife(PutLives, Prefab);
            StartPositions = new List<Vector3> { };
            StartPosAdd(PutLives);
            
            IsLifeManagerInitialized = true;
            PosList = new List<List<Vector3>> { };
            Time = 0;
            End = false;
            Treadmove = true;
        }

        public static void StartPosAdd(Vector3[] poss)
        {
            for(int i = 0; i < poss.Length; i++)
            {
                StartPosAdd(poss[i]);
            }
        }
        public static void StartPosAdd(Vector3 pos)
        {
            bool newPos = true;
            for(int i = 0; i < StartPositions.Count; i++)
            {
                if (StartPositions[i] == pos)
                {
                    newPos = false;
                    break;
                }
            }
            if (newPos)
            {
                StartPositions.Add(pos);
            }
        }
        //Update関数
        public void Update()
        {//スレッドを使って、LifeMove関数を非同期で呼び出している。
            if ( !End&&Ok)
            {
                if (Treadmove)
                {
                    Treadmove = false;
                    var thread = new Thread(new ThreadStart(LifeMove));
                    thread.Start();
                }
            }
        }
        //LateUpdate関数
        private void LateUpdate()
        {//インターバル毎にMakeView関数を呼び出している。
            Time += UnityEngine.Time.deltaTime;
            if (Ok && Time >= interval)
            {
                if (PosList.Count >= 1)
                {
                    MakeView(PosList[0]);
                    PosList.Remove(PosList[0]);
                    Time = 0;
                }
                else
                {
                    if (End)
                    {
                        while (Objects.Count >= 1)
                        {
                            Destroy(Objects[0]);
                            Objects.Remove(Objects[0]);
                        }
                    }
                }
            }
        }
        //MakeView関数
        private void MakeView(IEnumerable<Vector3> poss)
        {//ライフの状況を描写する。
            var array = poss as Vector3[] ?? poss.ToArray();
            var delta = array.Length - Objects.Count;
            if (delta > 0)
            {
                for (int i = 0; i < delta; i++)
                {
                    GameObject newObj = Instantiate(Prefab, new Vector3(), transform.rotation);
                    newObj.transform.parent = Gameobject.transform;
                    Objects.Add(newObj);
                }
            }
            for (int i = 0; i < Objects.Count; i++)
            {
                Objects[i].GetComponent<ParticleSystem>().Stop();
                Objects[i].GetComponent<ParticleSystem>().Clear();
                var len = array.Length;
                if (i < len)
                {
                    Objects[i].transform.localPosition = array[i];
                    Objects[i].GetComponent<ParticleSystem>().Play();
                }
                else
                {
                    Objects[i].transform.localPosition = new Vector3();
                }
            }
        }
        //PutLife関数
        public static void PutLife(List<Vector3> array, GameObject prefab)
        {//初期位置のライフを配置している。
            foreach (var pos in array)
            {
                //if(objects.Any(obj=>obj.transform.localPosition==pos))continue;
                bool con = false;
                for (int i = 0; i < Objects.Count; i++)
                {
                    if (Objects[i].transform.localPosition == pos)
                    {
                        con = true;
                        continue;
                    }
                }
                if (con)
                {
                    continue;
                }
                World.Put(pos);
                GameObject newObj = Instantiate(prefab, pos, new Quaternion());
                newObj.transform.parent = Gameobject.transform;
                Objects.Add(newObj);
            }
        }
        //LifeMove関数
        private void LifeMove()
        {//Lifeの動きを計算している。
            World.CallLookEnv();
            World.CallMove();
            var actives = World.Actives(true);
            if (actives.Count >= 1)
            {
                PosList.Add(actives);
            }
            if (actives.Count < 1)
            {
                End = true;
                Debug.Log("収束しました！\n是非再生しちゃってくだせえ！！");
            }
            Treadmove = true;
        }
        //GameStart関数
        public void GameStart()
        {
            if (!Ok)
            {
                PutLife(StartPositions,Prefab);
                Ok = true;
            }
            else
            {
                /*
                  一旦動きを止めて直前の状態をObjectsに保存
                  Lifeを全部falseにする
                */
                Ok = false;
            }
        }
    }
}
