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
    [Title("����UI")]
    public bool isUI;
    public GameObject raycastPannel;

    [Title("��ų ��ư")]
    public SkillUI skillUI;

    [Title("�� UI")]
    public TextMeshProUGUI moneyTxt;

    [Title("�� UI")]
    public HandUI handUI;

    [Title("����ġ�� UI")]
    public ExpBar expBar;

    [Title("�г�")]
    public BagPannel bagPannel;
    public StatPannel statPannel;

    public void CheckSkillUI()
    {
        Card_Player playerCard = SpawnManager.Instance.playerCard;

        // ��Ƽ�� ��ų
        skillUI.EnableActive(playerCard.mp >= playerCard.activeMP || playerCard.freeMP > 0);

        // �нú� ��ų
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

    // �� �̹��� ����
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

    // �� ���� ����
    public void HandAlphaUI()
    {
        int curHand = SpawnManager.Instance.playerCard.curHand;

        for (int i = 0; i < 2; i++)
            handAlpha[i].alpha = (curHand == i) ? 1f : 0.5f;
    }

    // ���� ����
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
        // �ʱ⼳��
        UIManager uiManager = UIManager.Instance;
        uiManager.isUI = true;
        uiManager.raycastPannel.SetActive(true);
        bagUI.gameObject.SetActive(true);
        bagUI.position = btnPos.position;
        bagUI.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        explainPannel.SetActive(false);

        // ��Ʈ��
        Sequence seq = DOTween.Sequence().SetUpdate(true);
        seq.Append(bagUI.DOScale(1f, 0.5f))
            .Join(bagUI.DOLocalMove(Vector3.zero, 0.5f));
    }

    public void CloseUI()
    {
        // �ʱ� ����
        UIManager uiManager = UIManager.Instance;
        uiManager.isUI = false;
        uiManager.raycastPannel.SetActive(false);

        // ��Ʈ��
        Sequence seq = DOTween.Sequence().SetUpdate(true);
        seq.Append(bagUI.DOScale(0.05f, 0.3f))
            .Join(bagUI.DOLocalMove(btnPos.localPosition, 0.3f))
            .OnComplete(() => bagUI.gameObject.SetActive(false));
    }

    public void ShowExplain(int index)
    {
        // ���� �ҷ�����
        CSVList csvList = CSVManager.Instance.csvList;
        RelicData data = csvList.FindRelic(index); // ������ �ҷ���
        Color tierColor = csvList.ExportColor(data.tier); // Ƽ�� �� �ҷ���

        // �ʱ� ����
        explainPannel.SetActive(true);
        nameTxt.SetText("");
        tierTxt.SetText("");
        explainTxt.SetText("");
        tierTxt.color = tierColor;

        // ��Ʈ��
        // �������� ��Ʈ�� ����
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
    public TextMeshProUGUI statPointTxt; // ���� �ؾ���
    public TextMeshProUGUI[] baseStatsTxt; // �� -> ��
    public TextMeshProUGUI[] extraStatsTxt; // �� -> �Ʒ�

    public void UpdateUI(Card_Player player)
    {
        // ĳ����(���ΰ�)
        img.sprite = SpriteData.Instance.playerSprites[(int)player.player.data.type];
        name.text = player.player.data.name;

        // ���� ����Ʈ
        statPointTxt.text = $"STAT POINT: {player.statPoint}";

        // �⺻ ����
        string totalTxt = player.bonusDmg >= 0 ? $"+{player.bonusDmg}" : $"{player.bonusDmg}";
        baseStatsTxt[0].text = $"<sprite=1>{player.hp}/{player.player.data.hp}"; // ü�� / MAXü��
        baseStatsTxt[1].text = $"<sprite=0>{player.equipWeapon[player.curHand].plus.dmg}({totalTxt})"; // ���� ���� ���ݷ� (+�� ���ʽ� ���ݷ�)
        baseStatsTxt[2].text = $"<sprite=2>{player.defend}"; // ����
        baseStatsTxt[3].text = $"<sprite=3>{player.mp}/{player.player.data.mp}"; // ���� / MAX����

        // �ΰ� ����
        extraStatsTxt[0].text = $"DMG: {player.bonusDmg}"; // �� ���ݷ�
        extraStatsTxt[1].text = $"LUK: {CSVManager.Instance.luck.luck * 100}%"; // ��
        extraStatsTxt[2].text = $"HEAL: {player.bonusHeal}"; // ��
        extraStatsTxt[3].text = $"DEF: {player.bonusDefend}"; // ��ȸ��
        extraStatsTxt[4].text = $"+HP: {player.player.data.hp}"; // �߰� ü��
        extraStatsTxt[5].text = $"+MP: {player.player.data.mp}"; // �߰� ����
    }

    public void UpStat(int id, Card_Player player)
    {
        player.statPoint--;

        switch(id)
        {
            // ���ݷ� ����
            case 0:
                player.bonusDmg++;
                break;
            // �� ���� (5%)
            case 1:
                CSVManager.Instance.luck.GainLuck(0.05f);
                break;
            // �� ����
            case 2:
                player.bonusHeal++;
                break;
            // �� ȸ�� ����
            case 3:
                player.bonusDefend++;
                break;
            // ü�� ����
            case 4:
                player.SetMaxHP(player.player.data.hp + 1);
                break;
            // ���� ����
            case 5:
                player.player.data.mp++;
                break;
        }

        UpdateUI(player);
        player.SetIconTxt();
    }
}