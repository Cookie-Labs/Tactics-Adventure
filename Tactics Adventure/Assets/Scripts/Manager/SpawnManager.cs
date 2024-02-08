using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;
using System.Linq;
using Sirenix.OdinInspector;

public class SpawnManager : Singleton<SpawnManager>
{
    [Title("변수", subtitle: "다른 스크립트 이용위해 저장")]
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
        // 종류별 소환될 카드 개수 (총 10개)
        // Player, Chest, Coin, Consumable, Monster, Relics, Trap, Weapon, Empty
        int[] spawnCount = { 1, 1, 1, 1, 3, 0, 1, 1, 0 };
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
        playerCard.SetNeighbor(); // 플레이어 카드의 주변 카드 터치 활성화
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

    public Card SpawnRanCard()
    {
        List<int> typeIDList = new List<int>(); // 타입ID를 담을 리스트 (정수형)

        // 랜덤 타입 정하기
        // 확률 설정
        for (int i = 0; i < maxCard.Length; i++)
        {
            typeIDList.AddRange(Enumerable.Repeat(i, maxCard[i])); // maxCard[i]만큼 i를 리스트에 추가한다
        }

        // 빈 pos 찾기
        int pos = System.Array.FindIndex(cardPos, cp => cp.childCount == 0); // 자식의 수가 0인 pos

        // 만약 빈 pos 가 없다면 (버그)
        if (pos == -1)
        {
            Debug.LogError("빈 pos가 없어서 카드가 소환되지 않았습니다.");
            return null;
        }

        return SpawnCard((CardType)typeIDList[Random.Range(0, typeIDList.Count)], pos); // 카드 소환!
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
        Player player = PoolManager.Instance.GetFromPool<Player>("Player_" + type); // 플레이어 소환

        // 위치 설정
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
        Monster monster = PoolManager.Instance.GetFromPool<Monster>("Monster_" + type); // 플레이어 소환

        // 위치 설정
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

            if (CSVManager.Instance.availMonStage.Contains(monsterType)) // 알맞은 몬스터를 무작위로 뽑아낸 경우
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
        Coin coin = PoolManager.Instance.GetFromPool<Coin>("Coin"); // 코인 생성

        // 위치 설정
        coin.transform.SetParent(parent);
        coin.transform.localPosition = Vector3.zero;
        coin.transform.localScale = Vector3.one;

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
        Portion portion = PoolManager.Instance.GetFromPool<Portion>("Consumable_Portion"); // 포션 소환

        // 위치 설정
        portion.transform.SetParent(parent);
        portion.transform.localPosition = Vector3.zero;
        portion.transform.localScale = Vector3.one;

        // 포션 설정
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
        Trap trap = PoolManager.Instance.GetFromPool<Trap>("Trap_" + trapName); // 트랩 생성

        // 위치 설정
        trap.transform.SetParent(parent);
        trap.transform.localPosition = Vector3.zero;
        trap.transform.localScale = Vector3.one;

        // 변수 설정
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
        Weapon weapon = PoolManager.Instance.GetFromPool<Weapon>("Weapon"); // 무기 생성

        // 위치 설정
        weapon.transform.SetParent(parent);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localScale = Vector3.one;

        // 변수 설정
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
        Relic relic = PoolManager.Instance.GetFromPool<Relic>("Relic"); // 무기 생성

        // 위치 설정
        relic.transform.SetParent(parent);
        relic.transform.localPosition = Vector3.zero;
        relic.transform.localScale = Vector3.one;

        // 변수 설정
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