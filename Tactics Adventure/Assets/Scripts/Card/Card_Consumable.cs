using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Card_Consumable : Card
{
    [Title("자식 변수")]
    public int amount;

    // 자식 컴포넌트
    private Consumable consumable;

    public override void SetCard()
    {
        amount = Random.Range(1, gameManager.maxPortion);
        consumable = spawnManager.SpawnPortion_Ran(amount, objTrans); // 현재는 포션만 존재

        // UI 설정
        SetCardName(consumable.consumableName); // 이름
        SetUI(SetUIText()); // 내용
    }

    public override void DestroyCard()
    {
        spawnManager.DeSpawnConsumable(consumable);
        DODestroy();
    }

    public override IEnumerator DoCard()
    {
        Card_Player playerCard = spawnManager.playerCard;

        yield return SetAnim(playerCard.player.anim, AnimID.Interaction);
        yield return new WaitForSeconds(playerCard.animTime);

        // 현재 포션 밖에 없음(확장성 고려)
        if (consumable.type == ConsumableType.Portion)
        {
            Portion portion = consumable.GetComponent<Portion>(); // 포션 컴포넌트 가져오기

            // 포션 타입 별 분류
            switch(portion.portionType)
            {
                case PortionType.HP: // 체력포션
                    if(!playerCard.isPicky)
                        playerCard.HealHP(amount); // 체력회복
                    break;
                case PortionType.MP: // 마나포션
                    playerCard.HealMP(amount); // 마나회복
                    break;
                case PortionType.Poison: // 독포션
                    playerCard.GetPoison(amount);
                    break;
            }
        }
        yield return playerCard.Move(pos);
    }

    public override IEnumerator Damaged(int _amount)
    {
        ChangeAmount(Mathf.Max(0, amount - _amount));

        DODamaged();

        yield return new WaitForSeconds(0.1f);

        if (amount <= 0)
            Die();
    }

    public void ChangeAmount(int _amount)
    {
        amount = _amount;

        SetUI(SetUIText());
    }

    public void Die()
    {
        amount = 0;
        spawnManager.ChangeCard(this, CardType.Empty);
    }

    public string SetUIText()
    {
        string s = ""; // 빈 문자열 생성

        // 일회용 아이템 타입별 UI 설정
        switch(consumable.type)
        {
            // 포션이라면
            case ConsumableType.Portion:
                Portion portion = consumable.GetComponent<Portion>(); // 포션 컴포넌트 받기

                // 포션 타입 구분
                switch(portion.portionType)
                {
                    // 체력
                    case PortionType.HP:
                        s = $"<sprite=1>{amount}";
                        break;
                    // 마나
                    case PortionType.MP:
                        s = $"<sprite=3>{amount}";
                        break;
                    // 독
                    case PortionType.Poison:
                        s = $"<sprite=0>{amount}";
                        break;
                } 
                break;
        }
        return s;
    }
}