using System.Collections.Generic;
using UnityEngine;
namespace LifeGame
{
    public class Life
    {
        public Vector3 Pos { get; private set; }
        public int Own { get; set; }
        public List<int> Env { get; set; }
        public bool DoHaveEnv { get; set; }
        public bool Active { get; private set; }
        private bool isGood;
        private static Vector2 MaxMin { get; set; }
        public static World World { get; set; }
        public bool IsGood { get; set; }
        private static void WorldInitializer()
        {//ワールドを初期化
            World.WorldInitializer();
            World = new World();
        }
        public static void LifeInitializer(int max, int min)
        {//ライフを初期化
            WorldInitializer();
            MaxMin = new Vector2(max, min);
        }
        public Life(Vector3 pos)
        {//一般のコンストラクタ
            Env = new List<int>();
            this.Pos = pos;
            DoHaveEnv = false;
            Active = true;
            Own=World.Add(this);
            World.GiveEnv(this);
        }
        public Life(Vector3 pos, bool active)
        {//ワールド専用のコンストラクタ
            Env = new List<int>();
            this.Pos = pos;
            DoHaveEnv = false;
            this.Active = active;
            Own=World.Add(this);
        }
        private int EnvState()
        {//Envirnmentの中でアクティブなライフの数
         //return Env.Count(next=>next.IsActive());
            int count = 0;
            for (int i = 0; i < Env.Count; i++)
            {
                try
                {
                    if (World.GetLife(Env[i]).Active)
                    {
                        count++;
                    }
                }
                catch (System.Exception e)
                {
                    Debug.Log(i + ":" + Env.Count);
                    Debug.Log(e);
                    throw;
                }
            }
            return count;
        }
        public void LookEnv()
        {
            var state = EnvState();

            if (state < MaxMin.x)
            {
                if (state > MaxMin.y)
                {
                    isGood = true;
                    return;
                }
            }
            isGood = false;
        }
        public void Move()
        {
            if (Active)
            {
                if (!isGood)
                {
                    Dead();
                }
            }
            else
            {
                if (isGood)
                {
                    Born();
                }
            }
            isGood = false;
        }
        private void Dead()
        {
            Active = false;
        }
        private void Born()
        {
            Active = true;
            if (DoHaveEnv) return;
            World.GiveEnv(this);
        }
        public void SetActive()
        {
            Active = true;
            if (DoHaveEnv) return;
            World.GiveEnv(this);
        }
    }
}
