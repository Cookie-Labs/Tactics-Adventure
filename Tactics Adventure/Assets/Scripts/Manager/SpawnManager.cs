using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;
using System.Linq;
using Sirenix.OdinInspector;

public class SpawnManager : Singleton<SpawnManager>
{
    [Title("����", subtitle: "�ٸ� ��ũ��Ʈ �̿����� ����")]
    public int[] maxCard;
    public Transform[] cardPos;
    public List<Card> cardList;
    [HideInInspector] public Card_Player playerCard;
    [HideInInspector] public List<Card> turnCardList; // ���� �۵��ϴ� ī�� ����Ʈ

    private void Start()
    {
        SpawnStage();
    }

    private void SpawnStage()
    {
        // ������ ��ȯ�� ī�� ���� (�� 10��)
        // Player, Chest, Coin, Consumable, Monster, Relics, Trap, Weapon, Empty
        int[] spawnCount = { 1, 1, 1, 1, 3, 0, 1, 1, 0 };
        int length = spawnCount.Length;

        List<int> drawnNumbers = new List<int> { 4 }; // �ߺ� ������ ����Ʈ (4�� �÷��̾� ��ȯ ��ġ)

        for(int i = 0; i < length; i++) // i -> spawnCount ��ȯ (Ÿ��)
        {
            // �÷��̾� ��ȯ
            if (i == 0)
            {
                SpawnCard((CardType)i, 4);
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
                SpawnCard((CardType)i, ranNum); // ��ȯ
            }
        }

        playerCard = cardList[0].GetComponent<Card_Player>(); // �÷��̾� ī�� ����
        playerCard.SetNeighbor(); // �÷��̾� ī���� �ֺ� ī�� ��ġ Ȱ��ȭ
    }

    #region ī��
    public Card SpawnCard(CardType type, int pos)
    {
        Card card = PoolManager.Instance.GetFromPool<Card>("Card_" + type); // ī�� ��ȯ

        maxCard[(int)type]--; // �ش� ī�� �ִ� ����--

        // ��ġ ����
        card.transform.SetParent(cardPos[pos]);
        card.transform.localPosition = Vector3.zero;

        // ���� ����
        card.pos = pos;

        // ī�� ����Ʈ�� �߰�
        cardList.Add(card);

        return card;
    }

    public Card SpawnRanCard()
    {
        List<int> typeIDList = new List<int>(); // Ÿ��ID�� ���� ����Ʈ (������)

        // ���� Ÿ�� ���ϱ�
        // Ȯ�� ����
        for (int i = 0; i < maxCard.Length; i++)
        {
            typeIDList.AddRange(Enumerable.Repeat(i, maxCard[i])); // maxCard[i]��ŭ i�� ����Ʈ�� �߰��Ѵ�
        }

        // �� pos ã��
        int pos = System.Array.FindIndex(cardPos, cp => cp.childCount == 0); // �ڽ��� ���� 0�� pos

        // ���� �� pos �� ���ٸ� (����)
        if (pos == -1)
        {
            Debug.LogError("�� pos�� ��� ī�尡 ��ȯ���� �ʾҽ��ϴ�.");
            return null;
        }

        return SpawnCard((CardType)typeIDList[Random.Range(0, typeIDList.Count)], pos); // ī�� ��ȯ!
    }

    public void DeSpawnCard(Card card)
    {
        card.DestroyCard();

        PoolManager.Instance.TakeToPool<Card>(card.name, card);

        maxCard[(int)card.type]++; // �ش� ī�� �ִ� ����++

        card.transform.SetParent(null);

        cardList.Remove(card);
        if (card.isTurn)
        {
            turnCardList.Remove(card);
            card.isTurn = false;
        }
    }

    public Card FindCard(Vector3 pos)
    {
        // ���� �˻�
        foreach (Card card in cardList)
            if (card.transform.parent.position == pos)
                return card;
        return null;
    }

    public Card[] FindCards(Vector3[] poses)
    {
        List<Card> cards = new List<Card>();

        foreach (Vector3 pos in poses)
        {
            Card card = FindCard(pos);

            if (card != null)
                cards.Add(FindCard(pos));
        }

        return cards.ToArray();
    }

    public void DoTurnCards()
    {
        foreach (Card card in turnCardList)
            card.DoTurnCard();
    }

    public Card ChangeCard(Card oriCard, CardType type)
    {
        Card newCard = SpawnCard(type, oriCard.pos);
        DeSpawnCard(oriCard);
        return newCard;
    }

    public void ChangeCoinCard(Card oriCard, int amount)
    {
        Card_Coin coinCard = ChangeCard(oriCard, CardType.Coin).GetComponent<Card_Coin>();

        coinCard.ChangeAmount(amount);
    }
    #endregion

    public Player SpawnPlayer(PlayerType type, Transform parent)
    {
        Player player = PoolManager.Instance.GetFromPool<Player>("Player_" + type); // �÷��̾� ��ȯ

        // ��ġ ����
        player.transform.SetParent(parent);
        player.transform.localPosition = Vector3.zero;
        player.transform.localScale = Vector3.one;

        return player;
    }

    public void DeSpawnPlayer(Player player)
    {
        player.transform.SetParent(null);

        PoolManager.Instance.TakeToPool<Player>("Player_" + player.name, player);
    }

    public Monster SpawnMonster(MonsterType type, Transform parent)
    {
        Monster monster = PoolManager.Instance.GetFromPool<Monster>("Monster_" + type); // �÷��̾� ��ȯ

        // ��ġ ����
        monster.transform.SetParent(parent);
        monster.transform.localPosition = Vector3.zero;
        monster.transform.localScale = Vector3.one;

        return monster;
    }

