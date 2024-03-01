using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicManager : Singleton<RelicManager>
{
    public List<int> collectList;

    public void AddRelicList(Relic relic)
    {
        collectList.Add(relic.data.index);
    }

    public bool CheckRelicCollection(int relicID)
    {
        return collectList.Contains(relicID);
    }

    public IEnumerator DoRelic(int relicID, Card_Player player)
    {
        float delay = 0f;
        switch(relicID)
        {
            // ��ũ��: �ִ�ü�� 1 ����
            case 1:
                player.player.data.hp++;
                break;
            // *�Ϲ��: ü�� MAX
            case 2:
                player.HealHP(player.player.data.hp);
                break;
            // �۷����溣��: ü��&���� MAX
            case 3:
                player.HealHP(player.player.data.hp);
                player.HealMP(player.player.data.mp);
                break;
            // ��ŷ: 50���� ȹ��
            case 4:
                GameManager.Instance.EarnMoney(50);
                break;
            // ������ �о�: 200���� ȹ��
            case 5:
                GameManager.Instance.EarnMoney(200);
                break;
            // �������� : ���� MAX
            case 6:
                player.HealMP(player.player.data.mp);
                break;
            // ����: ������ ���� ȹ��
            case 8:
                // Ƽ�� ���� �� ����
                break;
            // �߽���: ������ ���� ���� ȹ��
            case 9:
                // ���� ����ȭ �� ����
                break;
            // �缭: ������ ���� ���� ȹ��
            case 10:
                // ���� ����ȭ �� ����
                break;
            // VIP: ���� 30% ����
            case 11:
                // ���� ���� �� ����
                break;
            // VVIP: ���� 50% ����
            case 12:
                // ���� ���� �� ����
                break;
            // ���: ��� ���� ���� 3
            case 13:
                List<Card> monList = SpawnManager.Instance.cardList.FindAll(card => card.type == CardType.Monster);
                float maxDelay = 0f;

                foreach (Card card in monList)
                {
                    StartCoroutine(card.Damaged(3));
                    maxDelay = Mathf.Max(delay, card.animTime);
                }
                delay = maxDelay;
                break;
            // �ζ�: ���� óġ �� 1% Ȯ���� 500���� ȹ��
            case 14:
                // �� ��ġ �� ����
                break;
        }

        yield return new WaitForSeconds(delay);
    }
}
