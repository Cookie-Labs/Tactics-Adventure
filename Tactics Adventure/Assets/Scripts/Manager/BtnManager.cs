using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BtnManager : Singleton<BtnManager>
{
    bool isClicking;

    public void Tab(RectTransform rectTransform)
    {
        UIManager uiManager = UIManager.Instance;
        if (!rectTransform.gameObject.activeSelf)
        {
            uiManager.isUI = true;
            rectTransform.gameObject.SetActive(true);
            uiManager.raycastPannel.SetActive(true);
            rectTransform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            rectTransform.DOScale(new Vector3(1f, 1f, 1f), 0.5f).SetEase(Ease.InExpo).SetEase(Ease.OutBounce).SetUpdate(true);
        }
        else
        {
            uiManager.isUI = false;
            uiManager.raycastPannel.SetActive(false);
            rectTransform.DOScale(new Vector3(0.05f, 0.05f, 0.05f), 0.25f).SetEase(Ease.InOutExpo).SetUpdate(true).OnComplete(() => rectTransform.gameObject.SetActive(false));
        }
    }

    public void Tab_StatPannel()
    {
        UIManager uiManager = UIManager.Instance;

        uiManager.statPannel.UpdateUI(SpawnManager.Instance.playerCard);
        Tab(uiManager.statPannel.rect);
    }

    public void UpStatBtn(int id)
    {
        Card_Player player = SpawnManager.Instance.playerCard;

        if (player.statPoint <= 0)
            return;

        UIManager.Instance.statPannel.UpStat(id, player);
    }

    public void ActiveSkillBtn()
    {
        if (isClicking)
            return;

        SpawnManager sm = SpawnManager.Instance;

        sm.playerCard.Active();
        StartCoroutine(TouchManager.Instance.TouchEvent());
    }

    public void BtnDelay()
    {
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        isClicking = true;
        yield return new WaitForSeconds(0.2f);
        isClicking = false;
    }

    public void HandBtn(int num)
    {
        SpawnManager.Instance.playerCard.ChangeHand(num);
    }

    public void BagBtn(bool isOpen)
    {
        BagPannel bagUI = UIManager.Instance.bagPannel;

        if (isOpen)
            bagUI.OpenUI();
        else
            bagUI.CloseUI();
    }
}
