using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class SpriteData : Singleton<SpriteData>
{
    [Title("�� Ÿ��")]
    public StageSprite[] stageSprites;

    [Title("�÷��̾� ��������Ʈ", subtitle: "Enum ����")]
    public Sprite[] playerSprites;

    [Title("���� ��������Ʈ")]
    public PortionSprite[] portionSprites;

    [Title("���� ��������Ʈ", subtitle: "�������(�ռҵ� - ���ҵ� - �ϵ� - ������)")]
    public Sprite[] weaponSprites;

    [Title("���� ��������Ʈ", subtitle: "�������")]
    public Sprite[] relicSprites;

    [Title("�� ��������Ʈ")]
    public Sprite[] handSprites;

    [Title("���� �÷� �ִϸ��̼�")]
    public MonsterAnimator[] monsterAnimators;

    [Title("���� ī�� ��������Ʈ", subtitle: "0: �Ϲ�, 1: ����")]
    public Sprite[] monsterCards;

    // ���� �������� �̹��� ��������Ʈ ����
    public Sprite ExportRanStage()
    {
        StageSprite selStageSprite = Array.Find(stageSprites, sprite => sprite.stage == GameManager.Instance.stage); // �� stage �ҷ���

        return selStageSprite.sprites[UnityEngine.Random.Range(0, selStageSprite.sprites.Length)];
    }

    public Sprite ExportPortionSprite(PortionType portionType, Capacity cap)
    {
        PortionSprite target = Array.Find(portionSprites, sprite => sprite.portionType == portionType);

        return Array.Find(portionSprites, sprite => sprite.portionType == portionType).sprites[(int)cap];
    }

    public Sprite ExportRelicSprite(int index)
    {
        return relicSprites[index];
    }

    public Sprite ExportWeaponSprite(int index)
    {
        return weaponSprites[index];
    }

    public RuntimeAnimatorController ExportRanController(string name)
    {
        MonsterAnimator target = Array.Find(monsterAnimators, anim => anim.name == name);
        return target.controllers[UnityEngine.Random.Range(0, target.controllers.Length)];
    }

    public Sprite ExportMonCard(MonsterType type)
    {
        switch(type)
        {
            case MonsterType.Common:
            case MonsterType.CommonElite:
                return monsterCards[0];
            case MonsterType.SubBoss:
            case MonsterType.Boss:
                return monsterCards[1];
        }
        return null;
    }
}

[Serializable]
public struct StageSprite
{
    public Stage stage;
    public Sprite[] sprites;
}

[Serializable]
public struct PortionSprite
{
    public PortionType portionType;
    public Sprite[] sprites;
}

[Serializable]
public struct MonsterAnimator
{
    public string name;
    public RuntimeAnimatorController[] controllers;
}