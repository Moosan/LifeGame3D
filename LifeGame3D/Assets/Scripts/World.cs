using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Linq;
namespace LifeGame
{
    public class World
    {
        private List<Life> Lives { get; set; }
        private static Vector3[] Directions { get; set; }
        private Thread[] Threads { get; set; }
        public int Length()
        {
            return Lives.Count;
        }
        private static void DirectionsInitializer()
        {
            Directions = new Vector3[26];
            var count = 0;
            for (var i = -1; i <= 1; i++)
            {
                for (var j = -1; j <= 1; j++)
                {
                    for (var k = -1; k <= 1; k++)
                    {
                        if (i == 0 && j == 0 && k == 0) continue;
                        Directions[count] = new Vector3(i, j, k);
                        count++;
                    }
                }
            }
        }
        public static void WorldInitializer()
        {
            DirectionsInitializer();
        }
        public World()
        {
            Lives = new List<Life>();
        }
        private Life LifeExists(Vector3 pos)
        {
            for (int i = 0; i < Lives.Count; i++)
            {
                if (Lives[i].Pos == pos)
                {
                    return Lives[i];
                }
            }
            return null;
        }
        public int Add(Life life)
        {
            Lives.Add(life);
            return Lives.Count - 1;
        }
        public Life GetLife(int i)
        {
            return Lives[i];
        }
        public void GiveEnv(Life life)
        { 
         //ライフが、自分の周りのリストの配列を取得してる
            var pos = life.Pos;
            life.Env = new List<int>();
            foreach (var next in Directions)
            {
                var newPos = pos + next;
                life.Env.Add((LifeExists(newPos) ?? new Life(newPos, false)).Own);
            }
            foreach (var nearown in life.Env)
            {
                /*if (nearLife.Env.Any(nearnearLife => nearnearLife.GetPos() == life.GetPos()))
                {
                    continue;
                }*/
                bool Add = true;
                var nearLife = GetLife(nearown);
                for (int i = 0; i < nearLife.Env.Count; i++)
                {
                    if (GetLife(nearLife.Env[i]).Pos == life.Pos)
                    {
                        Add = false;
                        break;
                    }
                }
                if (!Add)
                {
                    continue;
                }
                nearLife.Env.Add(life.Own);
            }
            life.DoHaveEnv = true;
        }
        public List<Vector3> Actives()
        {
            return (from life in Lives where life.Active select life.Pos).ToList();
        }
        public void CallLookEnv()
        {
            /*foreach (var life in Lives)
            {
                life.LookEnv();
            }*/
            for (int i = 0; i < Lives.Count; i++)
            {
                try { Lives[i].LookEnv(); }
                catch (
                System.Exception e)
                {
                    Debug.Log(i + ":" + Lives.Count);
                    Debug.Log(e);
                    throw;
                }
            }
        }
        public void CallMove()
        {
          //最も並列化したい場所
            var Lives2 = Lives.ToList();
            
            //*
            foreach (var life in Lives2)
            {
                life.Move();
            }
            /*/
            Threads = new Thread[Lives2.Count];
            for (int i = 0; i < Lives2.Count; i++)
            {
                // 2.
                // Thread クラスの構築する
                Threads[i] = new Thread(new ThreadStart(Lives2[i].Move));

                // 3.
                // Start を使ってスレッドの開始する
                Threads[i].Start();
            }
            // スレッド終了待ち
            for (int i = 0; i < Lives2.Count; i++)
            {
                // 4.
                // Join を使ってスレッドの終了を待つ
                Threads[i].Join();
            }
            //*/
        }
        private bool ExistLife(Vector3 pos)
        {
            //return Lives.Any(life => life.GetPos() == pos);
            for (int i = 0; i < Lives.Count; i++)
            {
                if (Lives[i].Pos == pos)
                {
                    return true;
                }
            }
            return false;
        }
        public Life Put(Vector3 pos)
        {
            if (ExistLife(pos))
            {
                Life life;
                //var life = Lives.Find(ele=>ele.GetPos() == pos);
                for (int i = 0; i < Lives.Count; i++)
                {
                    if (Lives[i].Pos == pos)
                    {
                        life = Lives[i];
                        life.SetActive();
                        return life;
                    }
                    else
                    {
                        life = null;
                    }
                }
            }
            return new Life(pos);
        }
    }
}