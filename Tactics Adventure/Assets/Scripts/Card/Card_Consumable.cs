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

    public override void SetCard()
    {
        amount = Random.Range(1, gameManager.maxPortion);
        consumable = spawnManager.SpawnPortion_Ran(amount, objTrans); // ����� ���Ǹ� ����

        // UI ����
        SetCardName(consumable.consumableName); // �̸�
        SetUI(SetUIText()); // ����
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

        // ���� ���� �ۿ� ����(Ȯ�强 ���)
        if (consumable.type == ConsumableType.Portion)
        {
            Portion portion = consumable.GetComponent<Portion>(); // ���� ������Ʈ ��������

            // ���� Ÿ�� �� �з�
            switch(portion.portionType)
            {
                case PortionType.HP: // ü������
                    if(!playerCard.isPicky)
                        playerCard.HealHP(amount); // ü��ȸ��
                    break;
                case PortionType.MP: // ��������
                    playerCard.HealMP(amount); // ����ȸ��
                    break;
                case PortionType.Poison: // ������
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
                        s = $"<sprite=3>{amount}";
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