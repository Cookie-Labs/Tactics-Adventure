using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Card_Coin : Card
{
    [Title("자식 변수")]
    public bool isChange; // 빙고로 바뀐 코인 확인
    public int amount;

    // 자식 컴포넌트
    private Coin coin;

    public override void SetCard()
    {
        // 코인 생성
        isChange = false;
        amount = Random.Range(1, 10); // 돈 랜덤 설정
        coin = spawnManager.SpawnCoin(amount, objTrans);

        // UI설정
        SetCardName("코인");
        SetUI($"<sprite=4>{amount}");
    }

    public override void DestroyCard()
    {
        spawnManager.DeSpawnCoin(coin);
        DODestroy();
    }

    public override IEnumerator DoCard()
    {
        csvManager.money.EarnMoney(amount); // 돈 벌기
        yield return spawnManager.playerCard.Move(pos);
    }

    public override IEnumerator Damaged(int _amount)
    {
        ChangeAmount(Mathf.Max(0, amount - _amount), false);

        DODamaged();

        yield return new WaitForEndOfFrame();

        if (amount <= 0)
            Die();
    }

    public void ChangeAmount(int _amount, bool _isChanged)
    {
        isChange = _isChanged;
        amount = _amount;

        if (!isChange)
            coin.UpdateAnim(amount);
        else
            coin.anim.SetInteger("Change", (int)CoinType.Change);

        SetUI($"<sprite=4>{amount}");
    }

    public void Die()
    {
        amount = 0;
        spawnManager.ChangeCard(this, CardType.Empty);
    }
}
