using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Card_Coin : Card
{
    [Title("�ڽ� ����")]
    public int amount;

    // �ڽ� ������Ʈ
    private Coin coin;

    public override void SetCard()
    {
        // ���� ����
        amount = Random.Range(1, 10); // �� ���� ����
        coin = spawnManager.SpawnCoin(amount, objTrans);

        // UI����
        SetCardName("����");
        SetUI($"<sprite=3>{amount}");
    }

    public override void DestroyCard()
    {
        spawnManager.DeSpawnCoin(coin);
    }

    public override void DoCard()
    {
        gameManager.EarnMoney(amount); // �� ����

        spawnManager.playerCard.Move(pos);
    }

    public override void Damaged(int _amount)
    {
        ChangeAmount(amount - _amount);

        if (amount <= 0)
        {
            amount = 0;
            spawnManager.DeSpawnCard(this);
        }
    }

    public override void Anim(AnimID id)
    {
    }

    public void ChangeAmount(int _amount)
    {
        amount = _amount;

        coin.UpdateAnim(amount);

        SetUI($"<sprite=3>{amount}");
    }
}
