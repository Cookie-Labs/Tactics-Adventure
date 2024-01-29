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
        amount = 7;
        consumable = spawnManager.SpawnPortion(PortionType.HP, amount, objTrans); // 후에 설정

        // UI 설정
        SetCardName(consumable.consumableName); // 이름
        SetUI(SetUIText()); // 내용
    }

    public override void DestroyCard()
    {
        spawnManager.DeSpawnConsumable(consumable);
    }

    public override void DoCard()
    {
        // 현재 포션 밖에 없음(확장성 고려)
        if(consumable.type == ConsumableType.Portion)
        {
            Portion portion = consumable.GetComponent<Portion>(); // 포션 컴포넌트 가져오기
            Card_Player playerCard = spawnManager.playerCard;

            // 포션 타입 별 분류
            switch(portion.portionType)
            {
                case PortionType.HP: // 체력포션
                    playerCard.HealHP(amount); // 체력회복
                    break;
                case PortionType.MP: // 마나포션
                    playerCard.HealMP(amount); // 마나회복
                    break;
                case PortionType.Poison: // 독포션
                    playerCard.Damaged(amount);
                    playerCard.poisonCount = 4; // 독 효과는 모두 4턴 지속
                    break;
            }
        }

        spawnManager.PlayerCardMove(this);
    }

    public override void Damaged(int _amount)
    {
        amount -= _amount;

        if (amount <= 0)
        {
            amount = 0;
            spawnManager.DeSpawnCard(this);
        }

        SetUIText();
    }

    public override void Anim(AnimID id)
    {
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
                        s = $"<sprite=2>{amount}";
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