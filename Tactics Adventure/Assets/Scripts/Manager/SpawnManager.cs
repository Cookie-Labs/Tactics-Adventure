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
        // 종류별 소환될 카드 개수 (총 9개)
        // Coin -> Item -> Monster -> Player -> Trap [Enum]
        int[] spawnCount = { 1, 3, 3, 1, 1 };
        List<int> drawnNumbers = new List<int> { 4 }; // 중복 방지용 리스트 (4는 플레이어 소환 위치)

        for(int i = 0; i < 5; i++) // i -> spawnCount 순환 (타입)
        {
            // 플레이어 소환
            if (i == 3)
            {
                SpawnCard((CardType)i, cardPos[4]);
                continue;
            }

            // 플레이어 이외 카드 소환
            for(int j = 0; j < spawnCount[i]; j++) // -> j -> spawnCount[i] 순환 (타입 수)
            {
                // 랜덤 위치 설정
                int ranNum;
                do
                    ranNum = Random.Range(0, 9);
                while (drawnNumbers.Contains(ranNum));

                drawnNumbers.Add(ranNum);
                SpawnCard((CardType)i, cardPos[ranNum]); // 소환
            }
        }
    }

    public Card SpawnCard(CardType type, Transform parent)
    {
        Card card = PoolManager.Instance.GetFromPool<Card>("Card_" + type); // 카드 소환

        maxCard[(int)type]--; // 해당 카드 최대 수량--

        // 위치 설정
        card.transform.SetParent(parent);
        card.transform.localPosition = Vector3.zero;

        // 카드 리스트에 추가
        cardList.Add(card);
        return card;
    }

    public void DeSpawnCard(Card card)
    {
        PoolManager.Instance.TakeToPool<Card>(card.name, card);

        maxCard[(int)card.type]++; // 해당 카드 최대 수량++

        card.transform.SetParent(null);

        cardList.Remove(card);
    }

    public Player SpawnPlayer(PlayerType type, Transform parent)
    {
        Player player = PoolManager.Instance.GetFromPool<Player>("Player_" + type); // 플레이어 소환

        // 위치 설정
        player.transform.SetParent(parent);
        player.transform.localPosition = Vector3.zero;

        return player;
    }

    public void DeSpawnPlayer(Player player)
    {
        PoolManager.Instance.TakeToPool<Player>(player.name, player);
    }
}
