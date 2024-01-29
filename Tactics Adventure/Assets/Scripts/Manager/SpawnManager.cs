using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public class SpawnManager : Singleton<SpawnManager>
{
    public int[] maxCard;
    public Transform[] cardPos;
    public List<Card> cardList;
    [HideInInspector] public Card_Player playerCard;
    [HideInInspector] public List<Card> turnCardList; // 매턴 작동하는 카드 리스트

    private void Start()
    {
        SpawnStage();
    }

    private void SpawnStage()
    {
        // 종류별 소환될 카드 개수 (총 9개)
        // Player, Chest, Coin, Consumable, Monster, Relics, Trap, Weapon
        int[] spawnCount = { 1, 1, 1, 1, 3, 0, 1, 1 };
        int length = spawnCount.Length;

        List<int> drawnNumbers = new List<int> { 4 }; // 중복 방지용 리스트 (4는 플레이어 소환 위치)

        for(int i = 0; i < length; i++) // i -> spawnCount 순환 (타입)
        {
            // 플레이어 소환
            if (i == 0)
            {
                SpawnCard((CardType)i, 4);
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
                SpawnCard((CardType)i, ranNum); // 소환
            }
        }

        playerCard = cardList[0].GetComponent<Card_Player>(); // 플레이어 카드 대입
        TouchManager.Instance.SetTouch(playerCard, cardList); // 플레이어 카드의 주변 카드 터치 활성화
    }

    #region 카드
    public Card SpawnCard(CardType type, int pos)
    {
        Card card = PoolManager.Instance.GetFromPool<Card>("Card_" + type); // 카드 소환

        maxCard[(int)type]--; // 해당 카드 최대 수량--

        // 위치 설정
        card.transform.SetParent(cardPos[pos]);
        card.transform.localPosition = Vector3.zero;

        // 변수 설정
        card.pos = pos;

        // 카드 리스트에 추가
        cardList.Add(card);

        return card;
    }

    public void DeSpawnCard(Card card)
    {
        card.DestroyCard();

        PoolManager.Instance.TakeToPool<Card>(card.name, card);

        maxCard[(int)card.type]++; // 해당 카드 최대 수량++

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
        // 순차 검색
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

    public void PlayerCardMove(Card targetCard)
    {
        playerCard.Move(targetCard.pos);
        DeSpawnCard(targetCard);
    }

    public void ChangeCard(Card oriCard, CardType targetType)
    {
        SpawnCard(targetType, oriCard.pos);
        DeSpawnCard(oriCard);
    }
    #endregion

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
        player.transform.SetParent(null);

        PoolManager.Instance.TakeToPool<Player>("Player_" + player.name, player);
    }

    public Monster SpawnMonster(MonsterType type, Transform parent)
    {
        Monster monster = PoolManager.Instance.GetFromPool<Monster>("Monster_" + type); // 플레이어 소환

        // 위치 설정
        monster.transform.SetParent(parent);
        monster.transform.localPosition = Vector3.zero;

        return monster;
    }

    // 후에 스테이지마다 나오는 몬스터 제한
    public Monster SpawnRanMonster(Transform parent)
    {
        int ranMon = Random.Range(0, System.Enum.GetValues(typeof(MonsterType)).Length);

        Monster monster = SpawnMonster((MonsterType)ranMon, parent);

        return monster;
    }

    public void DeSpawnMonster(Monster monster)
    {
        monster.transform.SetParent(null);

        PoolManager.Instance.TakeToPool<Monster>("Monster_" + monster.name, monster);
    }

    public Coin SpawnCoin(int a, Transform parent)
    {
        Coin coin = PoolManager.Instance.GetFromPool<Coin>("Coin"); // 코인 생성

        // 위치 설정
        coin.transform.SetParent(parent);
        coin.transform.localPosition = Vector3.zero;

        // 애니메이션 설정
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
        Chest chest = PoolManager.Instance.GetFromPool<Chest>("Chest_" + type); // 상자 소환

        // 위치 설정
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
        Portion portion = PoolManager.Instance.GetFromPool<Portion>("Consumable_Portion"); // 포션 소환

        // 위치 설정
        portion.transform.SetParent(parent);
        portion.transform.localPosition = Vector3.zero;

        // 포션 설정
        portion.SetPortion(type, amount);

        return portion;
    }

    public void DeSpawnConsumable(Consumable consumable)
    {
        consumable.transform.SetParent(null);

        PoolManager.Instance.TakeToPool<Consumable>("Consumable_Portion", consumable);
    }

    public Trap SpawnTrap(TrapType trapName, Transform parent)
    {
        Trap trap = PoolManager.Instance.GetFromPool<Trap>("Trap_" + trapName); // 트랩 생성

        // 위치 설정
        trap.transform.SetParent(parent);
        trap.transform.localPosition = Vector3.zero;

        // 변수 설정
        trap.SetTrap();

        return trap;
    }

    public Trap SpawnRanTrap(Transform parent)
    {
        int ranTrap = Random.Range(0, System.Enum.GetValues(typeof(TrapType)).Length);

        return SpawnTrap((TrapType)ranTrap, parent);
    }

    public void DeSpawnTrap(Trap trap)
    {
        trap.transform.SetParent(null);

        PoolManager.Instance.TakeToPool<Trap>(trap.name, trap);
    }

    public Weapon SpawnWeapon(WeaponType type, Tier tier, Transform parent)
    {
        Weapon weapon = PoolManager.Instance.GetFromPool<Weapon>("Weapon"); // 무기 생성

        // 위치 설정
        weapon.transform.SetParent(parent);
        weapon.transform.localPosition = Vector3.zero;

        // 변수 설정
        weapon.SetWeapon(CSVManager.Instance.csvList.FindWeapon(type, tier));

        return weapon;
    }

    public void DeSpawnWeapon(Weapon weapon)
    {
        weapon.transform.SetParent(null);

        PoolManager.Instance.TakeToPool<Weapon>("Weapon", weapon);
    }

    public Relic SpawnRelic(int index, Transform parent)
    {
        Relic relic = PoolManager.Instance.GetFromPool<Relic>("Relic"); // 무기 생성

        // 위치 설정
        relic.transform.SetParent(parent);
        relic.transform.localPosition = Vector3.zero;

        // 변수 설정
        relic.SetRelic(index);

        return relic;
    }

    public void DeSpawnRelic(Relic relic)
    {
        relic.transform.SetParent(null);

        PoolManager.Instance.TakeToPool<Relic>("Relic", relic);
    }
}
