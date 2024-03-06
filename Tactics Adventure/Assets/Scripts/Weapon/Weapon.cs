using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public class Weapon : MonoBehaviour, IPoolObject
{
    // 변수
    public WeaponData data;

    // 내부 컴포넌트
    private SpriteRenderer spriteRenderer;

    // 매니저
    private CSVManager csvManager;
    private SpriteData spriteData;

    public virtual void OnCreatedInPool()
    {
        name = name.Replace("(Clone)", "");

        spriteRenderer = GetComponent<SpriteRenderer>();

        csvManager = CSVManager.Instance;
        spriteData = SpriteData.Instance;
    }

    public virtual void OnGettingFromPool()
    {
    }

    public void SetWeapon(WeaponData _data)
    {
        // 변수 대입
        data = _data;

        // 추가 스탯
        data.plus.dmg = csvManager.luck.TierToDmg(data.tier);
        data.plus.enforce = new bool[1]; // 0: Drain

        // 스프라이트 설정
        SetSprite();
    }

    private void SetSprite()
    {
        spriteRenderer.sprite = spriteData.ExportWeaponSprite(data.index);
    }
}

public enum WeaponType { LongSword = 0, ShortSword, Wand, Book }
public enum WeaponAttribute { Physics, Magic }
public enum EnforceID { Drain = 0 }