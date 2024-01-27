using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portion : Consumable
{
    public PortionType portionType;
    public Capacity capacity;

    public void SetPortion(PortionType p_type, int _amount)
    {
        // 타입 설정
        portionType = p_type;

        // 양 설정 (현 기준: 5)
        capacity = (_amount < 5) ? Capacity.Small : Capacity.Large;

        // 이름 입력
        consumableName = (capacity == Capacity.Small) ? "소형" : "대형";
        switch(portionType)
        {
            case PortionType.HP:
                consumableName += " 체력 물약";
                break;
            case PortionType.MP:
                consumableName += " 마나 물약";
                break;
            case PortionType.Poison:
                consumableName += " 독 물약";
                break;
        }

        // 이미지 스프라이트 설정
        spriteRenderer.sprite = spriteData.ExportPortionSprite(portionType, capacity);
    }
}

public enum Capacity { Small = 0, Large }
public enum PortionType { HP, MP, Poison }