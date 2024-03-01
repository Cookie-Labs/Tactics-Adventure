using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicManager : Singleton<RelicManager>
{
    public List<int> collectList;

    public void AddRelicList(Relic relic)
    {
        collectList.Add(relic.data.index);
    }

    public bool CheckRelicCollection(int relicID)
    {
        return collectList.Contains(relicID);
    }

    public IEnumerator DoRelic(int relicID, Card_Player player)
    {
        float delay = 0f;
        switch(relicID)
        {
            // 벌크업: 최대체력 1 증가
            case 1:
                player.player.data.hp++;
                break;
            // *일밴드: 체력 MAX
            case 2:
                player.HealHP(player.player.data.hp);
                break;
            // 글래스톤베리: 체력&마나 MAX
            case 3:
                player.HealHP(player.player.data.hp);
                player.HealMP(player.player.data.mp);
                break;
            // 슈킹: 50코인 획득
            case 4:
                GameManager.Instance.EarnMoney(50);
                break;
            // 은행을 털어: 200코인 획득
            case 5:
                GameManager.Instance.EarnMoney(200);
                break;
            // 구름과자 : 마나 MAX
            case 6:
                player.HealMP(player.player.data.mp);
                break;
            // 보급: 무작위 무기 획득
            case 8:
                // 티어 정리 후 제작
                break;
            // 견습생: 무작위 물리 무기 획득
            case 9:
                // 무기 세분화 후 제작
                break;
            // 사서: 무작위 마법 무기 획득
            case 10:
                // 무기 세분화 후 제작
                break;
            // VIP: 상점 30% 할인
            case 11:
                // 상점 제작 후 제작
                break;
            // VVIP: 상점 50% 할인
            case 12:
                // 상점 제작 후 제작
                break;
            // 쇠약: 모든 몬스터 피해 3
            case 13:
                List<Card> monList = SpawnManager.Instance.cardList.FindAll(card => card.type == CardType.Monster);
                float maxDelay = 0f;

                foreach (Card card in monList)
                {
                    StartCoroutine(card.Damaged(3));
                    maxDelay = Mathf.Max(delay, card.animTime);
                }
                delay = maxDelay;
                break;
            // 로또: 몬스터 처치 시 1% 확률로 500코인 획득
            case 14:
                // 운 패치 후 제작
                break;
        }

        yield return new WaitForSeconds(delay);
    }
}
