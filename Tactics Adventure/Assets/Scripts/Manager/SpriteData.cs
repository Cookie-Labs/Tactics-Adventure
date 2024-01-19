using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class SpriteData : Singleton<SpriteData>
{
    [Title("맵 타일")]
    public StageSprite[] stageSprites;

    [Title("포션 스프라이트")]
    public PortionSprite[] portionSprites;

    // 랜덤 스테이지 이미지 스프라이트 추출
    public Sprite ExportRanStage()
    {
        StageSprite selStageSprite = Array.Find(stageSprites, sprite => sprite.stage == GameManager.Instance.stage); // 현 stage 불러옴

        int ran = UnityEngine.Random.Range(0, selStageSprite.sprites.Length); // stage 이미지 랜덤 선택

        return selStageSprite.Export(ran); // 추출
    }

    public Sprite ExportPortionSprite(PortionType portionType, Capacity cap)
    {
        return portionSprites[(int)portionType].Export(cap);
    }
}

[Serializable]
public class StageSprite
{
    public Stage stage;
    public Sprite[] sprites;

    public Sprite Export(int a)
    {
        if (a > sprites.Length || a < 0)
        {
            a = 0;
            Debug.LogError("스테이지 이미지 추출 오류");
        }

        return sprites[a];
    }
}

[Serializable]
public class PortionSprite
{
    public PortionType portionType;
    public Sprite[] sprites;

    public Sprite Export(Capacity cap)
    {
        int capID = (int)cap;

        if(capID > Enum.GetValues(typeof(Capacity)).Length || cap < 0)
        {
            cap = Capacity.Small;
            capID = (int)cap;

            Debug.LogError("포션 양 이미지 추출 오류");
        }

        return sprites[capID];
    }
}