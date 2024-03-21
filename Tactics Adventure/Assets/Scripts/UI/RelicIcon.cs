using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;
using UnityEngine.UI;

public class RelicIcon : MonoBehaviour, IPoolObject
{
    public int ID;
    private Image img;

    private SpriteData spriteData;
    private UIManager uiManager;

    public void OnCreatedInPool()
    {
        name = name.Replace("(Clone)", "");

        img = GetComponent<Image>();
        spriteData = SpriteData.Instance;
        uiManager = UIManager.Instance;
    }

    public void OnGettingFromPool()
    {
    }

    public void SetIcon(int index)
    {
        ID = index;
        img.sprite = spriteData.ExportRelicSprite(ID);
    }

    public void Click()
    {
        uiManager.bagPannel.ShowExplain(ID);
    }
}