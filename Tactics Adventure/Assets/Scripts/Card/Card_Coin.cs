using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Coin : Card
{
    // ����
    public int amount;

    // �ڽ� ������Ʈ
    private Coin coin;

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
        // ���� ����
        amount = 100;

        // ���� ����
        coin = spawnManager.SpawnCoin(amount, objTrans); // �Ŀ� ��ġ �־ �ٲٱ�

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

        spawnManager.playerCard.Move(pos); // �÷��̾� ī�� �̵�
        spawnManager.DeSpawnCard(this); // ī�� ����
    }
}
