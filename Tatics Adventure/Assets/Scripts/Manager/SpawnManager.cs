using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public class SpawnManager : Singleton<SpawnManager>
{
    public int[] maxCard;
    public Transform[] cardPos;
    public List<Card> cardList;

    private void Start()
    {
        SpawnStage();
    }

    private void SpawnStage()
    {
        // ������ ��ȯ�� ī�� ���� (�� 9��)
        // Coin -> Item -> Monster -> Player -> Trap [Enum]
        int[] spawnCount = { 1, 3, 3, 1, 1 };
        List<int> drawnNumbers = new List<int> { 4 }; // �ߺ� ������ ����Ʈ (4�� �÷��̾� ��ȯ ��ġ)

        for(int i = 0; i < 5; i++) // i -> spawnCount ��ȯ (Ÿ��)
        {
            // �÷��̾� ��ȯ
            if (i == 3)
            {
                SpawnCard((CardType)i, cardPos[4]);
                continue;
            }

            // �÷��̾� �̿� ī�� ��ȯ
            for(int j = 0; j < spawnCount[i]; j++) // -> j -> spawnCount[i] ��ȯ (Ÿ�� ��)
            {
                // ���� ��ġ ����
                int ranNum;
                do
                    ranNum = Random.Range(0, 9);
                while (drawnNumbers.Contains(ranNum));

                drawnNumbers.Add(ranNum);
                SpawnCard((CardType)i, cardPos[ranNum]); // ��ȯ
            }
        }
    }

    public Card SpawnCard(CardType type, Transform parent)
    {
        Card card = PoolManager.Instance.GetFromPool<Card>("Card_" + type); // ī�� ��ȯ

        maxCard[(int)type]--; // �ش� ī�� �ִ� ����--

        // ��ġ ����
        card.transform.SetParent(parent);
        card.transform.localPosition = Vector3.zero;

        // ī�� ����Ʈ�� �߰�
        cardList.Add(card);
        return card;
    }

    public void DeSpawnCard(Card card)
    {
        PoolManager.Instance.TakeToPool<Card>(card.name, card);

        maxCard[(int)card.type]++; // �ش� ī�� �ִ� ����++

        card.transform.SetParent(null);

        cardList.Remove(card);
    }

    public Player SpawnPlayer(PlayerType type, Transform parent)
    {
        Player player = PoolManager.Instance.GetFromPool<Player>("Player_" + type); // �÷��̾� ��ȯ

        // ��ġ ����
        player.transform.SetParent(parent);
        player.transform.localPosition = Vector3.zero;

        return player;
    }

    public void DeSpawnPlayer(Player player)
    {
        PoolManager.Instance.TakeToPool<Player>(player.name, player);
    }
}
