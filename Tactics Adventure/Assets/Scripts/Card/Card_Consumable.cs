using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Consumable : Card
{
    // ����
    public int amount;

    // �ڽ� ������Ʈ
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
        consumable = spawnManager.SpawnPortion(PortionType.HP, amount, objTrans); // �Ŀ� ����

        // UI ����
        SetCardName(consumable.consumableName); // �̸�
        SetUI(SetUIText()); // ����
    }

    public override void DestroyCard()
    {
        spawnManager.DeSpawnConsumable(consumable);
    }

    public string SetUIText()
    {
        string s = ""; // �� ���ڿ� ����

        // ��ȸ�� ������ Ÿ�Ժ� UI ����
        switch(consumable.type)
        {
            // �����̶��
            case ConsumableType.Portion:
                Portion portion = consumable.GetComponent<Portion>(); // ���� ������Ʈ �ޱ�

                // ���� Ÿ�� ����
                switch(portion.portionType)
                {
                    // ü��
                    case PortionType.HP:
                        s = $"<sprite=1>{amount}";
                        break;
                    // ����
                    case PortionType.MP:
                        s = $"<sprite=2>{amount}";
                        break;
                    // ��
                    case PortionType.Poison:
                        s = $"<sprite=0>{amount}";
                        break;
                } 
                break;
        }
        return s;
    }
}