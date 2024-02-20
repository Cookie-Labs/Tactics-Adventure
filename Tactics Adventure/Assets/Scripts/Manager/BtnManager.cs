using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BtnManager : Singleton<BtnManager>
{
    bool isClicking;

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
}
