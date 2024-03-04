using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class GameManager : Singleton<GameManager>
{
    [Title("���� (����O)")]
    public PlayerType playerType;
    public int totalMoney;

    [Title("���� (����X)")]
    public Stage stage;
    public Level level;

    [Title("��ġ ���� (���� ����)")]
    public int[] coinUnit;
    public int maxPortion;
}

public enum Stage { Grass = 0, Cave, Swarm, Forest }
public enum Level { Easy = 0, Normal, Hard }
public enum Tier { Common = 0, Rare, Epic, Legend }