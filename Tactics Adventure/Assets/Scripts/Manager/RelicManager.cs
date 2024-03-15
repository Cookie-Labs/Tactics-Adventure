using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicManager : Singleton<RelicManager>
{
    // 14, 37, 44, 47, 51, 52, 68, 75 �������� Ȯ�� �� ���� ��ũ��Ʈ���� ����
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
            // ��絵����: 12�ϸ��� ü�� 3 ȸ�� (��) (�׽�Ʈ ��)
            case 0:
                turnRelicList.Add(new TurnRelic(0, 12));
                break;
            // ��ũ��: �ִ�ü�� 1 ���� (��) (�׽�Ʈ ��)
            case 1:
                player.SetMaxHP(player.player.data.hp + 1);
                break;
            // *�Ϲ��: ü�� MAX (��) (�׽�Ʈ ��)
            case 2:
                player.HealHP(player.player.data.hp);
                break;
            // �۷����溣��: ü��&���� MAX (��) (�׽�Ʈ ��)
            case 3:
                player.HealHP(player.player.data.hp);
                player.HealMP(player.player.data.mp);
                break;
            // ��ŷ: 50���� ȹ�� (��) (�׽�Ʈ ��)
            case 4:
                csvManager.money.EarnMoney(50);
                break;
            // ������ �о�: 200���� ȹ�� (��) (�׽�Ʈ ��)
            case 5:
                csvManager.money.EarnMoney(200);
                break;
            // ��������: ���� MAX (��) (�׽�Ʈ ��)
            case 6:
                player.HealMP(player.player.data.mp);
                break;
            // ����: 15�ϸ��� ��� ���� 3 ������ (��) (�׽�Ʈ ��)
            case 7:
                turnRelicList.Add(new TurnRelic(7, 15));
                break;
            // ����: ������ ���� ȹ�� (��) (�׽�Ʈ ��)
            case 8:
                EarnWeapon(csvList.weaponDatas);
                break;
            // �߽���: ������ ���� ���� ȹ�� (��) (�׽�Ʈ ��)
            case 9:
                EarnWeapon(csvList.FindWeapon(WeaponAttribute.Physics));
                break;
            // �缭: ������ ���� ���� ȹ�� (��) (�׽�Ʈ ��)
            case 10:
                EarnWeapon(csvList.FindWeapon(WeaponAttribute.Magic));
                break;
            // VIP: ���� 30% ����
            case 11:
                // ���� ���� �� ����
                break;
            // VVIP: ���� 50% ����
            case 12:
                // ���� ���� �� ����
                break;
            // ���: ��� ���� ���� 3 (��) (�׽�Ʈ ��)
            case 13:
                delay += AtkAll(3);
                break;
            // �ӻ����: ������ ���� ���� (��) (�׽�Ʈ ��)
            case 15:
                PortionType ranType = (PortionType)Random.Range(0, System.Enum.GetValues(typeof(PortionType)).Length);
                int amount = Random.Range(0, 5);

                Portion(ranType, amount);
                break;
            // ���尩: ���ݷ� +1 (��) (�׽�Ʈ ��)
            case 16:
                player.bonusDmg++;
                break;
            // ���� ������: ���ݷ� +2 (��) (�׽�Ʈ ��)
            case 17:
                player.bonusDmg += 2;
                break;
            // ������: ���ݷ� +3 (��) (�׽�Ʈ ��)
            case 18:
                player.bonusDmg += 3;
                break;
            // �͵�: �� ���⿡ ���ݷ� +3 (��) (�׽�Ʈ ��)
            case 19:
                player.UpDmg(3);
                break;
            // ����: �� ���⿡ �������� �ο� (��) (�׽�Ʈ ��)
            case 20:
                player.EnforceWeapon(EnforceID.Drain);
                break;
            // �ϱ� �ֹ���: 60% �� ���� ���ݷ� +5 (��) (�׽�Ʈ ��)
            case 21:
                player.UpDmg(OrderSheet(0.6f, 5));
                break;
            // �߱� �ֹ���: 40% �� ���� ���ݷ� +10 (��) (�׽�Ʈ ��)
            case 22:
                player.UpDmg(OrderSheet(0.4f, 10));
                break;
            // ��� �ֹ���: 10% �� ���� ���ݷ� +25 (��) (�׽�Ʈ ��)
            case 23:
                player.UpDmg(OrderSheet(0.1f, 25));
                break;
            // ��Ÿ: �� �ѹ� ��� ���� ���� (��) (�׽�Ʈ ��)
            case 24:
                player.UpInvincible(1);
                break;
            // ��������: �ִ� ü�� +3 (��) (�׽�Ʈ ��)
            case 25:
                player.SetMaxHP(player.player.data.hp + 3);
                break;
            // �� +7 (��) (�׽�Ʈ ��)
            case 26:
                player.UpDefend(7);
                break;
            // ��Ű: �� 10% ���� (��) (�׽�Ʈ ��)
            case 27:
                luck.GainLuck(1.1f);
                break;
            // ���й�ġ��: �� ���� �Ұ� �׸�ŭ ���� ���ݷ�++ (��) (�׽�Ʈ ��)
            case 28:
                int dmg = player.defend;
                player.defend = 0;
                player.UpDmg(dmg);
                break;
            // �޴�����: ���� 5 ���, ü���� 3 ȸ�� (��) (�׽�Ʈ ��)
            case 29:
                player.UpDefend(5);
                player.HealHP(3);
                break;
            // ��ũ����: �� +15 (��) (�׽�Ʈ ��)
            case 30:
                player.UpDefend(15);
                break;
            // ���ð���: ��� �������� 1��ŭ �� �޽��ϴ�. (��) (�׽�Ʈ ��)
            case 31:
                player.reduceDmg++;
                break;
            // ����: ��� ī�� ������ ��迭 (��) (�׽�Ʈ ��)
            case 32:
                SpawnManager.Instance.ShuffleAll();
                break;
            // �Ƶ巹����: ü�� ȸ���� +1 (��) (�׽�Ʈ ��)
            case 33:
                player.bonusHeal++;
                break;
            // �β����Ǻ�: �� ȸ���� +1 (��) (�׽�Ʈ ��)
            case 34:
                player.bonusDefend++;
                break;
            // ������: �ִ� ü�� -1, ���ݷ� +1 (��) (�׽�Ʈ ��)
            case 35:
                player.SetMaxHP(player.player.data.hp - 1);
                player.bonusDmg++;
                break;
            // ���кμ���: ���� ���� �Ұ� ��� ���Ϳ��� ���� (��) (�׽�Ʈ ��)
            case 36:
                delay += AtkAll(player.defend);
                player.defend = 0;
                break;
            // �����ǰ��: �ִ� ü�� 1, ���� 25 (��) (�׽�Ʈ ��)
            case 38:
                player.SetMaxHP(1);
                player.UpInvincible(25);
                break;
            // ���� ��ȯ: ��� ī�� ������ ī��� ���� (��) (�׽�Ʈ ��)
            case 39:
                SpawnManager.Instance.ChangeAllCard();
                break;
            // ����: ������ ���� ���� (��) (�׽�Ʈ ��)
            case 40:
                RemoveRelicList(collectList[Random.Range(0, collectList.Count - 1)]);
                break;
            // ����: ��� ī�带 �ʵ忡 �����ϴ� ������ ī��Ÿ������ ���� (��) (�׽�Ʈ ��)
            case 41:
                SpawnManager.Instance.DuplicateAllCard();
                break;
            // ����: �� 20 ����, 3�� �� �Ҹ� (��) (�׽�Ʈ ��)
            case 42:
                player.UpDefend(20);
                turnRelicList.Add(new TurnRelic(42, 3));
                break;
            // ��ȯ: ������ ī��� �÷��̾��� ��ġ�� �ٲ� (��) (�׽�Ʈ ��)
            case 43:
                yield return SpawnManager.Instance.ShuffleRanCard(player);
                break;
            // ����Ŭ�ι�: �� 5% ���� (��) (�׽�Ʈ ��)
            case 45:
                luck.GainLuck(1.05f);
                break;
            // ����������: ��� ����
            case 46:
                break;
            // ������: ���� óġ�� �߰����� ����ŭ ���ݷ� ����
            case 48:
                break;
            // ����: �� 20% ���� (��) (�׽�Ʈ ��)
            case 49:
                luck.GainLuck(1.2f);
                break;
            // Ŭ��ġ�ɷ�: 3�� �� ���ݷ��� 5 �����մϴ�. (��) (�׽�Ʈ ��)
            case 50:
                turnRelicList.Add(new TurnRelic(50, 3));
                break;
            // ����*: ��Ȱ �߰� (��) (�׽�Ʈ ��)
            case 53:
                player.rebornCount++;
                break;
            // ���þȷ귿: 90% ���, 10% Ŭ����
            case 54:
                // ���ӿ���, Ŭ���� ���� �� ����
                break;
            // ����: 50% 100����+, 50% 50����- (��) (�׽�Ʈ ��)
            case 55:
                bool prob = luck.Probability(0.5f);
                if (prob)
                    csvManager.money.EarnMoney(100);
                else
                    csvManager.money.LoseMoney(50);
                break;
            // �÷�: �� �� ü�� 1--, ���ݷ� 1++ (�׽�Ʈ ��)
            case 56:
                turnRelicList.Add(new TurnRelic(56, -1));
                break;
            // ��¥: ��ų�� �����Ҹ� ���� �� �� ��밡�� (��) (�׽�Ʈ ��)
            case 58:
                player.freeMP++;
                break;
            // ��¥����: ��ų�� �����Ҹ� ���� 5�� ��밡�� (��) (�׽�Ʈ ��)
            case 59:
                player.freeMP += 5;
                break;
            // �����ı���: ���⸦ �ı� �� ��� ���Ϳ��� �׸�ŭ ���� (���Ⱑ ���ٸ� 1 ����) (��) (�׽�Ʈ ��)
            case 61:
                dmg = player.GetEquipHand().plus.dmg;
                if (dmg == 0)
                    delay += AtkAll(1);
                else
                    delay += AtkAll(dmg);
                player.BreakWeapon(ref player.GetEquipHand());
                break;
            // ����ѹ���: ���� ��� ���⸦ ����ϴ�. (��) (�׽�Ʈ ��)
            case 62:
                player.EquipWeapon(csvList.FindWeapon(Tier.Legend).index);
                break;
            // ����: ���ٱ� ����ϴ� (��) (�׽�Ʈ ��)
            case 63:
                player.EquipWeapon(28);
                break;
            // ��������: ���ݷ��� 5�ϵ��� 1 ������ (��) (�׽�Ʈ ��)
            case 64:
                player.bonusDmg++;
                turnRelicList.Add(new TurnRelic(64, 5));
                break;
            // ����: ���� ������ 10%��ŭ ���� ���� (��) (�׽�Ʈ ��)
            case 65:
                csvManager.money.EarnMoney(Mathf.RoundToInt(csvManager.money.money * 0.1f));
                break;
            // �๰�� ��: ���ݷ� 20 ����, ���� ���ݷ� 1 ���� (��) (�׽�Ʈ ��)
            case 66:
                player.bonusDmg += 21;
                turnRelicList.Add(new TurnRelic(66, -1));
                break;
            // ����������: ������ Ǯ���ּ���
            case 67:
                break;
            // ����: �ִ�ü�� 6+ (��) (�׽�Ʈ ��)
            case 69:
                player.SetMaxHP(player.player.data.hp + 6);
                break;
            // ��ũ��: ���ݷ� 2 ����, �ִ�ü�� 10 ���� (��) (�׽�Ʈ ��)
            case 70:
                player.minusDmg += 2;
                player.SetMaxHP(player.player.data.hp + 10);
                break;
            // �������: 7�� ���� �������� �մϴ�. (��) (�׽�Ʈ ��)
            case 71:
                turnRelicList.Add(new TurnRelic(71, 7));
                break;
            // ��������: ���� ��� ���� �� (��) (�׽�Ʈ ��)
            case 72:
                player.EquipWeapon(csvList.FindWeapon(Tier.Rare).index);
                turnRelicList.Add(new TurnRelic(72, 10));
                break;
            // ū����: ���� ��� ���⸦ �� (��) (�׽�Ʈ ��)
            case 73:
                player.EquipWeapon(csvList.FindWeapon(Tier.Epic).index);
                break;
            // ������: ��� ĳ���� 2 ����(�ڽŵ�) (��)
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
            // Ȳ�ݵ���: ������ ������ ���⸦ ���� (�ִ� 300) (��)
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
            // �м�: ������ ���⸦ �� �ܰ� ���� ����� ������ ����� �ٲ� (��)
            case 78:
                int weaponTierID = (int)player.GetEquipHand().tier;

                weaponTierID = weaponTierID == 0 ? weaponTierID : weaponTierID - 1;

                player.EquipWeapon(csvList.FindWeapon((Tier)weaponTierID).index);
                break;
            // ��������: �ֹ��� 3���� ���� ����ϴ�. (��)
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
    public int turnCount; // -1 -> ���ѷ���

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