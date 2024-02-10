using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;

public class Card_Player : Card
{
    [Title("플레이어 변수")]
    public int hp;
    public int mp;
    public int defend;
    public int dmg;
    public WeaponData weaponData;
    public int poisonCount;
    public bool isMoving;
    public Card[] neighborCards;
    public int activeMP;
    public int passiveCount;

    // 자식 컴포넌트
    [Title("플레이어 컴포넌트")]
    public TextMeshPro iconTxt;
    [HideInInspector] public Player player;

    private UIManager uiManager;

    public override void OnCreatedInPool()
    {
        base.OnCreatedInPool();
        uiManager = UIManager.Instance;
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
        activeMP = player.data.skillMP;
        passiveCount = player.data.passiveCount;

        // 카드 UI 설정
        SetCardName(player.data.name);
        SetIconTxt();
    }

    public override void DestroyCard()
    {
        spawnManager.DeSpawnPlayer(player);
        DODestroy();
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

        // 이동 중
        transform.DOMove(targetTrans.position, 0.5f).SetEase(Ease.OutBounce).SetUpdate(true).OnComplete(() => {
            // 이동 완료
            transform.localPosition = Vector3.zero;
            isMoving = false;
            // 비어있는 카드에 새 카드 생성
            spawnManager.SpawnRanCard();
        });
    }

    public void Active()
    {
        if (mp < activeMP)
            return;

        mp -= activeMP;
        player.ActiveSkill();

        SetIconTxt();
    }

    public void Passive()
    {
        passiveCount--;

        if(passiveCount <= 0)
        {
            player.PassiveSkill();
            passiveCount = player.data.passiveCount;
        }
    }

    public void SetNeighbor()
    {
        neighborCards = FindNeighbors(new Direction[] { Direction.T, Direction.B, Direction.L, Direction.R });
    }

    public override void DoCard()
    {
    }

    public override void DoTurnCard()
    {
        Passive();

        uiManager.CheckSkillUI();
        SetIconTxt();
    }

    public void HealHP(int amount)
    {
        hp += amount; // 체력회복
        hp = Mathf.Min(hp, player.data.hp); // 최대체력 확인
    }

    public void HealMP(int amount)
    {
        mp += amount; // 체력회복
        mp = Mathf.Min(mp, player.data.mp); // 최대체력 확인
    }

    public override void Damaged(int _amount)
    {
        // 방어 계산
        if (defend > 0)
        {
            defend -= _amount;
            _amount = Mathf.Max(0, -defend);
            defend = Mathf.Max(0, defend);
        }

        hp = Mathf.Max(0, hp - _amount);

        if (hp <= 0)
        {
            // Die
        }

        DODamaged();
    }

    public void Atk(Card_Monster monster)
    {
        int defaultDmg;
        // 무기가 없다면
        if (string.IsNullOrEmpty(weaponData.name))
        {
            defaultDmg = Mathf.Min(hp, monster.hp);

            monster.Damaged(defaultDmg);
            Damaged(defaultDmg);
        }

        // 무기가 있다면
        else
        {
            defaultDmg = Mathf.Min(dmg, monster.hp);

            monster.Damaged(defaultDmg);
            dmg -= defaultDmg;

            if (dmg <= 0)
                weaponData = new WeaponData();
        }
    }

    public void EquipWeapon(Card_Weapon weaponCard)
    {
        dmg = weaponCard.dmg;

        // 무기 장착
        weaponData = weaponCard.weapon.data;
    }

    public void EquipWeapon(int ID, int _dmg)
    {
        dmg = _dmg;

        weaponData = csvManager.csvList.FindWeapon(ID);
    }

    public void Poisoned()
    {
        if (poisonCount <= 0)
            return;

        Damaged(1);
        poisonCount--;
    }

    private void SetIconTxt()
    {
        uiText.text = $"<sprite=0>{dmg}  <sprite=1>{hp}";
        iconTxt.text = $"<sprite=2>{defend}  <sprite=3>{mp}";
    }
}
