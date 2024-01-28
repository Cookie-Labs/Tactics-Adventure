using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;
using Sirenix.OdinInspector;
using TMPro;
using DG.Tweening;

public abstract class Card : MonoBehaviour, IPoolObject
{
    [Title("변수")]
    public int pos;
    public CardType type;
    public bool availableTouch;
    public bool isTurn; // 턴제 확인

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

    protected void OnMouseUp()
    {
        if (!availableTouch || gameManager.isMoving)
            return;
        DoCard();
        spawnManager.DoTurnCards();
        spawnManager.playerCard.SetTouch();
    }

    public abstract void SetCard();

    public abstract void DestroyCard();

    public abstract void DoCard();

    public virtual void DoTurnCard()
    {

    }

    public virtual void Move(int pos)
    {
        Transform target = spawnManager.cardPos[pos];

        transform.DOMove(target.position, 0.5f).SetEase(Ease.OutBounce).OnComplete(() => {
            transform.SetParent(target);
            transform.localPosition = Vector3.zero;
        });
    }

    public abstract void Damaged(int _amount);

    public void SetTurnCard()
    {
        spawnManager.turnCardList.Add(this);
        isTurn = true;
    }

    // UI
    protected void SetCardName(string s)
    {
        cardName.text = s;
    }

    protected void SetUI(string s)
    {
        uiText.text = s;
    }

    // 이웃 찾기
    protected Vector2 DirToPos(Direction dir)
    {
        Vector2 targetPos = transform.position;

        switch (dir)
        {
            case Direction.T:
                targetPos += new Vector2(0, 2.4f);
                break;
            case Direction.B:
                targetPos += new Vector2(0, -2.4f);
                break;
            case Direction.L:
                targetPos += new Vector2(-1.8f, 0);
                break;
            case Direction.R:
                targetPos += new Vector2(1.8f, 0);
                break;
        }

        return targetPos;
    }

    protected Card FindNeighbor(Direction dir)
    {
        return spawnManager.FindCard(DirToPos(dir));
    }

    protected Card[] FindNeighbors(Direction[] dir)
    {
        int length = dir.Length;
        Vector3[] poses = new Vector3[length];

        for(int i = 0; i < length; i++)
            poses[i] = DirToPos(dir[i]);

        return spawnManager.FindCards(poses);
    }
}

public enum CardType { Player, Chest, Coin, Consumable, Monster, Relics, Trap, Weapon }
public enum Direction { T, B, L, R }