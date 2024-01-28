using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Chest : Card
{
    private Chest chest;

    public override void OnCreatedInPool()
    {
        base.OnCreatedInPool();
    }

    public override void OnGettingFromPool()
    {
        base.OnGettingFromPool();
    }

    public override void SetCard()
    {
        chest = spawnManager.SpawnChest(ChestType.Coin, objTrans); // 후에 지정 스폰

        // UI설정
        SetCardName(chest.data.name); // 이름
        string[] explains = csvManager.csvList.chestExpainTxt; // 상자 내용 텍스트 불러오기
        SetUI(explains[Random.Range(0, explains.Length)]); // 내용
    }

    public override void DestroyCard()
    {
        spawnManager.DeSpawnChest(chest);
    }

    public override void DoCard()
    {
        spawnManager.SpawnCard((CardType)System.Enum.Parse(typeof(CardType), chest.data.type.ToString()), pos);
        spawnManager.DeSpawnCard(this);
    }

    public override void Damaged(int _amount)
    {
        return;
    }
}