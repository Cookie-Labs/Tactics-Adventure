using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CSVManager : Singleton<CSVManager>
{
    public Luck luck;
    public Money money;
    public TextAsset[] textAssets;
    public MonsterType[] availMonStage; // 스테이지별 허용 몬스터
    public CSVList csvList = new CSVList();

    protected override void Awake()
    {
        base.Awake();

        RelicCSVReading();
        WeaponCSVReading();
        SetAvailMon();
    }

    public void RelicCSVReading()
    {
        int order = 0; int size = 3;
        string[] data = textAssets[order].text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        int tableSize = data.Length / size - 1;
        csvList.relicDatas = new RelicData[tableSize];

        for (int i = 0; i < tableSize; i++)
        {
            int k = i + 1;
            csvList.relicDatas[i] = new RelicData
            {
                name = data[size * k],
                explanation = data[size * k + 1],
                tier = (Tier)Enum.Parse(typeof(Tier), data[size * k + 2]),
                index = i
            };
        }
    }

    public void WeaponCSVReading()
    {
        int order = 1; int size = 3;
        string[] data = textAssets[order].text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        int tableSize = data.Length / size - 1;
        csvList.weaponDatas = new WeaponData[tableSize];

        for (int i = 0; i < tableSize; i++)
        {
            int k = i + 1;
            csvList.weaponDatas[i] = new WeaponData
            {
                name = data[size * k],
                type = (WeaponType)Enum.Parse(typeof(WeaponType), data[size * k + 1]),
                tier = (Tier)Enum.Parse(typeof(Tier), data[size * k + 2]),
                attribute = (WeaponAttribute)Enum.Parse(typeof(WeaponAttribute), data[size * k + 3]),
                index = i
            };
        }
    }

    public void SetAvailMon()
    {
        List<MonsterType> availMonList = new List<MonsterType>();
        int curStage = (int)GameManager.Instance.stage;

        if (curStage == Enum.GetValues(typeof(Stage)).Length - 1) // 마지막 스테이지라면
            curStage--;

        // 해당 스테이지 이하 몬스터 전부 해금
        for (int i = 0; i <= curStage; i++)
        {
            foreach (MonsterType type in csvList.stageMonsterDatas[i].type)
            {
                availMonList.Add(type);
            }
        }

        availMonStage = availMonList.ToArray();
    }
}

[Serializable]
public class Luck
{
    public float luck = 1.0f;
    // 1:Common, 2:Rare, 3:Epic, 4:Legend 
    public int[] tierLuck = new int[4];
    public int weaponPerDmg;

    public void GainLuck(float _gain)
    {
        luck *= _gain;
        luck = MathF.Floor(luck * 10f) / 10f;
    }

    public bool Probability(float percent)
    {
        return UnityEngine.Random.Range(0, 1f) <= percent * luck;
    }

    public Tier LuckToTier()
    {
        int sum = tierLuck.Sum();

        int ranTier = UnityEngine.Random.Range(0, sum);

        for (int i = 0; i < tierLuck.Length; i++)
        {
            if (ranTier * luck < tierLuck[i])
                return (Tier)i;
        }

        return Tier.Common;
    }

    public int TierToDmg(Tier tier)
    {
        int tierID = (int)tier;

        return (int)MathF.Max(1, UnityEngine.Random.Range(weaponPerDmg * tierID, weaponPerDmg * (tierID + 1)));
    }

    public (WeaponData data, int dmg) TierToWeapon(WeaponData[] datas)
    {
        WeaponData[] weaponDatas = Array.FindAll(datas, data => data.tier == LuckToTier());
        WeaponData weaponData = weaponDatas[UnityEngine.Random.Range(0, weaponDatas.Length)];

        return (weaponData, TierToDmg(weaponData.tier));
    }
}

[Serializable]
public class Money
{
    public int money;

    public void EarnMoney(int coin)
    {
        money += coin;

        UIManager.Instance.MoneyTxt(money);
    }

    public void LoseMoney(int coin)
    {
        money = Mathf.Max(0, money - coin);

        UIManager.Instance.MoneyTxt(money);
    }
}