using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Card_Empty : Card
{
    public override void SetCard()
    {
        // UI¼³Á¤
        SetCardName("ºó Ä«µå");
        SetUI(csvManager.csvList.ExportExplain_Ran(type));
    }

    public override void DestroyCard()
    {
        DODestroy();
    }

    public override void DoCard()
    {
        spawnManager.playerCard.Move(pos);
    }

    public override void Damaged(int _amount)
    {
        return;
    }
}
