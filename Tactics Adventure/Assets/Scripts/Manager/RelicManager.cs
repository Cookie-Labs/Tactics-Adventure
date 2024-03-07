using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicManager : Singleton<RelicManager>
{
    public List<int> collectList;

    public IEnumerator AddRelicList(Relic relic)
    {
        if (CheckRelicCollection(relic.data.index))
            yield break;
        collectList.Add(relic.data.index);
        yield return DoRelic(relic.data.index);
    }

    public IEnumerator AddRelicList(int relicID)
    {
        if (CheckRelicCollection(relicID))
            yield break;
        collectList.Add(relicID);
        yield return DoRelic(relicID);
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
            // ��ũ��: �ִ�ü�� 1 ���� (��)
            case 1:
                player.SetMaxHP(player.player.data.hp + 1);
                break;
            // *�Ϲ��: ü�� MAX (��)
            case 2:
                player.HealHP(player.player.data.hp);
                break;
            // �۷����溣��: ü��&���� MAX (��)
            case 3:
                player.HealHP(player.player.data.hp);
                player.HealMP(player.player.data.mp);
                break;
            // ��ŷ: 50���� ȹ�� (��)
            case 4:
                csvManager.money.EarnMoney(50);
                break;
            // ������ �о�: 200���� ȹ�� (��)
            case 5:
                csvManager.money.EarnMoney(200);
                break;
            // ��������: ���� MAX (��)
            case 6:
                player.HealMP(player.player.data.mp);
                break;
            // ����: ������ ���� ȹ�� (��)
            case 8:
                EarnWeapon(csvList.weaponDatas);
                break;
            // �߽���: ������ ���� ���� ȹ�� (��)
            case 9:
                EarnWeapon(csvList.FindWeapon(WeaponAttribute.Physics));
                break;
            // �缭: ������ ���� ���� ȹ�� (��)
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
            // ���: ��� ���� ���� 3 (��)
            case 13:
                delay += AtkAll(3);
                break;
            // �ζ�: ���� ��� �� 1% Ȯ���� 500���� ȹ�� (��)
            case 14:
                player.isLotto = true;
                break;
            // �ӻ����: ������ ���� ���� (��)
            case 15:
                PortionType ranType = (PortionType)Random.Range(0, System.Enum.GetValues(typeof(PortionType)).Length);
                int amount = Random.Range(0, 5);

                switch (ranType)
                {
                    case PortionType.HP:
                        player.HealHP(amount);
                        break;
                    case PortionType.MP:
                        player.HealMP(amount);
                        break;
                    case PortionType.Poison:
                        player.GetPoison(amount);
                        break;
                }
                break;
            // ���尩: ���ݷ� +1 (��)
            case 16:
                player.bonusDmg++;
                break;
            // ���� ������: ���ݷ� +2 (��)
            case 17:
                player.bonusDmg += 2;
                break;
            // ������: ���ݷ� +3 (��)
            case 18:
                player.bonusDmg += 3;
                break;
            // �͵�: �� ���⿡ ���ݷ� +3 (��)
            case 19:
                player.UpDmg(3);
                break;
            // ����: �� ���⿡ �������� �ο� (��)
            case 20:
                player.EnforceWeapon(EnforceID.Drain);
                break;
            // �ϱ� �ֹ���: 60% �� ���� ���ݷ� +5 (��)
            case 21:
                player.UpDmg(OrderSheet(0.6f, 5));
                break;
            // �߱� �ֹ���: 40% �� ���� ���ݷ� +10 (��)
            case 22:
                player.UpDmg(OrderSheet(0.4f, 10));
                break;
            // ��� �ֹ���: 10% �� ���� ���ݷ� +25 (��)
            case 23:
                player.UpDmg(OrderSheet(0.1f, 25));
                break;
            // ��Ÿ: �� �ѹ� ��� ���� ���� (��)
            case 24:
                player.UpInvincible(1);
                break;
            // ��������: �ִ� ü�� +3 (��)
            case 25:
                player.SetMaxHP(player.player.data.hp + 3);
                break;
            // �� +7 (��)
            case 26:
                player.UpDefend(7);
                break;
            // ��Ű: �� 10% ���� (��)
            case 27:
                luck.GainLuck(1.1f);
                break;
            // ���й�ġ��: �� ���� �Ұ� �׸�ŭ ���� ���ݷ�++ (��)
            case 28:
                int dmg = player.defend;
                player.defend = 0;
                player.UpDmg(dmg);
                break;
            // �޴�����: ���� 5 ���, ü���� 3 ȸ�� (��)
            case 29:
                player.UpDefend(5);
                player.HealHP(3);
                break;
            // ��ũ����: �� +15 (��)
            case 30:
                player.UpDefend(15);
                break;
            // ���ð���: ��� �������� 1��ŭ �� �޽��ϴ�. (��)
            case 31:
                player.reduceDmg++;
                break;
            // ����: ��� ī�� ������ ��迭
            case 32:
                // ���� ���� �� ����
                break;
            // �Ƶ巹����: ü�� ȸ���� +1
            case 33:
                // ���ʽ� ü�� ���� �� ����
                break;
            // �β����Ǻ�: �� ȸ���� +1
            case 34:
                // �� ȸ�� ���� �� ����
                break;
            // ������: �ִ� ü�� -1, ���ݷ� +1
            case 35:
                player.SetMaxHP(player.player.data.hp - 1);
                // ���ʽ�(�⺻) ���ݷ� ���� �� ����
                break;
            // ���кμ���: ���� ���� �Ұ� ��� ���Ϳ��� ����
            case 36:
                delay += AtkAll(player.defend);
                break;
            // ���: �������� ü��ȸ�� �Ұ�, ����� ��� ����
            case 37:
                // ���� ���� �� ����
                break;
            // �����ǰ��: �ִ� ü�� 1, ���� 15
            case 38:
                player.SetMaxHP(1);
                // ���� ���� �� ����
                break;
            // ���� ��ȯ: ������ ī��� ��ġ �ٲ�
            case 39:
                // ī�� ��ȯ ���� �� ����
                break;
            // ����: ������ ���� ����
            case 40:
                // ���� �Ŵ��� �ϼ� �� ����
                break;
            // ����: ��� ī�� ������ ī��� ����
            case 41:
                break;
            // ����: �� 20 ���� (�Ҹ꼺)
            case 42:
                break;
            // ��ȯ: ī�� �ϳ� ���� �� ��ġ �ٲ�
            case 43:
                break;
            // �𵥵�ȭ: �����ǰ� ü������ ȿ�� �ٲ�
            case 44:
                break;
            // ����Ŭ�ι�: �� ���� ����
            case 45:
                break;
            // ����������: ��� ����
            case 46:
                break;
            // ��ȥ���: 10���� ���� óġ �� ���ݷ� 1 ����
            case 47:
                break;
            // ������: ���� óġ�� �߰����� ����ŭ ���ݷ� ����
            case 48:
                break;
            // ����: �� ���� ����
            case 49:
                break;
            // ����*: ��Ȱ �߰�
            case 53:
                break;
            // ���þȷ귿: 90% ���, 10% Ŭ����
            case 54:
                // ���ӿ���, Ŭ���� ���� �� ����
                break;
            // ����: 50% 100����+, 50% 50����-
            case 55:
                bool prob = luck.Probability(50);
                if (prob)
                    csvManager.money.EarnMoney(100);
                else
                    csvManager.money.LoseMoney(50);
                break;
            // �÷�: ���� ü�� 1 �Ұ�, ���ݷ� 1 ����
            case 56:
                break;
            // ��: ����
            case 57:
                break;
            // ��¥: ��ų�� �����Ҹ� ���� �� �� ��밡��
            case 58:
                break;
            // ��¥����: ��ų�� �����Ҹ� ���� 5�� ��밡��
            case 59:
                break;
            // ���� 404: ...
            case 60:
                break;
            // �����ı���: ���⸦ �ı� �� ��� ���Ϳ��� �׸�ŭ ���� (���Ⱑ ���ٸ� 1 ����)
            case 61:
                dmg = player.GetEquipWeapon().plus.dmg;
                if (dmg == 0)
                    delay += AtkAll(1);
                else
                    delay += AtkAll(dmg);
                player.GetEquipWeapon() = new WeaponData();
                break;
            // ����ѹ���: ���� ��� ���⸦ ����ϴ�.
            case 62:
                player.EquipWeapon(csvList.FindWeapon(Tier.Legend).index);
                break;
            // ����: ���ٱ� ����ϴ�
            case 63:
                player.EquipWeapon(28);
                break;
            // ��������: ���ݷ��� 5�ϵ��� 1 ������
            case 64:
                break;
            // ����: ���� ������ 10%��ŭ ���� ����
            case 65:
                csvManager.money.EarnMoney(Mathf.RoundToInt(csvManager.money.money * 1.1f));
                break;
            // �๰�� ��: ���ݷ� 20 ����, ���� ���ݷ� 1 ����
            case 66:
                break;
            // ����������: ������ Ǯ���ּ���
            case 67:
                break;
            // ����: �ִ�ü�� 6+
            case 69:
                player.SetMaxHP(player.player.data.hp + 6);
                break;
            // ��ũ��: ���ݷ� 2 ����, �ִ�ü�� 10 ����
            case 70:
                break;
            // ��������: ���� ��� ���� ��
            case 72:
                player.EquipWeapon(csvList.FindWeapon(Tier.Rare).index);
                break;
            // ū����: ���� ��� ���⸦ ��
            case 73:
                player.EquipWeapon(csvList.FindWeapon(Tier.Epic).index);
                break;
            // ������: ��� ĳ���� 2 ����(�ڽŵ�)
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
            // �ɺ�: ������ ���� ���� ����
            case 75:
                break;
            // Ȳ�ݵ���: ������ ������ ���⸦ ���� (�ִ� 300)
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
            // �м�: ������ ���⸦ �� �ܰ� ���� ����� ������ ����� �ٲ�
            case 78:
                break;
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

    private int OrderSheet(float percent, int amount)
    {
        bool Success = CSVManager.Instance.luck.Probability(percent);

        return Success ? amount : 0;
    }

    private float AtkAll(int dmg)
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
