using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class GameManager : Singleton<GameManager>
{
    [Title("변수 (저장O)")]
    public PlayerType playerType;
    public int totalMoney;

    [Title("변수 (저장X)")]
    public Stage stage;
    public Level level;

    [Title("수치 변수 (조정 가능)")]
    public int[] coinUnit;
    public int maxPortion;
}

public enum Stage { Grass = 0, Cave, Swarm, Forest }
public enum Level { Easy = 0, Normal, Hard }
public enum Tier { Common = 0, Rare, Epic, Legend }