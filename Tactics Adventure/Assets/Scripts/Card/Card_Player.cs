using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class Card_Player : Card
{
    // 변수
    public int hp, mp, defend, dmg;
    public int poisonCount;

    // 자식 컴포넌트
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
        // 플레이어 소환
        player = spawnManager.SpawnPlayer(gameManager.playerType, objTrans);

        // 변수 설정
        hp = player.data.hp;
        mp = player.data.mp;
        defend = player.data.defend;
        dmg = 0;

        // 카드 UI 설정
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

    // 플레이어 위치에 따른 카드 설정
    public void SetTouch()
    {
        Card[] nearCard = FindNeighbors(new Direction[] { Direction.T, Direction.B, Direction.L, Direction.R });

        // 모든 카드 터치 비활성화
        foreach (Card card in spawnManager.cardList)
            card.availableTouch = false;

        // 인접한 카드 터치 활성화
        foreach (Card near in nearCard)
            near.availableTouch = true;

        availableTouch = true; // 자신은 항상 터치 활성화
    }

    public void HealHP(int amount)
    {
        hp += amount; // 체력회복
        hp = Mathf.Min(hp, player.data.hp); // 최대체력 확인

        SetUI($"<sprite=0>{dmg}  <sprite=1>{hp}");
    }

    public void HealMP(int amount)
    {
        mp += amount; // 체력회복
        mp = Mathf.Min(hp, player.data.mp); // 최대체력 확인

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
