﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Life {
    private Vector3 Pos { get; set; }

    public Vector3 GetPos()
    {
        return Pos;
    }

    public List<Life> Env { get; set; }
    public bool DoHaveEnv { get; set; }
    private bool Active { get; set; }
    public bool IsActive()
    {
        return Active;
    }
    private bool isGood;
    private static Vector2 MaxMin { get; set; }
    public static World World { get; set; }

    public bool IsGood
    {
        get
        {
            return isGood;
        }

        set
        {
            isGood = value;
        }
    }

    private static void WorldInitializer()
    {//ワールドを初期化
        World.WorldInitializer();
        World=new World();
    }
    public static void LifeInitializer(int max,int min)
    {//ライフを初期化
        WorldInitializer();
        Debug.Log("WorldInitialized");
        MaxMin=new Vector2(max,min);
        Debug.Log("LifeInitialized");
    }
    public Life(Vector3 pos)
    {//一般のコンストラクタ
        Env=new List<Life>();
        this.Pos = pos;
        DoHaveEnv = false;
        Active = true;
        World.Add(this);
        World.GiveEnv(this);
    }
    public Life(Vector3 pos, bool active)
    {//ワールド専用のコンストラクタ
        Env=new List<Life>();
        this.Pos = pos;
        DoHaveEnv = false;
        this.Active = active;
        World.Add(this);
    }
    public Life(Vector3 pos, bool active,bool hoge)
    {//急遽作られた謎のコンストラクタ
        Env = new List<Life>();
        this.Pos = pos;
        DoHaveEnv = false;
        this.Active = active;
        //Live3にAddしてる、こうすることで、worldのCallMove内あるforeach文の母集合の要素数に変更を加えることを阻止している。
        World.Lives3.Add(this);
    }
    private int EnvState()
    {//Envirnmentの中でアクティブなライフの数
        return Env.Count(next=>next.IsActive());
    }
    public void LookEnv()
    {
        var state = EnvState();
        
        if (state < MaxMin.x)
        {
            if (state > MaxMin.y)
            {

                isGood = true ;
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
        if(DoHaveEnv)return;
        World.GiveEnv(this);
    }
    public void SetActive()
    {
        Active = true;
        if (DoHaveEnv) return;
        World.GiveEnv(this);
    }
}
public class World
{
    private List<Life> Lives { get; set; }
    private static Vector3[] Directions;

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
    public static void WorldInitializer() {
        DirectionsInitializer();
    }
    public World()
    {
        Lives=new List<Life>();
    }
    private Life LifeExists(Vector3 pos)
    {
        return Lives.Find(elem => elem.GetPos() == pos);
    }
    public void Add(Life life)
    {
        Lives.Add(life);
    }
    public void GiveEnv(Life life)
    {
        var pos = life.GetPos();
        life.Env=new List<Life>();
        foreach (var next in Directions)
        {
            var newPos = pos + next;
            life.Env.Add(LifeExists(newPos) ?? new Life(newPos, false));
        }

        foreach (var nearLife in life.Env)
        {
            if (nearLife.Env.Any(nearnearLife => nearnearLife.GetPos() == life.GetPos()))
            {
                continue;
            }
            nearLife.Env.Add(life);
        }
        life.DoHaveEnv = true;
    }
    public void GiveEnv2(Life life)
    {
        var pos = life.GetPos();
        life.Env = new List<Life>();
        foreach (var next in Directions)
        {
            var newPos = pos + next;
            life.Env.Add(LifeExists(newPos) ?? new Life(newPos, false,false));
        }
        Debug.Log("EnvCount==" + life.Env.Count);
        foreach (var nearLife in life.Env)
        {
            Debug.Log("nearLife.env.Count==" + nearLife.Env.Count);
            if (nearLife.Env.Any(nearnearLife => nearnearLife.GetPos() == life.GetPos()))
            {
                Debug.Log("FindOwn");
                continue;
            }
            nearLife.Env.Add(life);
        }
        life.DoHaveEnv = true;
    }
    public List<Vector3> Actives()
    {
        return (from life in Lives where life.IsActive() select life.GetPos()).ToList();
    }
    public void CallLookEnv()
    {
        foreach (var life in Lives)
        {
            life.LookEnv();
        }
    }

    public List<Life> Lives3;
    public void CallMove()
    { 
        var Lives2= Lives.ToList();
        Lives3=new List<Life>();
        foreach (var life in Lives2)
        {
            life.Move();
        }
        foreach (var life in Lives3)
        {
            life.Move();
            Lives.Add(life);
        }
        
    }
    private bool ExistLife(Vector3 pos)
    {
        return Lives.Any(life => life.GetPos() == pos);
    }
    public Life Put(Vector3 pos)
    {
        if (ExistLife(pos))
        {
            var life = Lives.Find(ele=>ele.GetPos() == pos);
            life.SetActive();
            return life;
        }
        return new Life(pos);
    }
}