using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class SpriteData : Singleton<SpriteData>
{
    [Title("맵 타일")]
    public StageSprite[] stageSprites;

    [Title("플레이어 스프라이트", subtitle: "Enum 순서")]
    public Sprite[] playerSprites;

    [Title("포션 스프라이트")]
    public PortionSprite[] portionSprites;

    [Title("무기 스프라이트", subtitle: "순서대로(롱소드 - 숏소드 - 완드 - 마법서)")]
    public Sprite[] weaponSprites;

    [Title("유물 스프라이트", subtitle: "순서대로")]
    public Sprite[] relicSprites;

    [Title("손 스프라이트")]
    public Sprite[] handSprites;

    [Title("몬스터 컬러 애니메이션")]
    public MonsterAnimator[] monsterAnimators;

    [Title("몬스터 카드 스프라이트", subtitle: "0: 일반, 1: 보스")]
    public Sprite[] monsterCards;

    // 랜덤 스테이지 이미지 스프라이트 추출
    public Sprite ExportRanStage()
    {
        StageSprite selStageSprite = Array.Find(stageSprites, sprite => sprite.stage == GameManager.Instance.stage); // 현 stage 불러옴

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