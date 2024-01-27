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
        // 조건 맞춤 무기 리스트
        List<WeaponData> validWeapons = new List<WeaponData>();

        // 무기 데이터 배열 순환
        foreach(WeaponData data in weaponDatas)
        {
            // 타입과 티어가 일치하면 조건 맞춤 무기 리스트에 추가한다.
            if (data.type == _type && data.tier == _tier)
                validWeapons.Add(data);
        }

        // 리스트 원소 개수가 0 이상이면
        if (validWeapons.Count > 0)
            return validWeapons[UnityEngine.Random.Range(0, validWeapons.Count)];
        // 오류 상황
        else
        {
            Debug.LogError("무기 찾기 오류");
            return validWeapons[0];
        }
    }
    #endregion
}