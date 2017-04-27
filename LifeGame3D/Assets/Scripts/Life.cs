using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine;
//using UnityEngine.Video;

public class Life {
    private Vector3 pos { get; set; }
    public Vector3 Pos()
    {
        return pos;
    }
    public List<Life> env { get; set; }
    public bool DoHaveEnv { get; set; }
    private bool active { get; set; }
    public bool isActive()
    {
        return active;
    }
    private bool isGood;
    private static Vector2 MaxMin { get; set; }
    public static World world { get; set; }
    private static void WorldInitializer()
    {//ワールドを初期化
        World.WorldInitializer();
        world=new World();
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
        env=new List<Life>();
        this.pos = pos;
        DoHaveEnv = false;
        active = true;
        world.Add(this);
        world.GiveEnv(this);
    }
    public Life(Vector3 pos, bool active)
    {//ワールド専用のコンストラクタ
        env=new List<Life>();
        this.pos = pos;
        DoHaveEnv = false;
        this.active = active;
        world.Add(this);
    }
    public Life(Vector3 pos, bool active,bool hoge)
    {//急遽作られた謎のコンストラクタ
        env = new List<Life>();
        this.pos = pos;
        DoHaveEnv = false;
        this.active = active;
        world.Lives3.Add(this);
    }
    private int EnvState()
    {//Envirnmentの中でアクティブなライフの数
        return env.Count(next=>next.isActive());
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

    public void move()
    {
        if (active)
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
        active = false;
    }
    private void Born()
    {
        active = true;
        if(DoHaveEnv)return;
        world.GiveEnv(this);
    }
    public void SetActive()
    {
        active = true;
        if (DoHaveEnv) return;
        world.GiveEnv(this);
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
    private Life isExist(Vector3 pos)
    {
        return Lives.Find(elem => elem.Pos() == pos);
    }
    public void Add(Life life)
    {
        Lives.Add(life);
    }
    public void GiveEnv(Life life)
    {
        var pos = life.Pos();
        life.env=new List<Life>();
        foreach (var next in Directions)
        {
            var newPos = pos + next;
            life.env.Add(isExist(newPos) ?? new Life(newPos, false));
        }
        Debug.Log("EnvCount=="+life.env.Count);
        foreach (var nearLife in life.env)
        {
            Debug.Log("nearLife.env.Count=="+nearLife.env.Count);
            if (nearLife.env.Any(nearnearLife => nearnearLife.Pos() == life.Pos()))
            {
                Debug.Log("FindOwn");
                continue;
            }
            nearLife.env.Add(life);
        }
        life.DoHaveEnv = true;
    }
    public void GiveEnv2(Life life)
    {
        var pos = life.Pos();
        life.env = new List<Life>();
        foreach (var next in Directions)
        {
            var newPos = pos + next;
            life.env.Add(isExist(newPos) ?? new Life(newPos, false,false));
        }
        Debug.Log("EnvCount==" + life.env.Count);
        foreach (var nearLife in life.env)
        {
            Debug.Log("nearLife.env.Count==" + nearLife.env.Count);
            if (nearLife.env.Any(nearnearLife => nearnearLife.Pos() == life.Pos()))
            {
                Debug.Log("FindOwn");
                continue;
            }
            nearLife.env.Add(life);
        }
        life.DoHaveEnv = true;
    }
    public List<Vector3> Actives()
    {
        return (from life in Lives where life.isActive() select life.Pos()).ToList();
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
            life.move();
        }
        foreach (var life in Lives3)
        {
            life.move();
            Lives.Add(life);
        }
        
    }
    private bool ExistLife(Vector3 pos)
    {
        return Lives.Any(life => life.Pos() == pos);
    }
    public Life Put(Vector3 pos)
    {
        if (ExistLife(pos))
        {
            var life = Lives.Find(ele=>ele.Pos()==pos);
            life.SetActive();
            return life;
        }
        return new Life(pos);
    }
}