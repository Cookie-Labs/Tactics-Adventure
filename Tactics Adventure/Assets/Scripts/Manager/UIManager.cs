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
    [Title("스킬 버튼")]
    public SkillUI skillUI;

    [Title("돈 UI")]
    public TextMeshProUGUI moneyTxt;

    [Title("손 UI")]
    public HandUI handUI;

    public void CheckSkillUI()
    {
        Card_Player playerCard = SpawnManager.Instance.playerCard;

        // 액티브 스킬
        skillUI.EnableActive(playerCard.mp >= playerCard.activeMP);

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
            Sprite handSprite = (player.equipWeapon[i].dmg == 0) ? spriteData.handSprites[i + 2] : spriteData.handSprites[i];
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
            if (player.equipWeapon[i].dmg == 0)
            {
                weaponIcon[i].gameObject.SetActive(false);
                weaponIcon[i].sprite = null;
            }
            
            else
            {
                weaponIcon[i].gameObject.SetActive(true);
                weaponIcon[i].sprite = spriteData.ExportWeaponSprite(player.equipWeapon[i].weaponData.index);
                weaponIcon[i].SetNativeSize();
            }
        }
    }
}