using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class Card_Player : Card
{
    [Title("자식 변수")]
    public int hp, mp, defend, dmg;
    public Weapon weapon;
    public int poisonCount;
    public bool isMoving;
    public Card[] neighborCards;

    // 자식 컴포넌트
    private Player player;

    public override void OnCreatedInPool()
    {
        base.OnCreatedInPool();
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
        SetUI($"<sprite=0>{dmg}  <sprite=1>{hp}");
    }

    public override void DestroyCard()
    {
        spawnManager.DeSpawnPlayer(player);
    }

    public override void Move(int _pos)
    {
        Transform targetTrans = spawnManager.cardPos[_pos]; // 타겟 위치 가져오기 (부모)
        Card targetCard = spawnManager.FindCard(targetTrans.position); // 타겟 카드 가져오기

        // 다른 카드 제거 & 이동
        // 뒷 카드 가져오기 (뒷 카드가 존재하지 않다면, 플레이어 이웃 카드 중 하나 가져오기 (타겟 카드 제외))
        Card backCard = FindNeighbor(targetCard.PosToDir(pos)) ?? neighborCards.FirstOrDefault(card => card != targetCard);
        backCard.Move(pos); // 뒷 카드 플레이어 위치로 이동시키기
        spawnManager.DeSpawnCard(targetCard); // 타겟 카드 삭제

        // 이동 시작
        transform.SetParent(targetTrans); // 부모 설정
        // 변수 설정
        isMoving = true;
        pos = _pos;
        Anim(AnimID.Walk); // 애니메이션(걷기)

        // 이동 중
        transform.DOMove(targetTrans.position, 1f).SetEase(Ease.OutBounce).OnComplete(() => {
            // 이동 완료
            transform.localPosition = Vector3.zero;
            isMoving = false;
            Anim(AnimID.Idle);
            // 비어있는 카드에 새 카드 생성
            spawnManager.SpawnRanCard();
        });
    }

    public void SetNeighbor()
    {
        neighborCards = FindNeighbors(new Direction[] { Direction.T, Direction.B, Direction.L, Direction.R });
    }

    public override void DoCard()
    {
    }

    public override void Anim(AnimID id)
    {
        player.SetAnim((int)id);
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

    public override void Damaged(int _amount)
    {
        hp -= _amount;
        
        if(hp <= 0)
        {
            // Die
            hp = 0;
        }

        SetUI($"<sprite=0>{dmg}  <sprite=1>{hp}");
    }

    public void Atk(Card_Monster monster)
    {
        int defaultDmg;
        // Dmg가 0이하라면 체력 공격
        if (dmg <= 0)
        {
            defaultDmg = Mathf.Min(hp, monster.hp);

            monster.Damaged(defaultDmg);
            Damaged(defaultDmg);
        }

        // Dmg가 1이상이면 무기 공격
        else
        {
            defaultDmg = Mathf.Min(dmg, monster.hp);

            monster.Damaged(defaultDmg);
            dmg -= defaultDmg;

        }
        SetUI($"<sprite=0>{dmg}  <sprite=1>{hp}");
    }

    public void EquipWeapon(Card_Weapon weaponCard)
    {
        // 무기 장착
        weapon = weaponCard.weapon;

        // 변수 설정
        dmg = weaponCard.dmg;

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
