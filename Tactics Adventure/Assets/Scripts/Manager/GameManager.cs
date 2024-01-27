using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class GameManager : Singleton<GameManager>
{
    [Title("변수 (저장O)")]
    public PlayerType playerType;
    public int money;

    [Title("변수 (저장X)")]
    public Stage stage;
    public Level level;
    public bool isMoving; // true면 카드 클릭 불가

    [Title("수치 변수 (조정 가능)")]
    public int[] coinUnit;
    public int weaponPerDmg;

    public void EarnMoney(int coin)
    {
        money += coin;
    }
}

public enum Stage { Grass = 0, Cave, Swarm, Forest }
public enum Level { Easy = 0, Normal, Hard }
public enum Tier { Common = 0, Rare, Epic, Legend }