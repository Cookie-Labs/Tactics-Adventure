using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public abstract class Monster : MonoBehaviour, IPoolObject
{
    public MonsterData data;

    [HideInInspector] public Animator anim;

    private CSVManager csvManager;

    public virtual void OnCreatedInPool()
    {
        name = name.Replace("(Clone)", "");

        anim = GetComponent<Animator>();

        csvManager = CSVManager.Instance;
    }

    public virtual void OnGettingFromPool()
    {

    }

    public virtual void SetMonster(string name)
    {
        data = csvManager.csvList.FindMonster(name);
    }
}

public enum MonsterType { Common = 3, CommonElite = 5, SubBoss = 10, Boss = 15 }