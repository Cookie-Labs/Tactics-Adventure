using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portion : Consumable
{
    public PortionType portionType;
    public Capacity capacity;

    public void SetPortion(PortionType p_type, int amount)
    {
        // Ÿ�� ����
        portionType = p_type;

        // �� ���� (�� ����: 5)
        capacity = (amount < 5) ? Capacity.Small : Capacity.Large;

        // �̹��� ��������Ʈ ����
        spriteRenderer.sprite = spriteData.ExportPortionSprite(portionType, capacity);
    }
}

public enum Capacity { Small = 0, Large }
public enum PortionType { HP, MP, Poison }