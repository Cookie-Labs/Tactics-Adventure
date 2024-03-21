using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using System;

public class UIManager : Singleton<UIManager>
{
    [Title("공통UI")]
    public bool isUI;
    public GameObject raycastPannel;

    [Title("스킬 버튼")]
    public SkillUI skillUI;

    [Title("돈 UI")]
    public TextMeshProUGUI moneyTxt;

    [Title("손 UI")]
    public HandUI handUI;

    [Title("경험치바 UI")]
    public ExpBar expBar;

    [Title("패널")]
    public BagPannel bagPannel;
    public StatPannel statPannel;

    public void CheckSkillUI()
    {
        Card_Player playerCard = SpawnManager.Instance.playerCard;

        // 액티브 스킬
        skillUI.EnableActive(playerCard.mp >= playerCard.activeMP || playerCard.freeMP > 0);

        // 패시브 스킬
        skillUI.PassiveUI();
    }

    public void MoneyTxt(int amount)
    {
        moneyTxt.text = $"{amount} <sprite=4>";
    }
}

[Serializable]
public class SkillUI
{
    public Button activeBtn;
    public TextMeshProUGUI passiveCountTxt;
    public Image[] skillCool; // 0: Active, 1 : Passive

    public void EnableActive(bool isActive)
    {
        if(isActive)
        {
            activeBtn.interactable = true;
            skillCool[0].DOFade(0f, 0.2f).SetUpdate(true);
        }
        else
        {
            activeBtn.interactable = false;
            skillCool[0].DOFade(1f, 0.2f).SetUpdate(true);
        }
    }

    public void PassiveUI()
    {
        Card_Player playerCard = SpawnManager.Instance.playerCard;

        passiveCountTxt.text = playerCard.passiveCount.ToString();

        Sequence passiveSeq = DOTween.Sequence().SetUpdate(true);
        passiveSeq.Append(skillCool[1].DOFillAmount((float)playerCard.passiveCount / playerCard.player.data.passiveCount, 0.2f))
            .Join(passiveCountTxt.transform.DOScale(2f, 0.2f))
            .Append(passiveCountTxt.transform.DOScale(1f, 0.1f));
    }
}

[Serializable]
public class HandUI
{
    // 0 : LEFT,    1 : RIGHT
    public Image[] handImg;
    public CanvasGroup[] handAlpha;
    public Image[] weaponIcon;

    // 손 이미지 설정
    public void HandImgUI()
    {
        SpriteData spriteData = SpriteData.Instance;
        Card_Player player = SpawnManager.Instance.playerCard;

        for(int i = 0; i < 2; i++)
        {
            Sprite handSprite = (player.equipWeapon[i].plus.dmg == 0) ? spriteData.handSprites[i + 2] : spriteData.handSprites[i];
            handImg[i].sprite = handSprite;
        }
    }

    // 손 투명도 설정
    public void HandAlphaUI()
    {
        int curHand = SpawnManager.Instance.playerCard.curHand;

        for (int i = 0; i < 2; i++)
            handAlpha[i].alpha = (curHand == i) ? 1f : 0.5f;
    }

    // 무기 설정
    public void WeaponIconUI()
    {
        SpriteData spriteData = SpriteData.Instance;
        Card_Player player = SpawnManager.Instance.playerCard;

        for (int i = 0; i < 2; i++)
        {
            if (player.equipWeapon[i].plus.dmg == 0)
            {
                weaponIcon[i].gameObject.SetActive(false);
                weaponIcon[i].sprite = null;
            }
            
            else
            {
                weaponIcon[i].gameObject.SetActive(true);
                weaponIcon[i].sprite = spriteData.ExportWeaponSprite(player.equipWeapon[i].index);
                weaponIcon[i].SetNativeSize();
            }
        }
    }
}

[Serializable]
public class BagPannel
{
    public RectTransform btnPos;
    public RectTransform bagUI;
    public GameObject explainPannel;
    public TextMeshProUGUI nameTxt, tierTxt, explainTxt;
    private Sequence textSequence;

    public void OpenUI()
    {
        // 초기설정
        UIManager uiManager = UIManager.Instance;
        uiManager.isUI = true;
        uiManager.raycastPannel.SetActive(true);
        bagUI.gameObject.SetActive(true);
        bagUI.position = btnPos.position;
        bagUI.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        explainPannel.SetActive(false);

        // 닷트윈
        Sequence seq = DOTween.Sequence().SetUpdate(true);
        seq.Append(bagUI.DOScale(1f, 0.5f))
            .Join(bagUI.DOLocalMove(Vector3.zero, 0.5f));
    }

