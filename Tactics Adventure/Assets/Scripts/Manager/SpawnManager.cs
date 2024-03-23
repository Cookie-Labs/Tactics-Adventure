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
    public Transform relicIconParent, buffIconParent;
    [HideInInspector] public Card_Player playerCard;
    [HideInInspector] public List<Card> turnCardList; // ���� �۵��ϴ� ī�� ����Ʈ

    private void Start()
    {
        SpawnStage();
    }

    private void SpawnStage() // �Ŀ� �� ��ȯ���� InGame������ ��ȯ�Ҷ� ���ǵ��� �ٲ� ����
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
                SpawnCard(CardType.Player, 4);
                continue;
            }

            // �÷��̾� �̿� ī�� ��ȯ
            for (int j = 0; j < spawnCount[i]; j++) // -> j -> spawnCount[i] ��ȯ (Ÿ�� ��)
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

        UIManager uiManager = UIManager.Instance;
        uiManager.CheckSkillUI();
        uiManager.handUI.HandImgUI();
        uiManager.handUI.HandAlphaUI();
        uiManager.handUI.WeaponIconUI();
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
        if (card.isTurn)
            turnCardList.Add(card);

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
        playerCard.SetNeighbor();
        return newCard;
    }

    public void ChangeCoinCard(Card oriCard, int amount)
    {
        Card_Coin coinCard = ChangeCard(oriCard, CardType.Coin).GetComponent<Card_Coin>();

        coinCard.ChangeAmount(amount);
    }

    public void ChangeAllCard()
    {
        int typeCount = Random.Range(0, System.Enum.GetValues(typeof(CardType)).Length);

        for (int i = 0; i < cardPos.Length; i++)
        {
            Card card = cardPos[i].GetComponentInChildren<Card>();

            if (card.type == CardType.Player)
                continue;

            CardType ranType = (CardType)Random.Range(1, typeCount);

            ChangeCard(card, ranType);
        }
    }

    public void DuplicateAllCard()
    {
        CardType ranType = cardList[Random.Range(0, cardList.Count)].type;

        if(ranType == CardType.Player)
        {
            DuplicateAllCard();
            return;
        }

        for (int i = 0; i < cardPos.Length; i++)
        {
            Card card = cardPos[i].GetComponentInChildren<Card>();

            if (card.type == CardType.Player)
                continue;

            ChangeCard(card, ranType);
        }
    }

    public IEnumerator ShuffleCard(Card selCard, Card tarCard)
    {
        int selPos = selCard.pos;
        int tarPos = tarCard.pos;

        selCard.transform.SetParent(cardPos[tarPos]);
        selCard.pos = tarPos;
        selCard.transform.localPosition = Vector3.zero;

        yield return tarCard.Move(selPos);
    }

    public IEnumerator ShuffleRanCard(Card selCard)
    {
        Card ranCard = cardList[Random.Range(0, cardList.Count)];

        if(selCard == ranCard)
        {
            ShuffleRanCard(selCard);
            yield break;
        }

        yield return ShuffleCard(selCard, ranCard);
    }

    public void ShuffleAll()
    {
        List<int> posList = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 };

        foreach (Card card in cardList)
        {
            int ranPos = posList[Random.Range(0, posList.Count)];

            card.transform.SetParent(cardPos[ranPos]);
            card.pos = ranPos;
            card.transform.localPosition = Vector3.zero;

            posList.Remove(ranPos);
        }
    }
    #endregion

    #region �÷��̾�
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
    #endregion

    #region ����
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
    #endregion

    #region ����
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
    #endregion

    #region ����
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
    #endregion

    #region ������
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
    #endregion

    #region Ʈ��
    public Trap SpawnTrap(TrapType trapName, Transform parent)
    {
        Trap trap = PoolManager.Instance.GetFromPool<Trap>("Trap_" + trapName); // Ʈ�� ����

        // ��ġ ����
        trap.transform.SetParent(parent);
        trap.transform.localPosition = Vector3.zero;
        trap.transform.localScale = Vector3.one;

        // ���� ����
        trap.SetTrap(trapName);

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
    #endregion

    #region ����
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

        Tier ranTier = CSVManager.Instance.luck.LuckToTier();

        return SpawnWeapon((WeaponType)ranType, ranTier, parent);
    }

    public void DeSpawnWeapon(Weapon weapon)
    {
        weapon.transform.SetParent(null);

        PoolManager.Instance.TakeToPool<Weapon>("Weapon", weapon);
    }
    #endregion

    #region ����
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
        CSVList csvList = CSVManager.Instance.csvList;

        int ranRelic = RandomID(csvList.relicDatas.Length);

        if (RelicManager.Instance.CheckRelicCollection(ranRelic))
            return SpawnRelic_Ran(parent);

        return SpawnRelic(ranRelic, parent);
    }

    public void DeSpawnRelic(Relic relic)
    {
        relic.transform.SetParent(null);

        PoolManager.Instance.TakeToPool<Relic>("Relic", relic);
    }
    #endregion

    #region UI
    public RelicIcon SpawnRelicIcon(int ID)
    {
        RelicIcon icon = PoolManager.Instance.GetFromPool<RelicIcon>("RelicIcon");

        icon.SetIcon(ID);

        return icon;
    }

    public void DeSpawnRelicIcon(int ID)
    {
        RelicIcon[] icons = relicIconParent.GetComponentsInChildren<RelicIcon>();

        PoolManager.Instance.TakeToPool<RelicIcon>("RelicIcon", System.Array.Find(icons, icon => icon.ID == ID));
    }

    public BuffIcon SpawnBuffIcon(string name, int count)
    {
        BuffIcon icon = PoolManager.Instance.GetFromPool<BuffIcon>("BuffIcon");

        icon.SetBuff(CSVManager.Instance.csvList.FindBuffIconData(name), count);

        return icon;
    }

    public void DeSpawnBuffIcon(string name)
    {
        BuffIcon[] icons = buffIconParent.GetComponentsInChildren<BuffIcon>();

        PoolManager.Instance.TakeToPool<BuffIcon>("BuffIcon", System.Array.Find(icons, icon => icon.buffName == name));
    }
    #endregion

    #region ����Ʈ
    public Transform SpawnEffect(EffectType type, Transform parent)
    {
        Transform effect = PoolManager.Instance.GetFromPool<Transform>(type.ToString()); // ����Ʈ ��ȯ

        effect.name = effect.name.Replace("(Clone)", "");

        // ��ġ ����
        effect.transform.SetParent(parent);
        effect.transform.localPosition = Vector3.zero;
        effect.transform.localScale = Vector3.one;

        return effect;
    }

    public void DeSpawnEffect(Transform effect)
    {
        effect.transform.SetParent(null);

        PoolManager.Instance.TakeToPool<Transform>(effect.name, effect);
    }

    public Transform SpawnInvincible(Transform parent)
    {
        Transform invincible = SpawnEffect(EffectType.Invincible, parent);

        invincible.transform.localPosition = new Vector3(0, -0.2f, 0);

        // invincible.GetComponentInChildren<TextMeshPro>().text = count.ToString();

        return invincible;
    }
    #endregion

    public int RandomID(int last)
    {
        return Random.Range(0, last);
    }
}

public enum EffectType { Invincible }