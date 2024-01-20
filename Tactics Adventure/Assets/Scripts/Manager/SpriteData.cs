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

    [Title("무기 스프라이트")]
    public WeaponSprite[] weaponSprites;

    // 랜덤 스테이지 이미지 스프라이트 추출
    public Sprite ExportRanStage()
    {
        StageSprite selStageSprite = Array.Find(stageSprites, sprite => sprite.stage == GameManager.Instance.stage); // 현 stage 불러옴

        int ran = UnityEngine.Random.Range(0, selStageSprite.sprites.Length); // stage 이미지 랜덤 선택

        return selStageSprite.Export(ran); // 추출
    }

    public Sprite ExportPortionSprite(PortionType portionType, Capacity cap)
    {
        return Array.Find(portionSprites, sprite => sprite.portionType == portionType).Export(cap);
    }

    public Sprite ExportRanWeaponSprite(WeaponType weaponType, Tier tier)
    {
        return Array.Find(weaponSprites, sprite => sprite.weaponType == weaponType).RanExport(tier);
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

[Serializable]
public class WeaponSprite
{
    public WeaponType weaponType;
    public WeaponSpriteStruct[] sprites;

    public Sprite Export(Tier tier, int i)
    {
        return Export(tier).sprites[i];
    }

    public Sprite RanExport(Tier tier)
    {
        WeaponSpriteStruct spriteStruct = Export(tier);

        int ranID = UnityEngine.Random.Range(0, spriteStruct.sprites.Length);

        return spriteStruct.sprites[ranID];
    }

    private WeaponSpriteStruct Export(Tier tier)
    {
        return Array.Find(sprites, sprite => sprite.tier == tier);
    }
}

[Serializable]
public struct WeaponSpriteStruct
{
    public Tier tier;
    public Sprite[] sprites;
}