    public void CloseUI()
    {
        // 초기 설정
        UIManager uiManager = UIManager.Instance;
        uiManager.isUI = false;
        uiManager.raycastPannel.SetActive(false);

        // 닷트윈
        Sequence seq = DOTween.Sequence().SetUpdate(true);
        seq.Append(bagUI.DOScale(0.05f, 0.3f))
            .Join(bagUI.DOLocalMove(btnPos.localPosition, 0.3f))
            .OnComplete(() => bagUI.gameObject.SetActive(false));
    }

    public void ShowExplain(int index)
    {
        // 유물 불러오기
        CSVList csvList = CSVManager.Instance.csvList;
        RelicData data = csvList.FindRelic(index); // 데이터 불러옴
        Color tierColor = csvList.ExportColor(data.tier); // 티어 색 불러옴

        // 초기 설정
        explainPannel.SetActive(true);
        nameTxt.SetText("");
        tierTxt.SetText("");
        explainTxt.SetText("");
        tierTxt.color = tierColor;

        // 닷트윈
        // 동작중인 닷트윈 무시
        if (textSequence != null && textSequence.IsActive())
            textSequence.Kill();

        textSequence = DOTween.Sequence().SetUpdate(true);
        textSequence.Append(nameTxt.DOText(data.name, 0.2f))
                    .Join(tierTxt.DOText(data.tier.ToString(), 0.2f))
                    .Append(explainTxt.DOText(data.explanation, 0.5f));
    }
}

[Serializable]
public class ExpBar
{
    public TextMeshProUGUI lvTxt;
    public Slider bar;

    public void UpdateUI(Card_Player player)
    {
        lvTxt.text = $"Lv {player.lv}";
        UpdateBar(player.exp, player.player.data.exp);
    }

    public void UpdateBar(float curExp, float maxExp)
    {
        bar.DOValue(curExp / maxExp, 0.3f).SetUpdate(true).SetEase(Ease.OutBounce);
    }
}

[Serializable]
public class StatPannel
{
    public RectTransform rect;
    public Image img;
    public TextMeshProUGUI name;
    public TextMeshProUGUI statPointTxt; // 제작 해야함
    public TextMeshProUGUI[] baseStatsTxt; // 왼 -> 오
    public TextMeshProUGUI[] extraStatsTxt; // 위 -> 아래

    public void UpdateUI(Card_Player player)
    {
        // 캐릭터(주인공)
        img.sprite = SpriteData.Instance.playerSprites[(int)player.player.data.type];
        name.text = player.player.data.name;

        // 스탯 포인트
        statPointTxt.text = $"STAT POINT: {player.statPoint}";

        // 기본 스탯
        string totalTxt = player.bonusDmg >= 0 ? $"+{player.bonusDmg}" : $"{player.bonusDmg}";
        baseStatsTxt[0].text = $"<sprite=1>{player.hp}/{player.player.data.hp}"; // 체력 / MAX체력
        baseStatsTxt[1].text = $"<sprite=0>{player.equipWeapon[player.curHand].plus.dmg}({totalTxt})"; // 현재 무기 공격력 (+총 보너스 공격력)
        baseStatsTxt[2].text = $"<sprite=2>{player.defend}"; // 방어력
        baseStatsTxt[3].text = $"<sprite=3>{player.mp}/{player.player.data.mp}"; // 마나 / MAX마나

        // 부가 스탯
        extraStatsTxt[0].text = $"DMG: {player.bonusDmg}"; // 총 공격력
        extraStatsTxt[1].text = $"LUK: {CSVManager.Instance.luck.luck * 100}%"; // 운
        extraStatsTxt[2].text = $"HEAL: {player.bonusHeal}"; // 힐
        extraStatsTxt[3].text = $"DEF: {player.bonusDefend}"; // 방어도회복
        extraStatsTxt[4].text = $"+HP: {player.player.data.hp}"; // 추가 체력
        extraStatsTxt[5].text = $"+MP: {player.player.data.mp}"; // 추가 마나
    }

    public void UpStat(int id, Card_Player player)
    {
        player.statPoint--;

        switch(id)
        {
            // 공격력 증가
            case 0:
                player.bonusDmg++;
                break;
            // 운 증가 (5%)
            case 1:
                CSVManager.Instance.luck.GainLuck(0.05f);
                break;
            // 힐 증가
            case 2:
                player.bonusHeal++;
                break;
            // 방어도 회복 증가
            case 3:
                player.bonusDefend++;
                break;
            // 체력 증가
            case 4:
                player.SetMaxHP(player.player.data.hp + 1);
                break;
            // 마나 증가
            case 5:
                player.player.data.mp++;
                break;
        }

        UpdateUI(player);
        player.SetIconTxt();
    }
}