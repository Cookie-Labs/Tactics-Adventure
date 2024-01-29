using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Card_Consumable : Card
{
    [Title("�ڽ� ����")]
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

    public override void DoCard()
    {
        // ���� ���� �ۿ� ����(Ȯ�强 ���)
        if(consumable.type == ConsumableType.Portion)
        {
            Portion portion = consumable.GetComponent<Portion>(); // ���� ������Ʈ ��������
            Card_Player playerCard = spawnManager.playerCard;

            // ���� Ÿ�� �� �з�
            switch(portion.portionType)
            {
                case PortionType.HP: // ü������
                    playerCard.HealHP(amount); // ü��ȸ��
                    break;
                case PortionType.MP: // ��������
                    playerCard.HealMP(amount); // ����ȸ��
                    break;
                case PortionType.Poison: // ������
                    playerCard.Damaged(amount);
                    playerCard.poisonCount = 4; // �� ȿ���� ��� 4�� ����
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