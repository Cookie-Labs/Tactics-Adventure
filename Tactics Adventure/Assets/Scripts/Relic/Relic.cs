using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public class Relic : MonoBehaviour, IPoolObject
{
    public RelicData data;

    // ���� ������Ʈ
    private SpriteRenderer spriteRenderer;

    // �ܺ� ������Ʈ
    private SpriteData spriteData;

    public void OnCreatedInPool()
    {
        name = name.Replace("(Clone)", "");

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteData = SpriteData.Instance;
    }

    public void OnGettingFromPool()
    {

    }

    public void SetRelic(int index)
    {
        data = CSVManager.Instance.csvList.FindRelic(index);

        spriteRenderer.sprite =  spriteData.ExportRelicSprite(index);
    }
}
