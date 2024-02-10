using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    [Title("스킬 버튼")]
    public Button activeBtn;
    public TextMeshProUGUI passiveCountTxt;
    public Image[] skillCool; // 0: Active, 1 : Passive

    [Title("돈 UI")]
    public TextMeshProUGUI moneyTxt;

    public void CheckSkillUI()
    {
        SpawnManager sm = SpawnManager.Instance;

        // 액티브 스킬
        if (sm.playerCard.mp >= sm.playerCard.activeMP)
        {
            activeBtn.interactable = true;
            skillCool[0].DOFade(0f, 0.2f).SetUpdate(true);
        }
        else
        {
            activeBtn.interactable = false;
            skillCool[0].DOFade(1f, 0.2f).SetUpdate(true);
        }

        // 패시브 스킬
        passiveCountTxt.text = sm.playerCard.passiveCount.ToString();

        Sequence passiveSeq = DOTween.Sequence().SetUpdate(true);
        passiveSeq.Append(skillCool[1].DOFillAmount((float)sm.playerCard.passiveCount / sm.playerCard.player.data.passiveCount, 0.2f))
            .Join(passiveCountTxt.transform.DOScale(2f, 0.2f))
            .Append(passiveCountTxt.transform.DOScale(1f, 0.1f));
    }

    public void MoneyTxt(int amount)
    {
        moneyTxt.text = $"{amount} <sprite=4>";
    }
}
