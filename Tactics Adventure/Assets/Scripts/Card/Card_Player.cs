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
        SetUI($"<sprite=0>{dmg}  <sprite=1>{hp}");
    }

    public override void DestroyCard()
    {
        spawnManager.DeSpawnPlayer(player);
    }

    public override void Move(int pos)
    {
        Transform target = spawnManager.cardPos[pos];
        transform.SetParent(target);
        isMoving = true;
        Anim(AnimID.Walk); // 애니메이션(걷기)

        transform.DOMove(target.position, 1f).SetEase(Ease.OutBounce).OnComplete(() => {
            transform.localPosition = Vector3.zero;
            isMoving = false;
            Anim(AnimID.Idle);
        });
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
