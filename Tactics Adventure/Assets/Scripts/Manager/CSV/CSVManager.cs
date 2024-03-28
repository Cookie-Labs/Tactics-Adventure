using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CSVManager : Singleton<CSVManager>
{
    public Luck luck;
    public Money money;
    public List<string> availMosters;
    public List<MonsterData> availBosses;
    public TextAsset[] textAssets;
    public CSVList csvList = new CSVList();

    protected override void Awake()
    {
        base.Awake();

        RelicCSVReading();
        WeaponCSVReading();
        MonsterCSVReading();
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
        int order = 1; int size = 4;
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

    public void MonsterCSVReading()
    {
        int order = 2; int size = 3;
        string[] data = textAssets[order].text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        int tableSize = data.Length / size - 1;
        csvList.monsterDatas = new MonsterData[tableSize];

        for (int i = 0; i < tableSize; i++)
        {
            int k = i + 1;
            csvList.monsterDatas[i] = new MonsterData
            {
                name = data[size * k],
                type = (MonsterType)Enum.Parse(typeof(MonsterType), data[size * k + 1]),
                stage = (Stage)Enum.Parse(typeof(Stage), data[size * k + 2])
            };
        }

        // 스테이지 몬스터 설정
        int stage = (int)GameManager.Instance.stage;
        foreach (MonsterData monsterData in csvList.monsterDatas)
        {
            // 일반 몬스터 설정
            if ((int)monsterData.stage <= stage &&
                (monsterData.type == MonsterType.Common || monsterData.type == MonsterType.CommonElite))
                availMosters.Add(monsterData.name);
            // 보스 몬스터 설정
            else if(monsterData.type == MonsterType.SubBoss || monsterData.type == MonsterType.Boss)
            {
                // 무한 모드라면
                if (stage == (int)Stage.Forest)
                    availBosses.Add(monsterData);
                // 무한 모드가 아니라면
                else if((int)monsterData.stage == stage)
                    availBosses.Add(monsterData);
            }
        }
    }
}

[Serializable]
public class Luck
{
    public float luck = 1.0f;
    // 1:Common, 2:Rare, 3:Epic, 4:Legend 
    public int[] tierLuck = new int[4];
    public int weaponPerDmg;

    public void GainLuck(float _gain) // ex) 10% -> 0.1f
    {
        luck += _gain;
    }

    public bool Probability(float percent)
    {
        return UnityEngine.Random.Range(0, 1f) <= percent * luck;
    }

    public Tier LuckToTier()
    {
        int sum = tierLuck.Sum();

        int ranInt = UnityEngine.Random.Range(0, sum);

        for (int i = 0; i < tierLuck.Length; i++)
        {
            if (ranInt < tierLuck[i] * luck)
                return (Tier)i;
        }

        return Tier.Common;
    }

    public int TierToDmg(Tier tier)
    {
        int tierID = (int)tier;

        return (int)MathF.Max(1, UnityEngine.Random.Range(weaponPerDmg * tierID, weaponPerDmg * (tierID + 1)));
    }

    public WeaponData TierToWeapon(WeaponData[] datas)
    {
        WeaponData[] weaponDatas = Array.FindAll(datas, data => data.tier == LuckToTier());
        return weaponDatas[UnityEngine.Random.Range(0, weaponDatas.Length)];
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