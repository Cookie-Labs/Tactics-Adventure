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

    // ���� �������� �̹��� ��������Ʈ ����
    public Sprite ExportRanStage()
    {
        StageSprite selStageSprite = Array.Find(stageSprites, sprite => sprite.stage == GameManager.Instance.stage); // �� stage �ҷ���

        int ran = UnityEngine.Random.Range(0, selStageSprite.sprites.Length); // stage �̹��� ���� ����

        return selStageSprite.Export(ran); // ����
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