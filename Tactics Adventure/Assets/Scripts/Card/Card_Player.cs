using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class Card_Player : Card
{
    [Title("�ڽ� ����")]
    public int hp, mp, defend, dmg;
    public Weapon weapon;
    public int poisonCount;
    public bool isMoving;
    public Card[] neighborCards;

    // �ڽ� ������Ʈ
    private Player player;

    public override void OnCreatedInPool()
    {
        base.OnCreatedInPool();
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
        SetUI($"<sprite=0>{dmg}  <sprite=1>{hp}");
    }

    public override void DestroyCard()
    {
        spawnManager.DeSpawnPlayer(player);
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
        Anim(AnimID.Walk); // �ִϸ��̼�(�ȱ�)

        // �̵� ��
        transform.DOMove(targetTrans.position, 1f).SetEase(Ease.OutBounce).OnComplete(() => {
            // �̵� �Ϸ�
            transform.localPosition = Vector3.zero;
            isMoving = false;
            Anim(AnimID.Idle);
            // ����ִ� ī�忡 �� ī�� ����
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
        // Dmg�� 0���϶�� ü�� ����
        if (dmg <= 0)
        {
            defaultDmg = Mathf.Min(hp, monster.hp);

            monster.Damaged(defaultDmg);
            Damaged(defaultDmg);
        }

        // Dmg�� 1�̻��̸� ���� ����
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
        // ���� ����
        weapon = weaponCard.weapon;

        // ���� ����
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
