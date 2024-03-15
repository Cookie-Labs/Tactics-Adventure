using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicManager : Singleton<RelicManager>
{
    // 14, 37, 44, 47, 51, 52, 68, 75 보유여부 확인 후 관련 스크립트에서 동작
    public List<int> collectList;
    public List<TurnRelic> turnRelicList;

    public IEnumerator AddRelicList(Relic relic)
    {
        int ID = relic.data.index;
        if (CheckRelicCollection(ID))
            yield break;

        collectList.Add(ID);
        SpawnManager.Instance.SpawnRelicIcon(ID);
        yield return DoRelic(ID);
    }

    public IEnumerator AddRelicList(int relicID)
    {
        if (CheckRelicCollection(relicID))
            yield break;

        collectList.Add(relicID);
        SpawnManager.Instance.SpawnRelicIcon(relicID);
        yield return DoRelic(relicID);
    }

    public void RemoveRelicList(int relicID)
    {
        if (!CheckRelicCollection(relicID) || collectList.Count <= 0)
            return;
        collectList.Remove(relicID);
        turnRelicList.Remove(turnRelicList.Find(item => item.relicID == relicID));

        SpawnManager.Instance.DeSpawnRelicIcon(relicID);
    }

    public bool CheckRelicCollection(int relicID)
    {
        return collectList.Contains(relicID);
    }

    public IEnumerator DoRelic(int relicID)
    {
        Card_Player player = SpawnManager.Instance.playerCard;
        CSVManager csvManager = CSVManager.Instance;
        CSVList csvList = csvManager.csvList;
        Luck luck = CSVManager.Instance.luck;

        float delay = 0f;

        switch (relicID)
        {
            // 고당도수박: 12턴마다 체력 3 회복 (완) (테스트 완)
            case 0:
                turnRelicList.Add(new TurnRelic(0, 12));
                break;
            // 벌크업: 최대체력 1 증가 (완) (테스트 완)
            case 1:
                player.SetMaxHP(player.player.data.hp + 1);
                break;
            // *일밴드: 체력 MAX (완) (테스트 완)
            case 2:
                player.HealHP(player.player.data.hp);
                break;
            // 글래스톤베리: 체력&마나 MAX (완) (테스트 완)
            case 3:
                player.HealHP(player.player.data.hp);
                player.HealMP(player.player.data.mp);
                break;
            // 슈킹: 50코인 획득 (완) (테스트 완)
            case 4:
                csvManager.money.EarnMoney(50);
                break;
            // 은행을 털어: 200코인 획득 (완) (테스트 완)
            case 5:
                csvManager.money.EarnMoney(200);
                break;
            // 구름과자: 마나 MAX (완) (테스트 완)
            case 6:
                player.HealMP(player.player.data.mp);
                break;
            // 심판: 15턴마다 모든 몬스터 3 데미지 (완) (테스트 완)
            case 7:
                turnRelicList.Add(new TurnRelic(7, 15));
                break;
            // 보급: 무작위 무기 획득 (완) (테스트 완)
            case 8:
                EarnWeapon(csvList.weaponDatas);
                break;
            // 견습생: 무작위 물리 무기 획득 (완) (테스트 완)
            case 9:
                EarnWeapon(csvList.FindWeapon(WeaponAttribute.Physics));
                break;
            // 사서: 무작위 마법 무기 획득 (완) (테스트 완)
            case 10:
                EarnWeapon(csvList.FindWeapon(WeaponAttribute.Magic));
                break;
            // VIP: 상점 30% 할인
            case 11:
                // 상점 제작 후 제작
                break;
            // VVIP: 상점 50% 할인
            case 12:
                // 상점 제작 후 제작
                break;
            // 쇠약: 모든 몬스터 피해 3 (완) (테스트 완)
            case 13:
                delay += AtkAll(3);
                break;
            // 임상실험: 무작위 포션 섭취 (완) (테스트 완)
            case 15:
                PortionType ranType = (PortionType)Random.Range(0, System.Enum.GetValues(typeof(PortionType)).Length);
                int amount = Random.Range(0, 5);

                Portion(ranType, amount);
                break;
            // 목장갑: 공격력 +1 (완) (테스트 완)
            case 16:
                player.bonusDmg++;
                break;
            // 웨폰 마스터: 공격력 +2 (완) (테스트 완)
            case 17:
                player.bonusDmg += 2;
                break;
            // 슈프림: 공격력 +3 (완) (테스트 완)
            case 18:
                player.bonusDmg += 3;
                break;
            // 맹독: 내 무기에 공격력 +3 (완) (테스트 완)
            case 19:
                player.UpDmg(3);
                break;
            // 착취: 내 무기에 생명력흡수 부여 (완) (테스트 완)
            case 20:
                player.EnforceWeapon(EnforceID.Drain);
                break;
            // 하급 주문서: 60% 내 무기 공격력 +5 (완) (테스트 완)
            case 21:
                player.UpDmg(OrderSheet(0.6f, 5));
                break;
            // 중급 주문서: 40% 내 무기 공격력 +10 (완) (테스트 완)
            case 22:
                player.UpDmg(OrderSheet(0.4f, 10));
                break;
            // 상급 주문서: 10% 내 무기 공격력 +25 (완) (테스트 완)
            case 23:
                player.UpDmg(OrderSheet(0.1f, 25));
                break;
            // 스타: 딱 한번 모든 공격 무시 (완) (테스트 완)
            case 24:
                player.UpInvincible(1);
                break;
            // 버섯스프: 최대 체력 +3 (완) (테스트 완)
            case 25:
                player.SetMaxHP(player.player.data.hp + 3);
                break;
            // 방어도 +7 (완) (테스트 완)
            case 26:
                player.UpDefend(7);
                break;
            // 럭키: 운 10% 증가 (완) (테스트 완)
            case 27:
                luck.GainLuck(1.1f);
                break;
            // 방패밀치기: 방어도 전부 잃고 그만큼 무기 공격력++ (완) (테스트 완)
            case 28:
                int dmg = player.defend;
                player.defend = 0;
                player.UpDmg(dmg);
                break;
            // 휴대용방패: 방어도를 5 얻고, 체력을 3 회복 (완) (테스트 완)
            case 29:
                player.UpDefend(5);
                player.HealHP(3);
                break;
            // 웅크리기: 방어도 +15 (완) (테스트 완)
            case 30:
                player.UpDefend(15);
                break;
            // 가시갑옷: 모든 데미지를 1만큼 덜 받습니다. (완) (테스트 완)
            case 31:
                player.reduceDmg++;
                break;
            // 셔플: 모든 카드 무작위 재배열 (완) (테스트 완)
            case 32:
                SpawnManager.Instance.ShuffleAll();
                break;
            // 아드레날린: 체력 회복량 +1 (완) (테스트 완)
            case 33:
                player.bonusHeal++;
                break;
            // 두꺼운피부: 방어도 회복량 +1 (완) (테스트 완)
            case 34:
                player.bonusDefend++;
                break;
            // 광전사: 최대 체력 -1, 공격력 +1 (완) (테스트 완)
            case 35:
                player.SetMaxHP(player.player.data.hp - 1);
                player.bonusDmg++;
                break;
            // 방패부수기: 방패 전부 잃고 모든 몬스터에게 피해 (완) (테스트 완)
            case 36:
                delay += AtkAll(player.defend);
                player.defend = 0;
                break;
            // 죽음의계약: 최대 체력 1, 무적 25 (완) (테스트 완)
            case 38:
                player.SetMaxHP(1);
                player.UpInvincible(25);
                break;
            // 랜덤 교환: 모든 카드 무작위 카드로 변경 (완) (테스트 완)
            case 39:
                SpawnManager.Instance.ChangeAllCard();
                break;
            // 투기: 무작위 유물 버림 (완) (테스트 완)
            case 40:
                RemoveRelicList(collectList[Random.Range(0, collectList.Count - 1)]);
                break;
            // 지진: 모든 카드를 필드에 존재하는 무작위 카드타입으로 통일 (완) (테스트 완)
            case 41:
                SpawnManager.Instance.DuplicateAllCard();
                break;
            // 무적: 방어도 20 증가, 3턴 후 소멸 (완) (테스트 완)
            case 42:
                player.UpDefend(20);
                turnRelicList.Add(new TurnRelic(42, 3));
                break;
            // 교환: 무작위 카드와 플레이어의 위치를 바꿈 (완) (테스트 완)
            case 43:
                yield return SpawnManager.Instance.ShuffleRanCard(player);
                break;
            // 네잎클로버: 운 5% 증가 (완) (테스트 완)
            case 45:
                luck.GainLuck(1.05f);
                break;
            // 대지가르기: 방사 피해
            case 46:
                break;
            // 레벨업: 현재 처치한 중간보스 수만큼 공격력 증가
            case 48:
                break;
            // 잭팟: 운 20% 증가 (완) (테스트 완)
            case 49:
                luck.GainLuck(1.2f);
                break;
            // 클러치능력: 3턴 후 공격력이 5 증가합니다. (완) (테스트 완)
            case 50:
                turnRelicList.Add(new TurnRelic(50, 3));
                break;
            // 에어*: 부활 추가 (완) (테스트 완)
            case 53:
                player.rebornCount++;
                break;
            // 러시안룰렛: 90% 사망, 10% 클리어
            case 54:
                // 게임오버, 클리어 제작 후 제작
                break;
            // 도박: 50% 100코인+, 50% 50코인- (완) (테스트 완)
            case 55:
                bool prob = luck.Probability(0.5f);
                if (prob)
                    csvManager.money.EarnMoney(100);
                else
                    csvManager.money.LoseMoney(50);
                break;
            // 시련: 매 턴 체력 1--, 공격력 1++ (테스트 완)
            case 56:
                turnRelicList.Add(new TurnRelic(56, -1));
                break;
            // 공짜: 스킬을 마나소모 없이 한 번 사용가능 (완) (테스트 완)
            case 58:
                player.freeMP++;
                break;
            // 공짜쿠폰: 스킬을 마나소모 없이 5번 사용가능 (완) (테스트 완)
            case 59:
                player.freeMP += 5;
                break;
            // 무기파괴술: 무기를 파괴 후 모든 몬스터에게 그만큼 피해 (무기가 없다면 1 피해) (완) (테스트 완)
            case 61:
                dmg = player.GetEquipHand().plus.dmg;
                if (dmg == 0)
                    delay += AtkAll(1);
                else
                    delay += AtkAll(dmg);
                player.BreakWeapon(ref player.GetEquipHand());
                break;
            // 요상한버섯: 전설 등급 무기를 얻습니다. (완) (테스트 완)
            case 62:
                player.EquipWeapon(csvList.FindWeapon(Tier.Legend).index);
                break;
            // 스컬: 뼈다귀 얻습니다 (완) (테스트 완)
            case 63:
                player.EquipWeapon(28);
                break;
            // 작은고추: 공격력이 5턴동안 1 증가함 (완) (테스트 완)
            case 64:
                player.bonusDmg++;
                turnRelicList.Add(new TurnRelic(64, 5));
                break;
            // 이자: 보유 코인의 10%만큼 코인 얻음 (완) (테스트 완)
            case 65:
                csvManager.money.EarnMoney(Mathf.RoundToInt(csvManager.money.money * 0.1f));
                break;
            // 약물의 힘: 공격력 20 증가, 매턴 공격력 1 잃음 (완) (테스트 완)
            case 66:
                player.bonusDmg += 21;
                turnRelicList.Add(new TurnRelic(66, -1));
                break;
            // 개구리왕자: 봉인을 풀어주세요
            case 67:
                break;
            // 거인: 최대체력 6+ (완) (테스트 완)
            case 69:
                player.SetMaxHP(player.player.data.hp + 6);
                break;
            // 살크업: 공격력 2 감소, 최대체력 10 증가 (완) (테스트 완)
            case 70:
                player.minusDmg += 2;
                player.SetMaxHP(player.player.data.hp + 10);
                break;
            // 요술램프: 7턴 동안 문질러야 합니다. (완) (테스트 완)
            case 71:
                turnRelicList.Add(new TurnRelic(71, 7));
                break;
            // 작은지니: 레어 등급 무기 줌 (완) (테스트 완)
            case 72:
                player.EquipWeapon(csvList.FindWeapon(Tier.Rare).index);
                turnRelicList.Add(new TurnRelic(72, 10));
                break;
            // 큰지니: 에픽 등급 무기를 줌 (완) (테스트 완)
            case 73:
                player.EquipWeapon(csvList.FindWeapon(Tier.Epic).index);
                break;
            // 펑펑펑: 모든 캐릭터 2 피해(자신도) (완)
            case 74:
                List<Card> wholeList = SpawnManager.Instance.cardList;
                float maxDelay = 0f;

                foreach (Card card in wholeList)
                {
                    StartCoroutine(card.Damaged(2));
                    maxDelay = Mathf.Max(0f, card.animTime);
                }

                delay += maxDelay;
                break;
            // 황금돼지: 코인을 지불해 무기를 얻음 (최대 300) (완)
            case 77:
                int curMoney = CSVManager.Instance.money.money;
                int expendMoney = 0;
                Tier weaponTier = Tier.Common;

                if (curMoney >= 300)
                {
                    expendMoney = 300;
                    weaponTier = Tier.Legend;
                }
                else if (curMoney >= 150)
                {
                    expendMoney = 150;
                    weaponTier = Tier.Epic;
                }
                else if (curMoney >= 50)
                {
                    expendMoney = 50;
                    weaponTier = Tier.Rare;
                }
                else if (curMoney >= 10)
                {
                    expendMoney = 10;
                    weaponTier = Tier.Common;
                }
                csvManager.money.LoseMoney(expendMoney);
                player.EquipWeapon(csvList.FindWeapon(weaponTier).index);
                break;
            // 압수: 보유한 무기를 한 단계 낮은 등급의 무작위 무기로 바꿈 (완)
            case 78:
                int weaponTierID = (int)player.GetEquipHand().tier;

                weaponTierID = weaponTierID == 0 ? weaponTierID : weaponTierID - 1;

                player.EquipWeapon(csvList.FindWeapon((Tier)weaponTierID).index);
                break;
            // 무지개냥: 주문서 3개를 전부 얻습니다. (완)
            case 79:
                StartCoroutine(AddRelicList(21));
                StartCoroutine(AddRelicList(22));
                StartCoroutine(AddRelicList(23));
                break;
            default:
                break;
        }

        yield return new WaitForSeconds(delay);
    }

    public void DoTurnRelic()
    {
        List<TurnRelic> removeList = new List<TurnRelic>();
        for (int i = 0; i < turnRelicList.Count; i++)
        {
            TurnRelic turnRelic = turnRelicList[i];
            turnRelic.DoTurnRelic();

            if (turnRelic.turnCount == 0)
                removeList.Add(turnRelic);
        }

        turnRelicList.RemoveAll(turnRelic => removeList.Contains(turnRelic));
    }

    private int OrderSheet(float percent, int amount)
    {
        bool Success = CSVManager.Instance.luck.Probability(percent);

        return Success ? amount : 0;
    }

    public void Portion(PortionType type, int amount)
    {
        Card_Player player = SpawnManager.Instance.playerCard;

        if(CheckRelicCollection(44))
        {
            if (type == PortionType.HP)
                type = PortionType.Poison;
            else if (type == PortionType.Poison)
                type = PortionType.HP;
        }

        int totalAmount = amount + player.bonusPortion;

        switch (type)
        {
            case PortionType.HP:
                if (!CheckRelicCollection(37))
                    player.HealHP(totalAmount);
                break;
            case PortionType.MP:
                player.HealMP(totalAmount);
                break;
            case PortionType.Poison:
                player.GetPoison(totalAmount);
                break;
        }
    }

    public float AtkAll(int dmg)
    {
        List<Card> monList = SpawnManager.Instance.cardList.FindAll(card => card.type == CardType.Monster);
        float maxDelay = 0f;

        foreach (Card card in monList)
        {
            StartCoroutine(card.Damaged(dmg));
            maxDelay = Mathf.Max(0f, card.animTime);
        }
        return maxDelay;
    }

    private void EarnWeapon(WeaponData[] datas)
    {
        WeaponData ranWeapon = CSVManager.Instance.luck.TierToWeapon(datas);
        SpawnManager.Instance.playerCard.EquipWeapon(ranWeapon.index);
    }
}

