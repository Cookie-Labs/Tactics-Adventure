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
    public int curHand;
    public EquipWeapon[] equipWeapon = new EquipWeapon[2];
    public Card[] neighborCards;
    public int poisonCount;
    public int activeMP;
    public int passiveCount;
    public bool isTalking;

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
        curHand = 0;
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

    public override IEnumerator Move(int _pos)
    {
        Transform targetTrans = spawnManager.cardPos[_pos]; // 타겟 위치 가져오기 (부모)
        Card targetCard = spawnManager.FindCard(targetTrans.position); // 타겟 카드 가져오기

        // 다른 카드 제거 & 이동
        // 뒷 카드 가져오기 (뒷 카드가 존재하지 않다면, 플레이어 이웃 카드 중 하나 가져오기 (타겟 카드 제외))
        Card backCard = FindNeighbor(targetCard.PosToDir(pos)) ?? neighborCards.FirstOrDefault(card => card != targetCard);
        StartCoroutine(backCard.Move(pos)); // 뒷 카드 플레이어 위치로 이동시키기
        spawnManager.DeSpawnCard(targetCard); // 타겟 카드 삭제

        // 이동 시작
        transform.SetParent(targetTrans); // 부모 설정
        // 변수 설정
        pos = _pos;
        yield return SetAnim(player.anim, AnimID.Walk);

        // 이동 중
        transform.DOMove(targetTrans.position, 0.5f).SetEase(Ease.OutBounce).SetUpdate(true).OnComplete(() => {
            // 이동 완료
            transform.localPosition = Vector3.zero;
            // 비어있는 카드에 새 카드 생성
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
        hp += amount; // 체력회복
        hp = Mathf.Min(hp, player.data.hp); // 최대체력 확인
    }

    public void HealMP(int amount)
    {
        mp += amount; // 체력회복
        mp = Mathf.Min(mp, player.data.mp); // 최대체력 확인
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

        Debug.Log("게임 종료");
    }

    public override IEnumerator Damaged(int _amount)
    {
        // 방어 계산
        if (defend > 0)
        {
            defend -= _amount;
            _amount = Mathf.Max(0, -defend);
            defend = Mathf.Max(0, defend);
        }

        hp = Mathf.Max(0, hp - _amount);

        DODamaged();
        SetIconTxt();
        StartCoroutine(Talk("아얏!", 0.5f));

        // 피격 애니메이션 (딜레이 포함)
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
        // 무기가 없다면
        if (equipWeapon[curHand].dmg == 0)
        {
            defaultDmg = Mathf.Min(hp, monster.hp);

            StartCoroutine(Damaged(defaultDmg));

            yield return monster.Atk(defaultDmg);
        }

        // 무기가 있다면
        else
        {
            defaultDmg = Mathf.Min(equipWeapon[curHand].dmg, monster.hp);

            equipWeapon[curHand].dmg -= defaultDmg;

            if (equipWeapon[curHand].dmg <= 0) // 무기 깨짐
                equipWeapon[curHand] = new EquipWeapon();

            SetIconTxt();

            // 공격 애니메이션
            yield return SetAnim(player.anim, AnimID.Atk);
            StartCoroutine(Talk("죽어라!!", 1f));

            yield return monster.Damaged(defaultDmg);
        }
    }

    public void EquipWeapon(Card_Weapon weaponCard)
    {
        // 현재 손에 무기 X || 모든 손에 무기
        if(equipWeapon[curHand].dmg == 0 || (equipWeapon[0].dmg != 0 && equipWeapon[1].dmg != 0))
            equipWeapon[curHand] = new EquipWeapon(weaponCard.dmg, weaponCard.weapon.data); // 현재 손에 무기 장착

        // 다른 손에 무기 X
        else if (equipWeapon[(curHand + 1) % 2].dmg == 0)
            equipWeapon[(curHand + 1) % 2] = new EquipWeapon(weaponCard.dmg, weaponCard.weapon.data); // 다른 손에 무기 장착
    }

    public void EquipWeapon(int ID, int _dmg)
    {
        WeaponData newWeapon = csvManager.csvList.FindWeapon(ID);
        // 현재 손에 무기 X || 모든 손에 무기
        if (equipWeapon[curHand].dmg == 0 || (equipWeapon[0].dmg != 0 && equipWeapon[1].dmg != 0))
            equipWeapon[curHand] = new EquipWeapon(_dmg, newWeapon); // 현재 손에 무기 장착

        // 다른 손에 무기 X
        else if(equipWeapon[(curHand + 1) % 2].dmg == 0)
            equipWeapon[(curHand + 1) % 2] = new EquipWeapon(_dmg, newWeapon); // 다른 손에 무기 장착
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
        if (equipWeapon[curHand].dmg != 0)
            equipWeapon[curHand].dmg += dmg;
        else if (equipWeapon[(curHand + 1) % 2].dmg != 0)
            equipWeapon[(curHand + 1) % 2].dmg += dmg;
        else return;
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
        uiText.text = $"<sprite=0>{equipWeapon[curHand].dmg}  <sprite=1>{hp}";
        iconTxt.text = $"<sprite=2>{defend}  <sprite=3>{mp}";
    }
}

