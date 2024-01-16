using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public abstract class Card : MonoBehaviour, IPoolObject
{
    public CardType type;

    public virtual void OnCreatedInPool()
    {
        name = name.Replace("(Clone)", "");
    }

    public abstract void OnGettingFromPool();
}

public enum CardType { Coin = 0, Item, Monster, Player, Trap }