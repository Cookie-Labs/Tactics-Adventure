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
    public Transform relicIconParent, buffIconParent;
    public BossSpawn bossSpawn;
    [HideInInspector] public Card_Player playerCard;
    [HideInInspector] public List<Card_Coin> coinCardList; // 코인 카드 리스트
    [HideInInspector] public List<Card> turnCardList; // 매턴 작동하는 카드 리스트
    [HideInInspector] public List<BuffIcon> buffIconList;

    private void Start()
    {
        SpawnStage();
    }

    private void SpawnStage() // 후에 씬 전환에서 InGame씬으로 전환할때 사용되도록 바꿀 것임
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
                playerCard = SpawnCard(CardType.Player, 4).GetComponent<Card_Player>();
                continue;
            }

            // 플레이어 이외 카드 소환
            for (int j = 0; j < spawnCount[i]; j++) // -> j -> spawnCount[i] 순환 (타입 수)
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

        playerCard.SetNeighbor(); // 플레이어 카드의 주변 카드 터치 활성화

        bossSpawn.SetMaxValue();

        UIManager uiManager = UIManager.Instance;
        uiManager.CheckSkillUI();
        uiManager.handUI.HandImgUI();
        uiManager.handUI.HandAlphaUI();
        uiManager.handUI.WeaponIconUI();
    }

    public void DoTurn()
    {
        for (int i = turnCardList.Count - 1; i >= 0; i--)
            turnCardList[i].DoTurnCard();
        for (int i = buffIconList.Count - 1; i >= 0; i--)
            buffIconList[i].DoTurnBuff();

        SpawnCard_Turn();
        CoinBingo();
        SortCard();
    }

    public void CoinBingo()
    {
        if (coinCardList.Count < 3)
            return;

        List<Card_Coin> changeCards = new List<Card_Coin>(); // 변경할 코인 카드 리스트

        // 가로 및 세로 빙고 확인
        for (int i = 0; i < 3; i++)
        {
            // 가로 빙고 확인
            Card_Coin[] rowCoins = coinCardList.Where(coinCard => coinCard.pos / 3 == i && !coinCard.isChange).ToArray();
            if (rowCoins.Length == 3)
            {
                changeCards.AddRange(rowCoins);
            }

            // 세로 빙고 확인
            Card_Coin[] colCoins = coinCardList.Where(coinCard => coinCard.pos % 3 == i && !coinCard.isChange).ToArray();
            if (colCoins.Length == 3)
            {
                changeCards.AddRange(colCoins);
            }
        }

        // 중복 제거
        changeCards = changeCards.Distinct().ToList();

        // 변경할 코인 카드들 변경
        foreach (Card_Coin card in changeCards)
        {
            ChangeCoinCard(card, card.amount * 2, true);
        }
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
        if (card.isTurn)
            turnCardList.Add(card);
        if(card.type == CardType.Coin)
            coinCardList.Add(card.GetComponent<Card_Coin>());

        return card;
    }

    public Card SpawnRanCard(int pos)
    {
        List<int> typeIDList = new List<int>(); // 타입ID를 담을 리스트 (정수형)

        // 랜덤 타입 정하기
        // 확률 설정
        for (int i = 0; i < maxCard.Length; i++)
            typeIDList.AddRange(Enumerable.Repeat(i, maxCard[i])); // maxCard[i]만큼 i를 리스트에 추가한다

        // 이미 카드가 존재한다면 return
        if (cardPos[pos].childCount > 0)
            return null;

        return SpawnCard((CardType)typeIDList[Random.Range(0, typeIDList.Count)], pos); // 카드 소환!
    }

    public void SpawnCard_Turn()
    {
        List<int> emptyPos = new List<int>();

        for (int i = 0; i < cardPos.Length; i++)
        {
            if (cardPos[i].childCount == 0)
            {
                emptyPos.Add(i);
            }
        }

        foreach(int i in emptyPos)
        {
            SpawnRanCard(i);
        }
    }

    public void DeSpawnCard(Card card)
    {
        card.DestroyCard();

        PoolManager.Instance.TakeToPool<Card>(card.name, card);

        maxCard[(int)card.type]++; // 해당 카드 최대 수량++

        card.transform.SetParent(null);

        cardList.Remove(card);
        if (card.isTurn)
            turnCardList.Remove(card);
        if (card.type == CardType.Coin)
            coinCardList.Remove(card.GetComponent<Card_Coin>());
    }

    public Card FindCard(int posID)
    {
        return cardPos[posID].GetComponentInChildren<Card>();
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

    public Card ChangeCard(Card oriCard, CardType type)
    {
        Card newCard = SpawnCard(type, oriCard.pos);
        DeSpawnCard(oriCard);
        playerCard.SetNeighbor();
        return newCard;
    }

    public void ChangeCoinCard(Card oriCard, int amount, bool isChanged)
    {
        Card_Coin coinCard = ChangeCard(oriCard, CardType.Coin).GetComponent<Card_Coin>();

        coinCard.ChangeAmount(amount, isChanged);
    }

    public void ChangeAllCard()
    {
        int typeCount = Random.Range(0, System.Enum.GetValues(typeof(CardType)).Length);

        for (int i = 0; i < cardPos.Length; i++)
        {
            Card card = FindCard(i);

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
            Card card = FindCard(i);

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

    private void SortCard()
    {
        cardList.Sort((card1, card2) => card1.pos.CompareTo(card2.pos)); 
    }
    #endregion

    #region 플레이어
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
    #endregion

    #region 몬스터
    public Monster SpawnMonster(string name, Transform parent)
    {
        Monster monster = PoolManager.Instance.GetFromPool<Monster>("Monster_" + name); // 플레이어 소환
        monster.SetMonster(name);

        // 위치 설정
        monster.transform.SetParent(parent);
        monster.transform.localPosition = Vector3.zero;
        monster.transform.localScale = Vector3.one;

        return monster;
    }

    public Monster SpawnMonster_Ran(Transform parent)
    {
        CSVManager csvManager = CSVManager.Instance;

        // 보스가 나올 차례라면
        if (bossSpawn.curBossCount >= bossSpawn.maxBossCount)
        {
            bossSpawn.isSpawn = true;
            // 서브 보스 등장
            if(bossSpawn.curBossClear < bossSpawn.maxBossClear)
                return SpawnMonster(csvManager.availBosses.Find(item => item.type == MonsterType.SubBoss).name, parent);
            // 보스 등장
            else if(bossSpawn.curBossClear >= bossSpawn.maxBossClear)
                return SpawnMonster(csvManager.availBosses.Find(item => item.type == MonsterType.Boss).name, parent);

            bossSpawn.curBossCount = 0;
        }

        // 일반 몬스터 등장
        List<string> stageMons = csvManager.availMosters;
        return SpawnMonster(stageMons[Random.Range(0, stageMons.Count)], parent);
    }

    public void DeSpawnMonster(Monster monster)
    {
        monster.transform.SetParent(null);

        PoolManager.Instance.TakeToPool<Monster>("Monster_" + monster.data.name, monster);
    }
    #endregion

    #region 코인
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
    #endregion

    #region 상자
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
    #endregion

    #region 아이템
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
    #endregion

    #region 트랩
    public Trap SpawnTrap(TrapType trapName, Transform parent)
    {
        Trap trap = PoolManager.Instance.GetFromPool<Trap>("Trap_" + trapName); // 트랩 생성

        // 위치 설정
        trap.transform.SetParent(parent);
        trap.transform.localPosition = Vector3.zero;
        trap.transform.localScale = Vector3.one;

        // 변수 설정
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

    #region 무기
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

        Tier ranTier = CSVManager.Instance.luck.LuckToTier();

        return SpawnWeapon((WeaponType)ranType, ranTier, parent);
    }

    public void DeSpawnWeapon(Weapon weapon)
    {
        weapon.transform.SetParent(null);

        PoolManager.Instance.TakeToPool<Weapon>("Weapon", weapon);
    }
    #endregion

    #region 유물
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

    public BuffIcon SpawnBuffIcon(BuffIconType type, int count)
    {
        BuffIcon icon = buffIconList.Find(data => data.buffType == type);

        // 새로 생성되는 버프라면,
        if (icon == null)
        {
            icon = PoolManager.Instance.GetFromPool<BuffIcon>("BuffIcon");

            buffIconList.Add(icon);
        }

        icon.SetBuff(CSVManager.Instance.csvList.FindBuffIconData(type), count);

        return icon;
    }

    public void DeSpawnBuffIcon(BuffIconType type)
    {
        BuffIcon icon = System.Array.Find(buffIconParent.GetComponentsInChildren<BuffIcon>(), data => data.buffType == type);

        buffIconList.Remove(icon);
        PoolManager.Instance.TakeToPool<BuffIcon>("BuffIcon", icon);
    }
    #endregion

    #region 이펙트
    public Transform SpawnEffect(EffectType type, Transform parent)
    {
        Transform effect = PoolManager.Instance.GetFromPool<Transform>(type.ToString()); // 이펙트 소환

        effect.name = effect.name.Replace("(Clone)", "");

        // 위치 설정
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

[System.Serializable]
public class BossSpawn
{
    public int maxBossCount;
    [ReadOnly] public int curBossCount; // 다음 스폰 까지 남은 보스 수
    public int maxBossClear;
    [ReadOnly] public int curBossClear; // 현재 클리어한 보스 수
    public bool isSpawn;

    public void SetMaxValue()
    {
        int stage = (int)GameManager.Instance.stage;
        maxBossCount += stage;
        maxBossClear += 2 * stage; // 등차수열 1, 3, 5, 7, 9...
    }
}