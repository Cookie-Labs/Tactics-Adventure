using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public class Weapon : MonoBehaviour, IPoolObject
{
    // 변수
    public WeaponType type;
    public Tier tier;
    public int dmg;

    // 내부 컴포넌트
    private SpriteRenderer spriteRenderer;

    // 매니저
    private GameManager gameManager;
    private SpriteData spriteData;

    public virtual void OnCreatedInPool()
    {
        name = name.Replace("(Clone)", "");

        spriteRenderer = GetComponent<SpriteRenderer>();
        gameManager = GameManager.Instance;
        spriteData = SpriteData.Instance;
    }

    public virtual void OnGettingFromPool()
    {
    }

    public void SetWeapon(WeaponType _type, Tier _tier)
    {
        // 변수 대입
        type = _type;
        tier = _tier;

        // 공격력 설정
        SetDmg();

        // 스프라이트 설정
        SetSprite();
    }

    private void SetDmg()
    {
        int dmgPer = gameManager.weaponPerDmg;
        int tierID = (int)tier;

        dmg = Random.Range(dmgPer * tierID, dmgPer * (tierID + 1)); // 티어에 따라 무작위 공격력 설정
        dmg = Mathf.Max(1, dmg); // 무기 데미지 0 방지
    }

    private void SetSprite()
    {
        spriteRenderer.sprite = spriteData.ExportRanWeaponSprite(type, tier);
    }
}

public enum WeaponType { LongSword = 0, ShortSword, Wand, Book }