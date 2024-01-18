using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;
using Sirenix.OdinInspector;

public abstract class Card : MonoBehaviour, IPoolObject
{
    public CardType type;

    [Title("�ڽ� ������Ʈ")]
    public SpriteRenderer backGround;
    public Transform[] childTrans;

    // �ܺ� ������Ʈ
    private SpriteData spriteData;

    // �Ŵ���
    protected GameManager gameManager;
    protected SpawnManager spawnManager;

    public virtual void OnCreatedInPool()
    {
        name = name.Replace("(Clone)", "");

        // �ܺ� ������Ʈ �ҷ�����
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