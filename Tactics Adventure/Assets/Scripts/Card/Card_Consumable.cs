using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Consumable : Card
{
    // 변수
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