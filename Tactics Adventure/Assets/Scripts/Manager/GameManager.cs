using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class GameManager : Singleton<GameManager>
{
    [Title("���� (����O)")]
    public PlayerType playerType;

    [Title("���� (����X)")]
    public Stage stage;
    public Level level;
}

public enum Stage { Grass = 0, Cave, Swarm, Forest }
public enum Level { Easy = 0, Normal, Hard }