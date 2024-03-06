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
    public int curHand;
    public WeaponData[] equipWeapon = new WeaponData[2];
    public Card[] neighborCards;
    public int poisonCount;
    public int activeMP;
    public int passiveCount;
    public bool isTalking;

    [Title("Ư��", "���� ����")]
    public int bonusDmg;
    public bool isLotto;

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
        curHand = 0;
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
        pos = _pos;
        yield return SetAnim(player.anim, AnimID.Walk);

        // �̵� ��
        transform.DOMove(targetTrans.position, 0.5f).SetEase(Ease.OutBounce).SetUpdate(true).OnComplete(() => {
            // �̵� �Ϸ�
            transform.localPosition = Vector3.zero;
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

        SetActive(true);
        foreach (Card card in neighborCards)
            card.SetActive(true);
    }

    public override IEnumerator DoCard()
    {
        yield return Talk(csvManager.csvList.ExportExplain_Ran(CardType.Player), 1.5f);
    }

    public override void DoTurnCard()
    {
        Passive();
        Poisoned();

        uiManager.handUI.HandImgUI();
        uiManager.handUI.WeaponIconUI();
        SetIconTxt();
    }

    public void SetMaxHP(int max)
    {
        player.data.hp = max;

        hp = Mathf.Min(player.data.hp, hp);
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

    public void UpDefend(int amount)
    {
        defend += amount;
    }

    private IEnumerator Die()
    {
        hp = 0;

        yield return SetAnim(player.anim, AnimID.Die);
        yield return new WaitForSeconds(animTime);

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
        StartCoroutine(Talk("�ƾ�!", 0.5f));

        // �ǰ� �ִϸ��̼� (������ ����)
        yield return SetAnim(player.anim, AnimID.Damaged);
        yield return new WaitForSeconds(animTime);

        if (hp <= 0)
        {
            yield return Die();
        }
    }

    public IEnumerator Atk(Card_Monster monster)
    {
        int defaultDmg;
        // ���Ⱑ ���ٸ�
        if (equipWeapon[curHand].plus.dmg == 0)
        {
            defaultDmg = Mathf.Min(hp, monster.hp);

            StartCoroutine(Damaged(defaultDmg));

            yield return monster.Atk(defaultDmg);
        }

        // ���Ⱑ �ִٸ�
        else
        {
            defaultDmg = Mathf.Min(equipWeapon[curHand].plus.dmg, monster.hp);

            equipWeapon[curHand].plus.dmg -= defaultDmg;

            int totalDmg = defaultDmg + bonusDmg;

            // ����� ���
            if (csvManager.csvList.EnforceCheck(equipWeapon[curHand], EnforceID.Drain))
                HealHP(totalDmg);

            if (equipWeapon[curHand].plus.dmg <= 0) // ���� ����
                equipWeapon[curHand] = new WeaponData();

            SetIconTxt();

            // ���� �ִϸ��̼�
            yield return SetAnim(player.anim, AnimID.Atk);
            StartCoroutine(Talk("�׾��!!", 1f));

            yield return monster.Damaged(totalDmg);
        }
    }

    public ref WeaponData GetEquipWeapon()
    {
        // ���� �տ� ���� O && �ٸ� �տ� ���� X
        if (equipWeapon[curHand].plus.dmg != 0 && equipWeapon[(curHand + 1) % 2].plus.dmg == 0)
            return ref equipWeapon[(curHand + 1) % 2]; // �ٸ� �� return

        return ref equipWeapon[curHand]; // ���� �� return
    }

    public void EquipWeapon(Card_Weapon weaponCard)
    {
        GetEquipWeapon() = weaponCard.weapon.data;
    }

    public void EquipWeapon(int ID)
    {
        WeaponData newWeapon = csvManager.csvList.FindWeapon(ID);
        newWeapon.plus.dmg = csvManager.luck.TierToDmg(newWeapon.tier);

        GetEquipWeapon() = newWeapon;
    }

    public void ChangeHand(int _number)
    {
        if (touchManager.isTouching)
            return;

        curHand = _number;

        SetIconTxt();
        uiManager.handUI.HandAlphaUI();
    }

    public void UpDmg(int dmg)
    {
        if (equipWeapon[curHand].plus.dmg != 0)
            equipWeapon[curHand].plus.dmg += dmg;
        else if (equipWeapon[(curHand + 1) % 2].plus.dmg != 0)
            equipWeapon[(curHand + 1) % 2].plus.dmg += dmg;
        else return;
    }

    public void GetPoison(int i)
    {
        poisonCount = i;
    }

    public void Poisoned()
    {
        if (poisonCount <= 0)
            return;

        Damaged(1);
        poisonCount--;
    }

    private Sequence talkSeq;
    public IEnumerator Talk(string explain, float time)
    {
        if (talkSeq != null && talkSeq.IsActive())
            talkSeq.Kill();

        isTalking = true;

        cardName.SetText("");

        talkSeq = DOTween.Sequence().SetUpdate(true);
        talkSeq.Append(cardName.DOText(explain, time))
            .AppendInterval(0.5f)
            .OnComplete(() =>
            {
                cardName.SetText(player.data.name);
                isTalking = false;
            });

        yield return new WaitForSeconds(time + 0.5f);
    }

    private void SetIconTxt()
    {
        if(equipWeapon[curHand].plus.dmg != 0 && bonusDmg > 0)
            uiText.text = $"<sprite=0>{equipWeapon[curHand].plus.dmg}<color=orange>+{bonusDmg}</color> <sprite=1>{hp}";
        else
            uiText.text = $"<sprite=0>{equipWeapon[curHand].plus.dmg} <sprite=1>{hp}";

        iconTxt.text = $"<sprite=2>{defend}  <sprite=3>{mp}";
    }
}

