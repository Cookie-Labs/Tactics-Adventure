using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;
using Sirenix.OdinInspector;

public abstract class Card : MonoBehaviour, IPoolObject
{
    public CardType type;

    [Title("자식 컴포넌트")]
    public SpriteRenderer backGround;
    public Transform[] childTrans;

    // 외부 컴포넌트
    private SpriteData spriteData;

    // 매니저
    protected GameManager gameManager;
    protected SpawnManager spawnManager;

    public virtual void OnCreatedInPool()
    {
        name = name.Replace("(Clone)", "");

        // 외부 컴포넌트 불러오기
        spriteData = SpriteData.Instance;

        gameManager = GameManager.Instance;
        spawnManager = SpawnManager.Instance;
    }

    public virtual void OnGettingFromPool()
    {
        backGround.sprite = spriteData.ExportRanStage();
        SetCard();
    }

    public abstract void SetCard();
}

public enum CardType { Coin = 0, Item, Monster, Player, Trap }