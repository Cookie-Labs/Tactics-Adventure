using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public class Relic : MonoBehaviour, IPoolObject
{
    public RelicData relicData;

    // 내부 컴포넌트
    private SpriteRenderer spriteRenderer;

    // 외부 컴포넌트
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
        relicData = CSVManager.Instance.csvList.FindRelic(index);

        spriteRenderer.sprite =  spriteData.ExportRelicSprite(index);
    }
}
