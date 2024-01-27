using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;
using Sirenix.OdinInspector;
using TMPro;

public abstract class Card : MonoBehaviour, IPoolObject
{
    public CardType type;

    [Title("자식 컴포넌트")]
    public SpriteRenderer backGround;
    public Transform objTrans;
    public TextMeshPro cardName;
    public TextMeshPro uiText;

    // 외부 컴포넌트
    private SpriteData spriteData;

    // 매니저
    protected GameManager gameManager;
    protected SpawnManager spawnManager;
    protected CSVManager csvManager;

    public virtual void OnCreatedInPool()
    {
        name = name.Replace("(Clone)", "");

        // 외부 컴포넌트 불러오기
        spriteData = SpriteData.Instance;

        gameManager = GameManager.Instance;
        spawnManager = SpawnManager.Instance;
        csvManager = CSVManager.Instance;
    }

    public virtual void OnGettingFromPool()
    {
        backGround.sprite = spriteData.ExportRanStage();
        SetCard();
    }

    public abstract void SetCard();

    public abstract void DestroyCard();

    protected void SetCardName(string s)
    {
        cardName.text = s;
    }

    protected void SetUI(string s)
    {
        uiText.text = s;
    }
}

public enum CardType { Player, Chest, Coin, Consumable, Monster, Relics, Trap, Weapon }