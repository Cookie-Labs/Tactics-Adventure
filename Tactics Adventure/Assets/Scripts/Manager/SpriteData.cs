using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class SpriteData : Singleton<SpriteData>
{
    [Title("맵 타일")]
    public StageSprite[] stageSprites;

    // 랜덤 스테이지 이미지 스프라이트 추출
    public Sprite ExportRanStage()
    {
        StageSprite selStageSprite = Array.Find(stageSprites, sprite => sprite.stage == GameManager.Instance.stage); // 현 stage 불러옴

        int ran = UnityEngine.Random.Range(0, selStageSprite.sprites.Length); // stage 이미지 랜덤 선택

        return selStageSprite.Export(ran); // 추출
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