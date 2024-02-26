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
    public int skillMP, passiveCount;
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
    public bool isCollect;
    public int index;
}

[Serializable]
public struct TrapData
{
    public string name;
    public TrapType type;
    public Direction[] targetDir;
    public bool isWait;
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
public struct StageMonsterData
{
    public Stage stage;
    public MonsterType[] type;
}

[Serializable]
public struct ExplainData
{
    public CardType type;
    public string[] explains;
}

[Serializable]
public struct TierColor
{
    public Tier tier;
    public Color color;
}

[Serializable]
public class CSVList
{
    public RelicData[] relicDatas;
    public WeaponData[] weaponDatas;
    public TrapData[] trapDatas;
    public StageMonsterData[] stageMonsterDatas;
    public MonsterType[] availMonList;
    public ExplainData[] explainDatas;
    public TierColor[] tierColors;

    #region Relic
    public RelicData FindRelic(int index)
    {
        return relicDatas[index];
    }

    public ref RelicData ExportRelic(RelicData data)
    {
        return ref relicDatas[data.index];
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

    public WeaponData FindWeapon(int ID)
    {
        return Array.Find(weaponDatas, data => data.index == ID);
    }
    #endregion

    #region Trap
    public TrapData FindTrap(TrapType type)
    {
        return Array.Find(trapDatas, data => data.type == type);
    }
    #endregion

    #region Explain
    public string ExportExplain_Ran(CardType type)
    {
        ExplainData explainData = Array.Find(explainDatas, data => data.type == type);

        return explainData.explains[UnityEngine.Random.Range(0, explainData.explains.Length)];
    }
    #endregion

    #region Tier
    public Color ExportColor(Tier tier)
    {
        return Array.Find(tierColors, color => color.tier == tier).color;
    }
    #endregion
}

// ��Ÿ ������
public enum AnimID { Idle, Walk, Damaged, Atk, Die, Interaction}

[Serializable]
public struct EquipWeapon
{
    public int dmg;
    public WeaponData weaponData;

    public EquipWeapon(int _dmg, WeaponData _data)
    {
        dmg = _dmg;
        weaponData = _data;
    }
}