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

        // UI����
        SetCardName(chest.data.name); // �̸�
        SetUI(csvManager.csvList.ExportExplain_Ran(type)); // ����
    }

    public override void DestroyCard()
    {
        spawnManager.DeSpawnChest(chest);
        DODestroy();
    }

    public override IEnumerator DoCard()
    {
        spawnManager.ChangeCard(this, (CardType)System.Enum.Parse(typeof(CardType), chest.data.type.ToString()));

        yield return new WaitForSeconds(0.1f);
    }

    public override IEnumerator Damaged(int _amount)
    {
        yield return new WaitForEndOfFrame();
    }
}