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
        // Player, Chest, Coin, Consumable, Monster, Relics, Trap, Weapon
        int[] spawnCount = { 1, 1, 1, 1, 3, 0, 1, 1 };
        int length = spawnCount.Length;

        List<int> drawnNumbers = new List<int> { 4 }; // �ߺ� ������ ����Ʈ (4�� �÷��̾� ��ȯ ��ġ)

        for(int i = 0; i < length; i++) // i -> spawnCount ��ȯ (Ÿ��)
        {
            // �÷��̾� ��ȯ
            if (i == 0)
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
        card.DestroyCard();

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
        player.transform.SetParent(null);

        PoolManager.Instance.TakeToPool<Player>(player.name, player);
    }

    public Monster SpawnMonster(MonsterType type, Transform parent)
    {
        Monster monster = PoolManager.Instance.GetFromPool<Monster>("Monster_" + type); // �÷��̾� ��ȯ

        // ��ġ ����
        monster.transform.SetParent(parent);
        monster.transform.localPosition = Vector3.zero;

        return monster;
    }

    // �Ŀ� ������������ ������ ���� ����
    public Monster SpawnRanMonster(Transform parent)
    {
        int ranMon = Random.Range(0, System.Enum.GetValues(typeof(MonsterType)).Length);

        Monster monster = SpawnMonster((MonsterType)ranMon, parent);

        return monster;
    }

    public void DeSpawnMonster(Monster monster)
    {
        monster.transform.SetParent(null);

        PoolManager.Instance.TakeToPool<Monster>(monster.name, monster);
    }

    public Coin SpawnCoin(int a, Transform parent)
    {
        Coin coin = PoolManager.Instance.GetFromPool<Coin>("Coin"); // ���� ����

        // ��ġ ����
        coin.transform.SetParent(parent);
        coin.transform.localPosition = Vector3.zero;

        // �ִϸ��̼� ����
        coin.UpdateAnim(a);

        return coin;
    }

    public void DeSpawnCoin(Coin coin)
    {
        coin.transform.SetParent(null);

        PoolManager.Instance.TakeToPool<Coin>(coin.name, coin);
    }

    public Chest SpawnChest(ChestType type, Transform parent)
    {
        Chest chest = PoolManager.Instance.GetFromPool<Chest>("Chest_" + type); // ���� ��ȯ

        // ��ġ ����
        chest.transform.SetParent(parent);
        chest.transform.localPosition = Vector3.zero;

        return chest;
    }

    public void DeSpawnChest(Chest chest)
    {
        chest.transform.SetParent(null);

        PoolManager.Instance.TakeToPool<Chest>(chest.name, chest);
    }

    public Portion SpawnPortion(PortionType type, int amount, Transform parent)
    {
        Portion portion = PoolManager.Instance.GetFromPool<Portion>("Consumable_Portion"); // ���� ��ȯ

        // ��ġ ����
        portion.transform.SetParent(parent);
        portion.transform.localPosition = Vector3.zero;

        // ���� ����
        portion.SetPortion(type, amount);

        return portion;
    }

    public void DeSpawnConsumable(Consumable consumable)
    {
        consumable.transform.SetParent(null);

        PoolManager.Instance.TakeToPool<Consumable>(consumable.name, consumable);
    }

    public Trap SpawnTrap(TrapName trapName, Transform parent)
    {
        Trap trap = PoolManager.Instance.GetFromPool<Trap>("Trap_" + trapName); // Ʈ�� ����

        // ��ġ ����
        trap.transform.SetParent(parent);
        trap.transform.localPosition = Vector3.zero;

        return trap;
    }

    public Trap SpawnRanTrap(Transform parent)
    {
        int ranTrap = Random.Range(0, System.Enum.GetValues(typeof(TrapName)).Length);

        return SpawnTrap((TrapName)ranTrap, parent);
    }

    public void DeSpawnTrap(Trap trap)
    {
        trap.transform.SetParent(null);

        PoolManager.Instance.TakeToPool<Trap>(trap.name, trap);
    }
}
