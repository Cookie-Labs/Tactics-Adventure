using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Card_Empty : Card
{
    public override void SetCard()
    {
        // UI����
        SetCardName("�� ī��");
        SetUI(csvManager.csvList.ExportExplain_Ran(type));
    }

    public override void DestroyCard()
    {
        DODestroy();
    }

    public override IEnumerator DoCard()
    {
        yield return spawnManager.playerCard.Move(pos);
    }

    public override IEnumerator Damaged(int _amount)
    {
        yield return new WaitForEndOfFrame();
    }
}
