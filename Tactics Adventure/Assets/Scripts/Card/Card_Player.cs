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

    public override IEnumerator Move(int _pos)
    {
        Transform targetTrans = spawnManager.cardPos[_pos]; // Ÿ�� ��ġ �������� (�θ�)
        Card targetCard = spawnManager.FindCard(targetTrans.position); // Ÿ�� ī�� ��������

        // �ٸ� ī�� ���� & �̵�
        // �� ī�� �������� (�� ī�尡 �������� �ʴٸ�, �÷��̾� �̿� ī�� �� �ϳ� �������� (Ÿ�� ī�� ����))
        Card backCard = FindNeighbor(targetCard.PosToDir(pos)) ?? neighborCards.FirstOrDefault(card => card != targetCard);
        StartCoroutine(backCard.Move(pos)); // �� ī�� �÷��̾� ��ġ�� �̵���Ű��
        spawnManager.DeSpawnCard(targetCard); // Ÿ�� ī�� ����

        // �̵� ����
        transform.SetParent(targetTrans); // �θ� ����
        // ���� ����
        isMoving = true;
        pos = _pos;
        SetAnim(player.anim, AnimID.Walk);
        yield return new WaitForEndOfFrame();

        // �̵� ��
        transform.DOMove(targetTrans.position, 0.5f).SetEase(Ease.OutBounce).SetUpdate(true).OnComplete(() => {
            // �̵� �Ϸ�
            transform.localPosition = Vector3.zero;
            isMoving = false;
            // ����ִ� ī�忡 �� ī�� ����
            spawnManager.SpawnRanCard();
        });

        yield return new WaitForSeconds(0.5f);
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

    public override IEnumerator DoCard()
    {
        yield return new WaitForSeconds(0.1f);
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

    private IEnumerator Die()
    {
        hp = 0;

        SetAnim(player.anim, AnimID.Die);
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(player.anim.GetCurrentAnimatorStateInfo(0).length);

        Debug.Log("���� ����");
    }

    public override IEnumerator Damaged(int _amount)
    {
        // ��� ���
        if (defend > 0)
        {
            defend -= _amount;
            _amount = Mathf.Max(0, -defend);
            defend = Mathf.Max(0, defend);
        }

        hp = Mathf.Max(0, hp - _amount);

        DODamaged();
        SetIconTxt();

        // �ǰ� �ִϸ��̼� (������ ����)
        SetAnim(player.anim, AnimID.Damaged);
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(player.anim.GetCurrentAnimatorStateInfo(0).length);

        if (hp <= 0)
        {
            yield return Die();
        }
    }

    public IEnumerator Atk(Card_Monster monster)
    {
        int defaultDmg;
        // ���Ⱑ ���ٸ�
        if (string.IsNullOrEmpty(weaponData.name))
        {
            defaultDmg = Mathf.Min(hp, monster.hp);

            StartCoroutine(Damaged(defaultDmg));

            yield return monster.Atk(defaultDmg);
        }

        // ���Ⱑ �ִٸ�
        else
        {
            defaultDmg = Mathf.Min(dmg, monster.hp);

            dmg -= defaultDmg;

            if (dmg <= 0) // ���� ����
                weaponData = new WeaponData();

            SetIconTxt();

            // ���� �ִϸ��̼�
            SetAnim(player.anim, AnimID.Atk);

            yield return monster.Damaged(defaultDmg);
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
