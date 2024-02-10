using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;

public class Card_Player : Card
{
    [Title("�÷��̾� ����")]
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

    // �ڽ� ������Ʈ
    [Title("�÷��̾� ������Ʈ")]
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
        // �÷��̾� ��ȯ
        player = spawnManager.SpawnPlayer(gameManager.playerType, objTrans);

        // ���� ����
        hp = player.data.hp;
        mp = player.data.mp;
        defend = player.data.defend;
        dmg = 0;
        activeMP = player.data.skillMP;
        passiveCount = player.data.passiveCount;

        // ī�� UI ����
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
        Transform targetTrans = spawnManager.cardPos[_pos]; // Ÿ�� ��ġ �������� (�θ�)
        Card targetCard = spawnManager.FindCard(targetTrans.position); // Ÿ�� ī�� ��������

        // �ٸ� ī�� ���� & �̵�
        // �� ī�� �������� (�� ī�尡 �������� �ʴٸ�, �÷��̾� �̿� ī�� �� �ϳ� �������� (Ÿ�� ī�� ����))
        Card backCard = FindNeighbor(targetCard.PosToDir(pos)) ?? neighborCards.FirstOrDefault(card => card != targetCard);
        backCard.Move(pos); // �� ī�� �÷��̾� ��ġ�� �̵���Ű��
        spawnManager.DeSpawnCard(targetCard); // Ÿ�� ī�� ����

        // �̵� ����
        transform.SetParent(targetTrans); // �θ� ����
        // ���� ����
        isMoving = true;
        pos = _pos;

        // �̵� ��
        transform.DOMove(targetTrans.position, 0.5f).SetEase(Ease.OutBounce).SetUpdate(true).OnComplete(() => {
            // �̵� �Ϸ�
            transform.localPosition = Vector3.zero;
            isMoving = false;
            // ����ִ� ī�忡 �� ī�� ����
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
        hp += amount; // ü��ȸ��
        hp = Mathf.Min(hp, player.data.hp); // �ִ�ü�� Ȯ��
    }

    public void HealMP(int amount)
    {
        mp += amount; // ü��ȸ��
        mp = Mathf.Min(mp, player.data.mp); // �ִ�ü�� Ȯ��
    }

    public override void Damaged(int _amount)
    {
        // ��� ���
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
        // ���Ⱑ ���ٸ�
        if (string.IsNullOrEmpty(weaponData.name))
        {
            defaultDmg = Mathf.Min(hp, monster.hp);

            monster.Damaged(defaultDmg);
            Damaged(defaultDmg);
        }

        // ���Ⱑ �ִٸ�
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

        // ���� ����
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