    public Monster SpawnMonster_Ran(Transform parent)
    {
        while(true)
        {
            MonsterType monsterType = (MonsterType)RandomID(System.Enum.GetValues(typeof(MonsterType)).Length);

            if (CSVManager.Instance.availMonStage.Contains(monsterType)) // �˸��� ���͸� �������� �̾Ƴ� ���
                return SpawnMonster(monsterType, parent);
        }
    }

    public void DeSpawnMonster(Monster monster)
    {
        monster.transform.SetParent(null);

        PoolManager.Instance.TakeToPool<Monster>("Monster_" + monster.name, monster);
    }

    public Coin SpawnCoin(int a, Transform parent)
    {
        Coin coin = PoolManager.Instance.GetFromPool<Coin>("Coin"); // ���� ����

        // ��ġ ����
        coin.transform.SetParent(parent);
        coin.transform.localPosition = Vector3.zero;
        coin.transform.localScale = Vector3.one;

        // �ִϸ��̼� ����
        coin.UpdateAnim(a);

        return coin;
    }

    public void DeSpawnCoin(Coin coin)
    {
        coin.transform.SetParent(null);

        PoolManager.Instance.TakeToPool<Coin>("Coin", coin);
    }

    public Chest SpawnChest(ChestType type, Transform parent)
    {
        Chest chest = PoolManager.Instance.GetFromPool<Chest>("Chest_" + type); // ���� ��ȯ

        // ��ġ ����
        chest.transform.SetParent(parent);
        chest.transform.localPosition = Vector3.zero;
        chest.transform.localScale = Vector3.one;

        return chest;
    }

    public Chest SpawnChest_Ran(Transform parent)
    {
        return SpawnChest((ChestType)RandomID(System.Enum.GetValues(typeof(ChestType)).Length), parent);
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
        portion.transform.localScale = Vector3.one;

        // ���� ����
        portion.SetPortion(type, amount);

        return portion;
    }

    public Portion SpawnPortion_Ran(int amount, Transform parent)
    {
        int ranPortion = RandomID(System.Enum.GetValues(typeof(PortionType)).Length);

        return SpawnPortion((PortionType)ranPortion, amount, parent);
    }

    public void DeSpawnConsumable(Consumable consumable)
    {
        consumable.transform.SetParent(null);

        PoolManager.Instance.TakeToPool<Consumable>("Consumable_Portion", consumable);
    }

    public Trap SpawnTrap(TrapType trapName, Transform parent)
    {
        Trap trap = PoolManager.Instance.GetFromPool<Trap>("Trap_" + trapName); // Ʈ�� ����

        // ��ġ ����
        trap.transform.SetParent(parent);
        trap.transform.localPosition = Vector3.zero;
        trap.transform.localScale = Vector3.one;

        // ���� ����
        trap.SetTrap();

        return trap;
    }

    public Trap SpawnTrap_Ran(Transform parent)
    {
        return SpawnTrap((TrapType)RandomID(System.Enum.GetValues(typeof(TrapType)).Length), parent);
    }

    public void DeSpawnTrap(Trap trap)
    {
        trap.transform.SetParent(null);

        PoolManager.Instance.TakeToPool<Trap>(trap.name, trap);
    }

    public Weapon SpawnWeapon(WeaponType type, Tier tier, Transform parent)
    {
        Weapon weapon = PoolManager.Instance.GetFromPool<Weapon>("Weapon"); // ���� ����

        // ��ġ ����
        weapon.transform.SetParent(parent);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localScale = Vector3.one;

        // ���� ����
        weapon.SetWeapon(CSVManager.Instance.csvList.FindWeapon(type, tier));

        return weapon;
    }

    public Weapon SpawnWeapon_Ran(Transform parent)
    {
        int ranType = RandomID(System.Enum.GetValues(typeof(WeaponType)).Length);

        Tier ranTier = LuckToTier();

        return SpawnWeapon((WeaponType)ranType, ranTier, parent);
    }

    public void DeSpawnWeapon(Weapon weapon)
    {
        weapon.transform.SetParent(null);

        PoolManager.Instance.TakeToPool<Weapon>("Weapon", weapon);
    }

    public Relic SpawnRelic(int index, Transform parent)
    {
        Relic relic = PoolManager.Instance.GetFromPool<Relic>("Relic"); // ���� ����

        // ��ġ ����
        relic.transform.SetParent(parent);
        relic.transform.localPosition = Vector3.zero;
        relic.transform.localScale = Vector3.one;

        // ���� ����
        relic.SetRelic(index);

        return relic;
    }

    public Relic SpawnRelic_Ran(Transform parent)
    {
        int ranRelic = RandomID(CSVManager.Instance.csvList.weaponDatas.Length);

        return SpawnRelic(ranRelic, parent);
    }

    public void DeSpawnRelic(Relic relic)
    {
        relic.transform.SetParent(null);

        PoolManager.Instance.TakeToPool<Relic>("Relic", relic);
    }

    public Tier LuckToTier()
    {
        int[] luck = GameManager.Instance.luck;
        int sum = luck.Sum();

        int ranTier = Random.Range(0, sum);

        for (int i = 0; i < luck.Length; i++)
        {
            if (ranTier < luck[i])
                return (Tier)i;
        }

        return Tier.Common;
    }

    public int RandomID(int last)
    {
        return Random.Range(0, last);
    }
}