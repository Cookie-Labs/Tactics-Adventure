using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[Serializable]
public struct PlayerData
{
    public string name;
    public PlayerType type;
    public int hp, mp, defend;
}

[Serializable]
public struct ChestData
{
    public string name;
    public ChestType type;
}

[Serializable]
public struct MonsterData
{
    public string name;
    public MonsterType type;
    public int hp;
}

[Serializable]
public struct RelicData
{
    public string name, explanation;
    public Tier tier;
    public bool isHave;
    public int index;
}

[Serializable]
public struct TrapData
{
    public string name;
    public TrapType type;
    public bool isWait;
    public int wait;
}

[Serializable]
public struct WeaponData
{
    public string name;
    public WeaponType type;
    public Tier tier;
    public int index;
}

[Serializable]
public class CSVList
{
    public RelicData[] relicDatas;
    public WeaponData[] weaponDatas;
    public string[] chestExpainTxt;

    #region Relic
    public RelicData FindRelic(int index)
    {
        return relicDatas[index];
    }
    #endregion

    #region Weapon
    public WeaponData FindWeapon(WeaponType _type, Tier _tier)
    {
        // ���� ���� ���� ����Ʈ
        List<WeaponData> validWeapons = new List<WeaponData>();

        // ���� ������ �迭 ��ȯ
        foreach(WeaponData data in weaponDatas)
        {
            // Ÿ�԰� Ƽ� ��ġ�ϸ� ���� ���� ���� ����Ʈ�� �߰��Ѵ�.
            if (data.type == _type && data.tier == _tier)
                validWeapons.Add(data);
        }

        // ����Ʈ ���� ������ 0 �̻��̸�
        if (validWeapons.Count > 0)
            return validWeapons[UnityEngine.Random.Range(0, validWeapons.Count)];
        // ���� ��Ȳ
        else
        {
            Debug.LogError("���� ã�� ����");
            return validWeapons[0];
        }
    }
    #endregion
}