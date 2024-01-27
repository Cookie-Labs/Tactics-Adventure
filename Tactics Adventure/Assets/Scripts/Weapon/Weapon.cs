using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public class Weapon : MonoBehaviour, IPoolObject
{
    // ����
    public WeaponData data;
    public int dmg;

    // ���� ������Ʈ
    private SpriteRenderer spriteRenderer;

    // �Ŵ���
    private SpriteData spriteData;

    public virtual void OnCreatedInPool()
    {
        name = name.Replace("(Clone)", "");

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteData = SpriteData.Instance;
    }

    public virtual void OnGettingFromPool()
    {
    }

    public void SetWeapon(WeaponData _data)
    {
        // ���� ����
        data = _data;

        // ��������Ʈ ����
        SetSprite();
    }

    private void SetSprite()
    {
        spriteRenderer.sprite = spriteData.ExportWeaponSprite(data.index);
    }
}

public enum WeaponType { LongSword = 0, ShortSword, Wand, Book }