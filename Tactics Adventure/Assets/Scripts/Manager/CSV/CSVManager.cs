using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSVManager : Singleton<CSVManager>
{
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
