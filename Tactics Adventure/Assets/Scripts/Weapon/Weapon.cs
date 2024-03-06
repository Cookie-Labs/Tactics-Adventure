using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public class Weapon : MonoBehaviour, IPoolObject
{
    // ����
    public WeaponData data;

    // ���� ������Ʈ
    private SpriteRenderer spriteRenderer;

    // �Ŵ���
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
        // ���� ����
        data = _data;

        // �߰� ����
        data.plus.dmg = csvManager.luck.TierToDmg(data.tier);
        data.plus.enforce = new bool[1]; // 0: Drain

        // ��������Ʈ ����
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