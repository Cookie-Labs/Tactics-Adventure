using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portion : Consumable
{
    public PortionType portionType;
    public Capacity capacity;

    public void SetPortion(PortionType p_type, int _amount)
    {
        // Ÿ�� ����
        portionType = p_type;

        // �� ���� (�� ����: 5)
        capacity = (_amount < 5) ? Capacity.Small : Capacity.Large;

        // �̸� �Է�
        consumableName = (capacity == Capacity.Small) ? "����" : "����";
        switch(portionType)
        {
            case PortionType.HP:
                consumableName += " ü�� ����";
                break;
            case PortionType.MP:
                consumableName += " ���� ����";
                break;
            case PortionType.Poison:
                consumableName += " �� ����";
                break;
        }

        // �̹��� ��������Ʈ ����
        spriteRenderer.sprite = spriteData.ExportPortionSprite(portionType, capacity);
    }
}

public enum Capacity { Small = 0, Large }
public enum PortionType { HP, MP, Poison }