[System.Serializable]
public class TurnRelic
{
    public int relicID;
    public int turnCount; // -1 -> 무한루프

    public TurnRelic(int id, int count)
    {
        relicID = id;
        turnCount = count > 0 ? count + 1 : count;
    }

    public void DoTurnRelic()
    {
        if (turnCount > 0)
            turnCount--;

        Card_Player player = SpawnManager.Instance.playerCard;

        if(turnCount <= 0)
        {
            switch (relicID)
            {
                case 0:
                    player.HealHP(3);
                    turnCount = 12;
                    break;
                case 7:
                    RelicManager.Instance.AtkAll(3);
                    turnCount = 15;
                    break;
                case 42:
                    player.DownDefend(20);
                    break;
                case 50:
                    player.bonusDmg += 3;
                    break;
                case 56:
                    if (player.hp == 1)
                        break;
                    player.StartCoroutine(player.Damaged(1));
                    player.bonusDmg++;
                    break;
                case 63:
                    player.bonusDmg++;
                    break;
                case 64:
                case 66:
                    player.minusDmg++;
                    break;
                case 71:
                    RelicManager.Instance.RemoveRelicList(71);
                    RelicManager.Instance.StartCoroutine(RelicManager.Instance.AddRelicList(72));
                    break;
                case 72:
                    RelicManager.Instance.RemoveRelicList(72);
                    RelicManager.Instance.StartCoroutine(RelicManager.Instance.AddRelicList(73));
                    break;
            }
        }
    }
}