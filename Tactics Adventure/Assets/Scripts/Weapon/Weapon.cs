using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public class Weapon : MonoBehaviour, IPoolObject
{
    // ����
    public WeaponType type;
    public Tier tier;
    public int dmg;

    // ���� ������Ʈ
    private SpriteRenderer spriteRenderer;

    // �Ŵ���
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
        // ���� ����
        type = _type;
        tier = _tier;

        // ���ݷ� ����
        SetDmg();

        // ��������Ʈ ����
        SetSprite();
    }

    private void SetDmg()
    {
        int dmgPer = gameManager.weaponPerDmg;
        int tierID = (int)tier;

        dmg = Random.Range(dmgPer * tierID, dmgPer * (tierID + 1)); // Ƽ� ���� ������ ���ݷ� ����
        dmg = Mathf.Max(1, dmg); // ���� ������ 0 ����
    }

    private void SetSprite()
    {
        spriteRenderer.sprite = spriteData.ExportRanWeaponSprite(type, tier);
    }
}

public enum WeaponType { LongSword = 0, ShortSword, Wand, Book }