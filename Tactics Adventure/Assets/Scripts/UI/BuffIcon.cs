using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BuffIcon : MonoBehaviour, IPoolObject
{
    [HideInInspector] public Image img;
    public TextMeshProUGUI countTxt;

    public BuffIconType buffType;
    public int maxCount, count;

    public void OnCreatedInPool()
    {
        name = name.Replace("(Clone)", "");

        img = GetComponent<Image>();
    }

    public void OnGettingFromPool()
    {
    }

    public void SetBuff(BuffIconData data, int _count)
    {
        // 변수 설정
        buffType = data.type;
        img.sprite = data.sprite;
        maxCount = _count;
        count = _count;

        UpdateUI();
    }

    public void DoTurnBuff()
    {
        switch(buffType)
        {
            case BuffIconType.Poison:
                count--;
                break;
            case BuffIconType.Invicible:
                count = SpawnManager.Instance.playerCard.invincible;
                break;
        }

        if (count <= 0)
            SpawnManager.Instance.DeSpawnBuffIcon(buffType);

        UpdateUI();
    }

    public void UpdateUI()
    {
        countTxt.text = count.ToString();
        img.DOFillAmount((float)count / maxCount, 0.2f);
    }
}

public enum BuffIconType { Poison = 0, Invicible }