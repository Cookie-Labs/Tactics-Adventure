using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class Card_Player : Card
{
    // ����
    public int hp, mp, defend, dmg;
    public int poisonCount;

    // �ڽ� ������Ʈ
    private Player player;

    public override void OnCreatedInPool()
    {
        base.OnCreatedInPool();
    }

    public override void OnGettingFromPool()
    {
        base.OnGettingFromPool();
    }

    public override void SetCard()
    {
        // �÷��̾� ��ȯ
        player = spawnManager.SpawnPlayer(gameManager.playerType, objTrans);

        // ���� ����
        hp = player.data.hp;
        mp = player.data.mp;
        defend = player.data.defend;
        dmg = 0;

        // ī�� UI ����
        SetCardName(player.data.name);
        Damaged(5);
        SetUI($"<sprite=0>{dmg}  <sprite=1>{hp}");
    }

    public override void DestroyCard()
    {
        spawnManager.DeSpawnPlayer(player);
    }

    public override void Move(int pos)
    {
        Transform target = spawnManager.cardPos[pos];
        gameManager.isMoving = true;

        transform.DOMove(target.position, 0.5f).SetEase(Ease.OutBounce).OnComplete(() => {
            transform.SetParent(target);
            transform.localPosition = Vector3.zero;
            SetTouch();
            gameManager.isMoving = false;
        });
    }

    public override void DoCard()
    {
    }

    // �÷��̾� ��ġ�� ���� ī�� ����
    public void SetTouch()
    {
        Card[] nearCard = FindNeighbors(new Direction[] { Direction.T, Direction.B, Direction.L, Direction.R });

        // ��� ī�� ��ġ ��Ȱ��ȭ
        foreach (Card card in spawnManager.cardList)
            card.availableTouch = false;

        // ������ ī�� ��ġ Ȱ��ȭ
        foreach (Card near in nearCard)
            near.availableTouch = true;

        availableTouch = true; // �ڽ��� �׻� ��ġ Ȱ��ȭ
    }

    public void HealHP(int amount)
    {
        hp += amount; // ü��ȸ��
        hp = Mathf.Min(hp, player.data.hp); // �ִ�ü�� Ȯ��

        SetUI($"<sprite=0>{dmg}  <sprite=1>{hp}");
    }

    public void HealMP(int amount)
    {
        mp += amount; // ü��ȸ��
        mp = Mathf.Min(hp, player.data.mp); // �ִ�ü�� Ȯ��

        SetUI($"<sprite=0>{dmg}  <sprite=1>{hp}");
    }

    public void Damaged(int amount)
    {
        hp -= amount;
        
        if(hp <= 0)
        {
            // Die
            hp = 0;
        }

        SetUI($"<sprite=0>{dmg}  <sprite=1>{hp}");
    }

    public void Poisoned()
    {
        if (poisonCount <= 0)
            return;

        Damaged(1);
        poisonCount--;

        SetUI($"<sprite=0>{dmg}  <sprite=1>{hp}");
    }
}
