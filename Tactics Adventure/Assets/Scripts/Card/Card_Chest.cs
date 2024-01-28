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
        chest = spawnManager.SpawnChest(ChestType.Coin, objTrans); // �Ŀ� ���� ����

        // UI����
        SetCardName(chest.data.name); // �̸�
        string[] explains = csvManager.csvList.chestExpainTxt; // ���� ���� �ؽ�Ʈ �ҷ�����
        SetUI(explains[Random.Range(0, explains.Length)]); // ����
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