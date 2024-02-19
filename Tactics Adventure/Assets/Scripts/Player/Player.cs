using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public abstract class Player : MonoBehaviour, IPoolObject
{
    public PlayerData data;

    // 내부 컴포넌트
    [HideInInspector] public Animator anim;
    // 외부 컴포넌트
    protected SpawnManager spawnManager;

    public virtual void OnCreatedInPool()
    {
        name = name.Replace("(Clone)", "");

        anim = GetComponent<Animator>();

        spawnManager = SpawnManager.Instance;
    }

    public virtual void OnGettingFromPool()
    {
    }

    public abstract void ActiveSkill();

    public abstract void PassiveSkill();
}

public enum PlayerType { Knight, Archer, Ninja, Magician }