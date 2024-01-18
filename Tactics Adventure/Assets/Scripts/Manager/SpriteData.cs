using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class SpriteData : Singleton<SpriteData>
{
    [Title("�� Ÿ��")]
    public StageSprite[] stageSprites;

    // ���� �������� �̹��� ��������Ʈ ����
    public Sprite ExportRanStage()
    {
        StageSprite selStageSprite = Array.Find(stageSprites, sprite => sprite.stage == GameManager.Instance.stage); // �� stage �ҷ���

        int ran = UnityEngine.Random.Range(0, selStageSprite.sprites.Length); // stage �̹��� ���� ����

        return selStageSprite.Export(ran); // ����
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