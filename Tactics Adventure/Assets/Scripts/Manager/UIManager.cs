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

    [Title("가방 UI")]
    public BagUI bagUI;

    [Title("경험치바 UI")]
    public ExpBar expBar;

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
public class BagUI
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