using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class SpriteData : Singleton<SpriteData>
{
    [Title("�� Ÿ��")]
    public StageSprite[] stageSprites;

    [Title("���� ��������Ʈ")]
    public PortionSprite[] portionSprites;

    [Title("���� ��������Ʈ")]
    public WeaponSprite[] weaponSprites;

    // ���� �������� �̹��� ��������Ʈ ����
    public Sprite ExportRanStage()
    {
        StageSprite selStageSprite = Array.Find(stageSprites, sprite => sprite.stage == GameManager.Instance.stage); // �� stage �ҷ���

        int ran = UnityEngine.Random.Range(0, selStageSprite.sprites.Length); // stage �̹��� ���� ����

        return selStageSprite.Export(ran); // ����
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
            Debug.LogError("�������� �̹��� ���� ����");
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

            Debug.LogError("���� �� �̹��� ���� ����");
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