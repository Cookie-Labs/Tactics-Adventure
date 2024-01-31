using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Card_Chest : Card
{
    private Chest chest;

    public override void SetCard()
    {
        chest = spawnManager.SpawnChest_Ran(objTrans);

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
        spawnManager.ChangeCard(this, (CardType)System.Enum.Parse(typeof(CardType), chest.data.type.ToString()));
    }

    public override void Damaged(int _amount)
    {
        return;
    }

    public override void Anim(AnimID id)
    {
    }
}