using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Card_Coin : Card
{
    [Title("자식 변수")]
    public int amount;

    // 자식 컴포넌트
    private Coin coin;

    public override void SetCard()
    {
        // 코인 생성
        amount = Random.Range(1, 10); // 돈 랜덤 설정
        coin = spawnManager.SpawnCoin(amount, objTrans);

        // UI설정
        SetCardName("코인");
        SetUI($"<sprite=3>{amount}");
    }

    public override void DestroyCard()
    {
        spawnManager.DeSpawnCoin(coin);
    }

    public override void DoCard()
    {
        gameManager.EarnMoney(amount); // 돈 벌기

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
