using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Coin : Card
{
    // 변수
    public int amount;

    // 자식 컴포넌트
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
        // 변수 설정
        amount = 100;

        // 코인 생성
        coin = spawnManager.SpawnCoin(amount, objTrans); // 후에 수치 넣어서 바꾸기

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

        spawnManager.playerCard.Move(pos); // 플레이어 카드 이동
        spawnManager.DeSpawnCard(this); // 카드 삭제
    }
}
