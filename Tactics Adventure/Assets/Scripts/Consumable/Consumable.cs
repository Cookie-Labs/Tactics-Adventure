using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public abstract class Consumable : MonoBehaviour, IPoolObject
{
    public string consumableName;
    public ConsumableType type;

    // ���� ������Ʈ
    protected SpriteRenderer spriteRenderer;

    // �Ŵ���
    protected SpriteData spriteData;

    public virtual void OnCreatedInPool()
    {
        name = name.Replace("(Clone)", "");

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteData = SpriteData.Instance;
    }

    public virtual void OnGettingFromPool()
    {

    }
}

public enum ConsumableType { Portion }